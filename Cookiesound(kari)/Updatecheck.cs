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
        public string currentVer;
        public Boolean dlcomplete;
        public Boolean noupdate;

        public Updatecheck()
        {
            mre = new ManualResetEvent(false);
            currentVer = "0.5.5";
            dlcomplete = false;
            noupdate = false;
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
            string str_sound;
            string str_csr;
            string str_csr_list;
            
            try
            {
                WebClient wc = new WebClient();
                //Stream st = wc.OpenRead("https://www.dropbox.com/sh/vxnv1zltyr7sais/AADhPLLkZbbH7tppr6DXRVHaa?dl=0"); //Debug
                Stream st = wc.OpenRead("https://www.dropbox.com/sh/wb47tpk36741rp7/AABEUAPlgnMO8onurQNOvCBta?dl=0"); //main
                StreamReader sr = new StreamReader(st, Encoding.GetEncoding(51932));

                s = sr.ReadToEnd();
                System.Text.RegularExpressions.Regex ver_r = new System.Text.RegularExpressions.Regex(@"[\d|.]+_Cookiesound");
                System.Text.RegularExpressions.Match ver_m = ver_r.Match(s);
                str = System.Text.RegularExpressions.Regex.Replace(ver_m.Value, "_Cookiesound", "");

                st.Close();
                wc.Dispose();

                DateTime rewirte_keyboardhook_time = new DateTime(2015, 11, 9, 0, 0, 0);

                if (String.Compare(str, currentVer) > 0 || str.Length > currentVer.Length || System.IO.File.GetLastWriteTime("KeyboardHooked.dll").CompareTo(rewirte_keyboardhook_time) < 0)
                {
                    MessageBox.Show("アップデートがあります。自動更新後に再起動を行います。");

                    if (System.IO.File.GetLastWriteTime("KeyboardHooked.dll").CompareTo(rewirte_keyboardhook_time) < 0)
                    {
                        System.Net.WebClient KeyboardHooked_dll_wc = new System.Net.WebClient();
                        System.IO.File.Delete("KeyboardHooked.old");
                        System.IO.File.Move("KeyboardHooked.dll", "KeyboardHooked.old");
                        KeyboardHooked_dll_wc.DownloadFile("https://dl.dropboxusercontent.com/u/37080107/KeyboardHooked.dll", "KeyboardHooked.dll");
                        KeyboardHooked_dll_wc.Dispose();
                    }

                    System.Net.HttpWebRequest webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/readme.txt");

                    System.Net.HttpWebResponse webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                    System.IO.Stream strm = webres.GetResponseStream();

                    System.IO.FileStream fs = new System.IO.FileStream("readme.txt", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    byte[] readData = new byte[1024];
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
                            str_sound = System.Text.RegularExpressions.Regex.Replace("" + addsounds[i], @"_[\w-|^]+.ogg", "");
                            if (String.Compare(str_sound, currentVer) > 0 || str_sound.Length > currentVer.Length)
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
                            str_csr = System.Text.RegularExpressions.Regex.Replace("" + addcsrs[i], @"_[\w|-|^]+.mp3", "");
                            if (String.Compare(str_csr, currentVer) > 0 || str_csr.Length > currentVer.Length)
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
                    str_csr_list = System.Text.RegularExpressions.Regex.Replace(m2.Value, "\"", "");
                    str_csr_list = System.Text.RegularExpressions.Regex.Replace(str_csr_list, @"_csr_list.txt", "");
                    if (String.Compare(str_csr_list, currentVer) > 0)
                    {
                        webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/"+ str_csr_list);

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

                    System.IO.File.Delete("Cookiesound(kari).old");
                    System.IO.File.Move("Cookiesound(kari).exe", "Cookiesound(kari).old");

                    /*
                    //WebRequestを作成 I used a Public folder in Dropbox.
                    webreq =
                        //(System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://www.dropbox.com/s/izgznijak4fj1f5/0.3.7_Cookiesound%28kari%29.exe?dl=0"); //Debug
                        (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + str + "_Cookiesound(kari).exe"); //main
                    */
                    System.Net.WebClient downloadClient = null;
                    Uri u = new Uri("https://dl.dropboxusercontent.com/u/37080107/" + str + "_Cookiesound(kari).exe");

                    //WebClientの作成
                    if (downloadClient == null)
                    {
                        downloadClient = new System.Net.WebClient();
                        //イベントハンドラの作成
                        downloadClient.DownloadFileCompleted +=
                            new System.ComponentModel.AsyncCompletedEventHandler(
                                downloadClient_DownloadFileCompleted);
                    }
                    //非同期ダウンロードを開始する
                    downloadClient.DownloadFileAsync(u, "Cookiesound(kari).exe");
                }
                else
                {
                    noupdate = true;
                }
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
        private void downloadClient_DownloadFileCompleted(object sender,
            System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("エラー:{0}", e.Error.Message);
                System.IO.File.Delete("Cookiesound(kari).exe");
                System.IO.File.Move("Cookiesound(kari).old", "Cookiesound(kari).exe");
            }
            else
            {
                dlcomplete = true;
            }
        }
    }
}
