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
            string currentVer = "0.3.0";
            try
            {
                mre.Reset();
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead("https://www.dropbox.com/sh/wb47tpk36741rp7/AABEUAPlgnMO8onurQNOvCBta?dl=0");
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
                        (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + str + "_Cookiesound(kari).exe");

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

                    System.Text.RegularExpressions.MatchCollection mc =
                        System.Text.RegularExpressions.Regex.Matches(s, "\"" + @"[\d|.]+_[\w|-|^]+.ogg");

                    ArrayList addsounds = new ArrayList();
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        //正規表現に一致したグループを表示
                        //Console.WriteLine("URL:{0}", m.Groups[0].Value);
                        addsounds.Add(System.Text.RegularExpressions.Regex.Replace(m.Groups[0].Value, "\"", ""));
                    }
                    for (int i = 0; i < addsounds.Count; i++)
                    {
                        Console.WriteLine("RESULT:{0}", addsounds[i]);
                        str = System.Text.RegularExpressions.Regex.Replace("" + addsounds[i], @"_[\w|-|^]+.ogg", "");
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

                    System.Text.RegularExpressions.MatchCollection mc2 =
                        System.Text.RegularExpressions.Regex.Matches(s, "\"" + @"[\d|.]+_[\w|-|^]+.mp3");

                    ArrayList addcsrs = new ArrayList();
                    foreach (System.Text.RegularExpressions.Match m in mc2)
                    {
                        addcsrs.Add(System.Text.RegularExpressions.Regex.Replace(m.Groups[0].Value, "\"", ""));
                    }
                    for (int i = 0; i < addcsrs.Count; i++)
                    {
                        Console.WriteLine("RESULT:{0}", addcsrs[i]);
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
