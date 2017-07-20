using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;

namespace Cookiesound_kari_
{
    class Updatecheck
    {
        public ManualResetEvent mre; 
        public string currentVer;
        public int currentmajorVer;
        public int currentminorVer;
        public int currentbuild_number;
        public string currentminorVer_tmpstr;
        public Boolean dlcomplete;
        public Boolean noupdate;
        private string s;
        private string str;
        private int str_major;
        private int str_minor;
        private int str_build_number;
        private string str_minor_tmpstr;
        private string str_sound;
        private string str_csr;
        private string str_csr_list;
        private string str_cfg;
        private MatchCollection mc;
        private List<string> addsounds;
        private MatchCollection mc2;
        private List<string> addcsrs;
        System.Net.WebClient downloadClient;
        Uri u;
        System.Net.HttpWebRequest webreq;
        HttpWebResponse webres;
        Stream strm;
        System.IO.FileStream fs;
        byte[] readData;
        Match m2;
        Match m3;

        public Updatecheck()
        {
            mre = new ManualResetEvent(false);
            currentVer = Application.ProductVersion;
            currentmajorVer = int.Parse(Regex.Replace(currentVer, @".[\d]+.[\d]+$", ""));
            currentminorVer_tmpstr = Regex.Replace(currentVer, @"^[\d]+.", "");
            currentminorVer = int.Parse(Regex.Replace(currentminorVer_tmpstr, @".[\d]+$", ""));
            currentbuild_number = int.Parse(Regex.Replace(currentVer, @"^[\d]+.[\d].", ""));
            dlcomplete = false;
            noupdate = false;
        }

        public async Task UpdatecheckAsync()
        {
            try
            {
                WebClient wc = new WebClient();
                //Stream st = wc.OpenRead("https://www.dropbox.com/sh/vxnv1zltyr7sais/AADhPLLkZbbH7tppr6DXRVHaa?dl=0"); //bkDebug
                //Stream st = wc.OpenRead("https://www.dropbox.com/sh/wb47tpk36741rp7/AABEUAPlgnMO8onurQNOvCBta?dl=0"); //Debug
                Stream st = wc.OpenRead("https://www.dropbox.com/sh/ryqu51l1y8mokwo/AAAmZm3p6FyGei7qatiO1xTLa?dl=0");//main
                StreamReader sr = new StreamReader(st, Encoding.GetEncoding(51932));

                s = sr.ReadToEnd();
                Regex ver_r = new Regex(@"[\d|.]+_Cookiesound");
                Match ver_m = ver_r.Match(s);
                str = Regex.Replace(ver_m.Value, "_Cookiesound", "");
                str_major = int.Parse(Regex.Replace(str, @".[\d]+.[\d]+$", ""));
                str_minor_tmpstr = Regex.Replace(str, @"^[\d]+.", "");
                str_minor = int.Parse(Regex.Replace(str_minor_tmpstr, @".[\d]+$", ""));
                str_build_number = int.Parse(Regex.Replace(str, @"^[\d]+.[\d]+.", ""));

                st.Close();
                wc.Dispose();

                DateTime rewirte_keyboardhook_time = new DateTime(2015, 11, 9, 0, 0, 0);
                DateTime rewirte_ConfigEditor_time = new DateTime(2017, 7, 17, 0, 0, 0);

                if (File.GetLastWriteTime("KeyboardHooked.dll").CompareTo(rewirte_keyboardhook_time) < 0
                    || File.GetLastWriteTime("ConfigEditor.exe").CompareTo(rewirte_ConfigEditor_time) < 0
                    || currentmajorVer < str_major
                    || (currentmajorVer >= str_major && currentminorVer < str_minor)
                    || (currentmajorVer >= str_major && currentminorVer >= str_minor && currentbuild_number < str_build_number))
                {
                    MessageBox.Show("アップデートがあります。自動更新後に再起動を行います。");

                    if (File.GetLastWriteTime("KeyboardHooked.dll").CompareTo(rewirte_keyboardhook_time) < 0)
                    {
                        System.Net.WebClient KeyboardHooked_dll_wc = new System.Net.WebClient();
                        File.Delete("KeyboardHooked.old");
                        File.Move("KeyboardHooked.dll", "KeyboardHooked.old");
                        KeyboardHooked_dll_wc.DownloadFile("https://cookiesound-4de19.firebaseapp.com/KeyboardHooked.dll", "KeyboardHooked.dll");
                        KeyboardHooked_dll_wc.Dispose();
                    }

                    //ConfigEditorの更新チェックと更新
                    m3 = Regex.Match(s, "\"" + @"[\d|.]+_ConfigEditor.exe");
                    str_cfg = Regex.Replace(m3.Value, "\"", "");
                    str_cfg = Regex.Replace(str_cfg, @"_ConfigEditor.exe", "");
                    str_major = int.Parse(Regex.Replace(str_cfg, @".[\d]+.[\d]+$", ""));
                    str_minor_tmpstr = Regex.Replace(str_cfg, @"^[\d]+.", "");
                    str_minor = int.Parse(Regex.Replace(str_minor_tmpstr, @".[\d]+$", ""));
                    str_build_number = int.Parse(Regex.Replace(str_cfg, @"^[\d]+.[\d]+.", ""));
                    if (currentmajorVer < str_major || File.GetLastWriteTime("ConfigEditor.exe").CompareTo(rewirte_ConfigEditor_time) < 0
                        || (currentmajorVer >= str_major && currentminorVer < str_minor)
                        || (currentmajorVer >= str_major && currentminorVer >= str_minor && currentbuild_number < str_build_number))
                    {
                        downloadClient = null;
                        u = new Uri("https://cookiesound-4de19.firebaseapp.com/" + str_cfg + "_ConfigEditor.exe");

                        //WebClientの作成
                        if (downloadClient == null)
                        {
                            downloadClient = new System.Net.WebClient();
                            //イベントハンドラの作成
                            downloadClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);
                        }
                        //非同期ダウンロードを開始する
                        downloadClient.DownloadFileAsync(u, "ConfigEditor.exe");
                    }

                    //readmeの更新
                    Updatereadme();

                    await Task.Run(() =>
                    {
                        if (System.IO.Directory.Exists("sound"))
                        {
                            mc = Regex.Matches(s, "\"filename\": \"" + @"[\d|.]+_[\w-|^]+.ogg");
                            addsounds = new List<string>();
                            foreach (Match m in mc)
                            {
                                str_sound = Regex.Replace(m.Groups[0].Value, "\"filename\": \"", "");
                                str_sound = Regex.Replace(str_sound, @"_[\w-|^]+.ogg", "");
                                str_sound = Regex.Replace(str_sound, "\"", "");
                                str_major = int.Parse(Regex.Replace(str_sound, @".[\d]+.[\d]+$", ""));
                                str_minor_tmpstr = Regex.Replace(str_sound, @"^[\d]+.", "");
                                str_minor = int.Parse(Regex.Replace(str_minor_tmpstr, @".[\d]+$", ""));
                                str_build_number = int.Parse(Regex.Replace(str_sound, @"^[\d]+.[\d]+.", ""));
                                if (currentmajorVer < str_major
                                    || (currentmajorVer >= str_major && currentminorVer < str_minor)
                                    || (currentmajorVer >= str_major && currentminorVer >= str_minor && currentbuild_number < str_build_number))
                                {
                                    addsounds.Add(Regex.Replace(m.Groups[0].Value, "\"filename\": \"", ""));
                                }
                            }
                            for (int i = 0; i < addsounds.Count; i++)
                            {
                                webreq = (HttpWebRequest)System.Net.WebRequest.Create("https://cookiesound-4de19.firebaseapp.com/" + addsounds[i]);
                                webres = (HttpWebResponse)webreq.GetResponse();
                                strm = webres.GetResponseStream();

                                fs = new System.IO.FileStream("sound/" + Regex.Replace("" + addsounds[i], @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                readData = new byte[1024];
                                for (;;)
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
                            MessageBox.Show("Cookiesound(kari).exeと同じフォルダにsoundフォルダが存在しません。");
                            Environment.Exit(0);
                        }

                        if (System.IO.Directory.Exists("sound/csr"))
                        {
                            mc2 = Regex.Matches(s, "\"" + @"[\d|.]+_[\w|-|^]+.mp3");
                            addcsrs = new List<string>();
                            foreach (Match m in mc2)
                            {
                                str_csr = Regex.Replace("" + m.Groups[0].Value, "\"filename\": \"", "");
                                str_csr = Regex.Replace("" + str_csr, @"_[\w|-|^]+.mp3", "");
                                str_csr = Regex.Replace(str_csr, "\"", "");
                                str_major = int.Parse(Regex.Replace(str_csr, @".[\d]+.[\d]+$", ""));
                                str_minor_tmpstr = Regex.Replace(str_csr, @"^[\d]+.", "");
                                str_minor = int.Parse(Regex.Replace(str_minor_tmpstr, @".[\d]+$", ""));
                                str_build_number = int.Parse(Regex.Replace(str_csr, @"^[\d]+.[\d]+.", ""));
                                if (currentmajorVer < str_major
                                    || (currentmajorVer >= str_major && currentminorVer < str_minor)
                                    || (currentmajorVer >= str_major && currentminorVer >= str_minor && currentbuild_number < str_build_number))
                                {
                                    addcsrs.Add(Regex.Replace(m.Groups[0].Value, "\"filename\": \"", ""));
                                }
                            }
                            for (int i = 0; i < addcsrs.Count; i++)
                            {
                                webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://cookiesound-4de19.firebaseapp.com/" + addcsrs[i]);
                                webres = (HttpWebResponse)webreq.GetResponse();
                                strm = webres.GetResponseStream();

                                fs = new System.IO.FileStream("sound/csr/" + Regex.Replace("" + addcsrs[i], @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                readData = new byte[1024];
                                for (;;)
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
                            MessageBox.Show("soundフォルダ内にcsrフォルダが存在しません。");
                            Environment.Exit(0);

                        }

                        m2 = Regex.Match(s, "\"" + @"[\d|.]+_csr_list.txt");
                        str_csr_list = Regex.Replace(m2.Value, "\"", "");
                        str_csr_list = Regex.Replace(str_csr_list, @"_csr_list.txt", "");
                        str_major = int.Parse(Regex.Replace(str_csr_list, @".[\d]+.[\d]+$", ""));
                        str_minor_tmpstr = Regex.Replace(str_csr_list, @"^[\d]+.", "");
                        str_minor = int.Parse(Regex.Replace(str_minor_tmpstr, @".[\d]+$", ""));
                        str_build_number = int.Parse(Regex.Replace(str_csr_list, @"^[\d]+.[\d]+.", ""));
                        if (currentmajorVer < str_major
                            || (currentmajorVer >= str_major && currentminorVer < str_minor)
                            || (currentmajorVer >= str_major && currentminorVer >= str_minor && currentbuild_number < str_build_number))
                        {
                            webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://cookiesound-4de19.firebaseapp.com/" + str_csr_list);

                            webres = (HttpWebResponse)webreq.GetResponse();

                            strm = webres.GetResponseStream();

                            fs = new System.IO.FileStream("sound/csr/" + Regex.Replace("csr_list.txt", @"[\d|.]+_", ""), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            readData = new byte[1024];
                            for (;;)
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
                    }).ConfigureAwait(false);

                    //本体の更新
                    Checkcookiesound();

                    dlcomplete = true;
                }
                else
                {
                    dlcomplete = true;
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

        /// <summary>
        /// readmeの更新
        /// </summary>
        private void Updatereadme()
        {
            downloadClient = null;
            u = new Uri("https://cookiesound-4de19.firebaseapp.com/readme.txt");

            //WebClientの作成
            if (downloadClient == null)
            {
                downloadClient = new System.Net.WebClient();
                //イベントハンドラの作成
                downloadClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);
            }
            //非同期ダウンロードを開始する
            downloadClient.DownloadFileAsync(u, "readme.txt");
        }

            /// <summary>
            /// Cookie☆sound本体の更新チェックと更新
            /// </summary>
            private void Checkcookiesound()
        {
            //現在のファイルをoldファイルに変更
            File.Delete("Cookiesound(kari).old");
            File.Move("Cookiesound(kari).exe", "Cookiesound(kari).old");

            downloadClient = null;
            u = new Uri("https://cookiesound-4de19.firebaseapp.com/" + str + "_Cookiesound(kari).exe");

            //WebClientの作成
            if (downloadClient == null)
            {
                downloadClient = new System.Net.WebClient();
                //イベントハンドラの作成
                downloadClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);
            }
            //非同期ダウンロードを開始する
            downloadClient.DownloadFileAsync(u, "Cookiesound(kari).exe");
        }

        /// <summary>
        /// ダウンロード完了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadClient_DownloadFileCompleted(object sender,
            System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //エラー処理
            if (e.Error != null)
            {
                Console.WriteLine("エラー:{0}", e.Error.Message);

                //oldファイルに変更した現在のファイルをexeファイルに差し戻す
                if (File.Exists("ConfigEditor.old"))
                {
                    File.Delete("ConfigEditor.exe");
                    File.Move("ConfigEditor.old", "ConfigEditor.exe");
                }
                if (File.Exists("Cookiesound(kari).old"))
                {
                    File.Delete("Cookiesound(kari).exe");
                    File.Move("Cookiesound(kari).old", "Cookiesound(kari).exe");
                }
                dlcomplete = false;
            }
            else
            {
                dlcomplete = true;
            }
        }
    }
}
