using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
using IrrKlang;
using System.Text.RegularExpressions;
using System.IO;

namespace Cookiesound_kari_
{
    using IrcConnection = Irc.IrcBot;

    class Program
    {
        public static ArrayList files;
        public static ArrayList ignores;
        // start the sound engine with default parameters
        public static ISoundEngine engine;
        public static Irc.resive res;
        public static Updatecheck uc;
        public static Form1 f;

        static Program()
        {
            files = new ArrayList();
            ignores = new ArrayList();
            res = new Irc.resive();
            uc = new Updatecheck();
            f = new Form1();
            // start the sound engine with default parameters
            engine = new ISoundEngine();
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        /// 
        [STAThread]
        static void Main (string[] args)
        {
            // Register the IOleMessageFilter to handle any threading 
            // errors.
            MessageFilter.Register();

            // =====================================
            // ==Insert your automation code here.==
            // =====================================
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
                    }
                    catch (Exception)
                    {
                    }
                    //更新前の旧ファイルを削除
                    //File.Delete("Cookiesound(kari).old");
                    //File.Delete("KeyboardHooked.old");
                    //File.Delete("ConfigEditor.old");
                }
                // 二重起動を禁止する
                if (mutex.WaitOne(0, false))
                {
                    var currentContext = TaskScheduler.FromCurrentSynchronizationContext();
                    ShowloadscreenAsync();
                    Task[] tasks = {
                        uc.UpdatecheckAsync().ContinueWith((Task) => {
                            //更新前の旧ファイルを削除
                            File.Delete("Cookiesound(kari).old");
                            File.Delete("KeyboardHooked.old");
                            File.Delete("ConfigEditor.old");
                        }),
                        IrcConnection.ConnectionAync(args)
                    };
                    Task.WaitAll(tasks);
                    //Task.WaitAny(uc.UpdatecheckAsync());
                    if (uc.dlcomplete && !uc.noupdate)
                    {
                        Application.Exit();
                        if (IrcConnection.irc.IsConnected)
                        {
                            IrcConnection.irc.Disconnect();
                        }
                        System.Diagnostics.Process.Start("Cookiesound(kari).exe", "/up " + System.Diagnostics.Process.GetCurrentProcess().Id);
                        Environment.Exit(0);
                    }
                    GetAllFiles(@"./sound", "*.ogg", ref files);
                    //IrcConnection.Connection(args);
                    Program.res.Start();
                    while (Irc.IrcBot.irc.IsRegistered == false) { }
                    Application.Exit();
                    Form1.Form1Instance = f;
                    Application.Run(f);
                }
                else
                {
                    MessageBox.Show("二重起動は禁止されています。");
                }
                // =====================================

                // turn off the IOleMessageFilter.
                MessageFilter.Revoke();
            }
        }

        private static async void ShowloadscreenAsync()
        {
            await Task.Run(() =>
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
            });
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
            if (File.Exists(@"./mute.txt"))
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
            if (File.Exists(@"./ignore.txt"))
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
