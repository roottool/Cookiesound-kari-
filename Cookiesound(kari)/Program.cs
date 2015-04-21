using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IrrKlang;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

namespace Cookiesound_kari_
{
    using IrcConnection = Irc.IrcBot;

    class LoadScreen
    {
        private static Thread loading;

        public LoadScreen ()
        {
                loading = new Thread(new ThreadStart(this.Run));
        }

	    // Starts the thread
	    public void Start () 
	    {
                loading.Start(); 
	    }
        public void Run()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2());
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            finally
            {
            }
        }
    }

    class Updatecheck
    {
        private static Thread checking;
        public ManualResetEvent mre;

        public Updatecheck()
        {
            checking = new Thread(new ThreadStart(this.Run));
            mre = new ManualResetEvent(false);
        }

        // Starts the thread
        public void Start()
        {
            checking.Start();
        }
        public void Run()
        {
            string s;
            try
            {
                mre.Reset();
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead("https://www.dropbox.com/sh/wb47tpk36741rp7/AABEUAPlgnMO8onurQNOvCBta?dl=0");
                StreamReader sr = new StreamReader(st, Encoding.GetEncoding(51932));

                s = sr.ReadToEnd();
                System.Text.RegularExpressions.Regex ver_r = new System.Text.RegularExpressions.Regex(@"[\d|.]+_Cookiesound");
                System.Text.RegularExpressions.Match ver_m = ver_r.Match(s);
                s = System.Text.RegularExpressions.Regex.Replace(ver_m.Value, "_Cookiesound", "");

                st.Close();
                wc.Dispose();

                if (s != "0.2.0")
                {
                    MessageBox.Show("アップデートがあります。自動更新後に発生するoldファイルは次回起動時に\r\n削除されるのでそのままでお願いします。");
                    //WebRequestを作成 I used a Public folder in Dropbox.
                    System.Net.HttpWebRequest webreq = 
                        (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dl.dropboxusercontent.com/u/37080107/" + s + "_Cookiesound(kari).exe");

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

    class Program
    {
        public static ArrayList files;
        public static ArrayList ignores;
        // start the sound engine with default parameters
        public static ISoundEngine engine;
        public static Irc.resive res;
        public static LoadScreen ls;
        public static Updatecheck uc;
        public static Form1 f;

        static Program()
        {
            files = new ArrayList();
            ignores = new ArrayList();
            // start the sound engine with default parameters
            engine = new ISoundEngine();
            res = new Irc.resive();
            ls = new LoadScreen();
            uc = new Updatecheck();
            f = new Form1();
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            GetAllFiles(@"./sound", "*.ogg", ref files);
            // ミューテックス生成
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, Application.ProductName))
            {
                // 二重起動を禁止する
                if (mutex.WaitOne(0, false))
                {
                    ls.Start();
                    uc.Start();
                    uc.mre.WaitOne();
                    IrcConnection.Connection(args);
                    Program.res.Start();
                    while (Irc.IrcBot.irc.IsRegistered == false) { }
                    Application.Exit();
                    Thread.Sleep(1000);
                    Form1.Form1Instance = f;

                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(f);
                    //Application.Run(new Form1());

                    //string stCurrentDir = System.IO.Directory.GetCurrentDirectory();
                    //System.Windows.Forms.MessageBox.Show(stCurrentDir);
                }
            }
        }

        public static void GetAllFiles(string folder, string searchPattern, ref ArrayList files)
        {
            Regex reg = new Regex("^./sound\\\\|.ogg$");
            if (System.IO.Directory.Exists(folder))
            {
                try
                {
                    string[] fs = System.IO.Directory.GetFiles(folder, searchPattern);
                    for (int i = 0; i < fs.Length; i++) { fs[i] = reg.Replace(fs[i], ""); }
                    files.AddRange(fs);

                    Regex reg2 = new Regex("^./sound\\\\csr\\\\|.mp3$");
                    //folderのサブフォルダを取得する
                    //string[] ds = System.IO.Directory.GetDirectories(folder);
                    string[] ds = System.IO.Directory.GetDirectories(folder);

                    //サブフォルダにあるファイルも調べる
                    foreach (string d in ds)
                    {
                        if (d.Equals("./sound\\csr"))
                        {
                            string[] dfs = System.IO.Directory.GetFiles(d, "*.mp3");
                            for (int i = 0; i < dfs.Length; i++) { dfs[i] = reg2.Replace(dfs[i], ""); }
                            files.AddRange(dfs);
                            //string dd = reg2.Replace(d, "");
                            //GetAllFiles(dd, "*.wav", ref files);
                        }
                    }
                    //GetAllFiles(d, searchPattern, ref files);
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
            }
            if (System.IO.File.Exists(@"./mute.txt"))
            {
                try
                {
                    System.IO.StreamReader cReader = (new System.IO.StreamReader(@"./mute.txt", System.Text.Encoding.Default));
                    string stResult = string.Empty;
                    while (cReader.Peek() >= 0)
                    {
                        string stBuffer = cReader.ReadLine();
                        if (Program.files.IndexOf(stBuffer, 0) != -1)
                        {
                            files.RemoveAt(files.IndexOf(stBuffer, 0));
                        }
                    }
                    cReader.Close();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
            }
            if (System.IO.File.Exists(@"./ignore.txt"))
            {
                try
                {
                    System.IO.StreamReader cReader = (new System.IO.StreamReader(@"./ignore.txt", System.Text.Encoding.Default));
                    string stResult = string.Empty;
                    while (cReader.Peek() >= 0)
                    {
                        string stBuffer = cReader.ReadLine();
                        ignores.Add(stBuffer);
                    }
                    cReader.Close();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
            }
        }
    }
}
