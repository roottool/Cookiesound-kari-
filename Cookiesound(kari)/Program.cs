using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IrrKlang;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

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
                Application.Run(new Cookiesound());
            }
            catch (System.Threading.ThreadAbortException)
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
            // ミューテックス生成
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, Application.ProductName))
            {
                if (Environment.CommandLine.IndexOf("/up", StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    try
                    {
                        string[] args2 = Environment.GetCommandLineArgs();
                        int pid = Convert.ToInt32(args2[2]);
                        System.Diagnostics.Process.GetProcessById(pid).WaitForExit();    // 終了待ち
                        System.IO.File.Delete("Cookiesound(kari).old");
                    }
                    catch (Exception)
                    {
                    }
                }
                // 二重起動を禁止する
                if (mutex.WaitOne(0, false))
                {
                    ls.Start();
                    Thread checking = new Thread(new ThreadStart(uc.Run));
                    checking.Start();
                    uc.mre.WaitOne();
                    checking.Join();
                    if (System.IO.File.Exists("Cookiesound(kari).old"))
                    {
                        System.Diagnostics.Process.Start("Cookiesound(kari).exe", "/up " + System.Diagnostics.Process.GetCurrentProcess().Id);
                        //Application.Restart();
                        //mutex.ReleaseMutex();
                        //System.IO.File.Delete("Cookiesound(kari).old");
                        Environment.Exit(0);
                    }
                    GetAllFiles(@"./sound", "*.ogg", ref files);
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
                else
                {
                    MessageBox.Show("二重起動は禁止されています。");
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
                    string[] fs = System.IO.Directory.GetFiles(folder.ToLower(), searchPattern);
                    for (int i = 0; i < fs.Length; i++) 
                    {
                        if (System.Text.RegularExpressions.Regex.Match(fs[i], @"OGG$").Success)
                        {
                            fs[i] = System.Text.RegularExpressions.Regex.Replace(fs[i], "OGG", "ogg");
                        }
                        fs[i] = reg.Replace(fs[i].ToLower(), ""); 
                    }
                    files.AddRange(fs);

                    Regex reg2 = new Regex("^./sound\\\\csr\\\\|.mp3$");
                    //folderのサブフォルダを取得する
                    string[] ds = System.IO.Directory.GetDirectories(folder);

                    //サブフォルダにあるファイルも調べる
                    foreach (string d in ds)
                    {
                        if (d.Equals("./sound\\csr"))
                        {
                            string[] dfs = System.IO.Directory.GetFiles(d, "*.mp3");
                            for (int i = 0; i < dfs.Length; i++) { dfs[i] = reg2.Replace(dfs[i], ""); }
                            files.AddRange(dfs);
                        }
                    }
                    files.Sort();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Cookiesound(kari).exeと同じフォルダにsoundフォルダが存在しません。");
                Environment.Exit(0);
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
