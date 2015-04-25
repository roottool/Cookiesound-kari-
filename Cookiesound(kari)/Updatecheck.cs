using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

namespace Cookiesound_kari_
{
    class Updatecheck
    {
        public ManualResetEvent mre;

        public Updatecheck()
        {
            mre = new ManualResetEvent(false);
        }

        // Starts the thread
        /*public void Start()
        {
            checking.Start();
        }*/
        public void Run()
        {
            string s;
            string str;
            string str2;
            string currentVer = "0.3.3";
            try
            {
                mre.Reset();
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead("https://www.dropbox.com/sh/vxnv1zltyr7sais/AADhPLLkZbbH7tppr6DXRVHaa?dl=0"); //Debug
                //Stream st = wc.OpenRead("https://www.dropbox.com/sh/wb47tpk36741rp7/AABEUAPlgnMO8onurQNOvCBta?dl=0"); //main
                StreamReader sr = new StreamReader(st, Encoding.GetEncoding(51932));

                s = sr.ReadToEnd();
                System.Text.RegularExpressions.Regex ver_r = new System.Text.RegularExpressions.Regex(@"[\d|.]+_Cookiesound");
                System.Text.RegularExpressions.Match ver_m = ver_r.Match(s);
                str = System.Text.RegularExpressions.Regex.Replace(ver_m.Value, "_Cookiesound", "");

                st.Close();
                wc.Dispose();

                if (String.Compare(str, currentVer) > 0)
                {
                    MessageBox.Show("アップデートがあります。自動更新後に発生するoldファイルは次回起動時に\r\n削除されるのでそのままでお願いします。");
                    //WebRequestを作成 I used a Public folder in Dropbox.
                    System.Net.HttpWebRequest webreq =
                        (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://www.dropbox.com/s/izgznijak4fj1f5/0.3.4_Cookiesound%28kari%29.exe?dl=0"); //Debug
                        //(System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + str + "_Cookiesound(kari).exe"); //main

                    //サーバーからの応答を受信するためのWebResponseを取得
                    System.Net.HttpWebResponse webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                    //応答データを受信するためのStreamを取得
                    System.IO.Stream strm = webres.GetResponseStream();

                    System.IO.File.Delete("Cookiesound(kari).old");
                    System.IO.File.Move("Cookiesound(kari).exe", "Cookiesound(kari).old");

                    //ファイルに書き込むためのFileStreamを作成
                    System.IO.FileStream fs = new System.IO.FileStream("Cookiesound(kari).exe", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    //応答データをファイルに書き込む
                    byte[] readData = new byte[1024];
                    for (; ; )
                    {
                        //データを読み込む
                        int readSize = strm.Read(readData, 0, readData.Length);
                        if (readSize == 0)
                        {
                            //すべてのデータを読み込んだ時
                            break;
                        }
                        //読み込んだデータをファイルに書き込む
                        fs.Write(readData, 0, readSize);
                    }

                    //閉じる
                    fs.Close();
                    strm.Close();

                    webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/readme.txt");

                    webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                    strm = webres.GetResponseStream();

                    fs = new System.IO.FileStream("readme.txt", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    readData = new byte[1024];
                    for (; ; )
                    {
                        int readSize = strm.Read(readData, 0, readData.Length);
                        if (readSize == 0)
                        {
                            break;
                        }
                        fs.Write(readData, 0, readSize);
                    }

                    fs.Close();
                    strm.Close();

                    if (System.IO.Directory.Exists("sound"))
                    {
                        System.Text.RegularExpressions.MatchCollection mc =
                            System.Text.RegularExpressions.Regex.Matches(s, "\"" + @"[\d|.]+_[\w-|^]+.ogg");
                        ArrayList addsounds = new ArrayList();
                        foreach (System.Text.RegularExpressions.Match m in mc)
                        {
                            addsounds.Add(System.Text.RegularExpressions.Regex.Replace(m.Groups[0].Value, "\"", ""));
                        }
                        for (int i = 0; i < addsounds.Count; i++)
                        {
                            str = System.Text.RegularExpressions.Regex.Replace("" + addsounds[i], @"_[\w-|^]+.ogg", "");
                            if (String.Compare(str, currentVer) > 0)
                            {
                                webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + addsounds[i]);

                                webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                                strm = webres.GetResponseStream();

                                fs = new System.IO.FileStream("sound/" + System.Text.RegularExpressions.Regex.Replace("" + addsounds[i], @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                readData = new byte[1024];
                                for (; ; )
                                {
                                    int readSize = strm.Read(readData, 0, readData.Length);
                                    if (readSize == 0)
                                    {
                                        break;
                                    }
                                    fs.Write(readData, 0, readSize);
                                }

                                fs.Close();
                                strm.Close();
                            }
                        }
                    }
                    else 
                    {
                        MessageBox.Show("Cookiesound(kari).exeと同じフォルダにsoundフォルダが存在しません。");
                        Environment.Exit(0);
                    }

                    if (System.IO.Directory.Exists("sound/csr"))
                    {
                        System.Text.RegularExpressions.MatchCollection mc2 =
                        System.Text.RegularExpressions.Regex.Matches(s, "\"" + @"[\d|.]+_[\w|-|^]+.mp3");

                        ArrayList addcsrs = new ArrayList();
                        foreach (System.Text.RegularExpressions.Match m in mc2)
                        {
                            addcsrs.Add(System.Text.RegularExpressions.Regex.Replace(m.Groups[0].Value, "\"", ""));
                        }
                        for (int i = 0; i < addcsrs.Count; i++)
                        {
                            str = System.Text.RegularExpressions.Regex.Replace("" + addcsrs[i], @"_[\w|-|^]+.mp3", "");
                            if (String.Compare(str, currentVer) > 0)
                            {
                                webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + addcsrs[i]);

                                webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                                strm = webres.GetResponseStream();

                                fs = new System.IO.FileStream("sound/csr/" + System.Text.RegularExpressions.Regex.Replace("" + addcsrs[i], @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                readData = new byte[1024];
                                for (; ; )
                                {
                                    int readSize = strm.Read(readData, 0, readData.Length);
                                    if (readSize == 0)
                                    {
                                        break;
                                    }
                                    fs.Write(readData, 0, readSize);
                                }

                                fs.Close();
                                strm.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("soundフォルダ内にcsrフォルダが存在しません。");
                        Environment.Exit(0);

                    }

                    System.Text.RegularExpressions.Match m2 =
                        System.Text.RegularExpressions.Regex.Match(s, "\"" + @"[\d|.]+_csr_list.txt");
                    str2 = System.Text.RegularExpressions.Regex.Replace(m2.Value, "\"", "");
                    str = System.Text.RegularExpressions.Regex.Replace(str2, @"_csr_list.txt", "");
                    if (String.Compare(str, currentVer) > 0)
                    {
                        webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/"+ str2);

                        webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                        strm = webres.GetResponseStream();

                        fs = new System.IO.FileStream("sound/csr/" + System.Text.RegularExpressions.Regex.Replace("csr_list.txt", @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                        readData = new byte[1024];
                        for (; ; )
                        {
                            int readSize = strm.Read(readData, 0, readData.Length);
                            if (readSize == 0)
                            {
                                break;
                            }
                            fs.Write(readData, 0, readSize);
                        }

                        fs.Close();
                        strm.Close();
                    }
                }
                else
                {
                    System.IO.File.Delete("Cookiesound(kari).old");
                }
                mre.Set();
            }
            catch (System.Net.HttpListenerException)
            {
            }
            catch (System.Net.WebException)
            {
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (System.Exception)
            {
            }
            finally
            {
            }
        }
    }
}
