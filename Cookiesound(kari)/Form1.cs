using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IrrKlang;
using System.IO;
using System.Threading;
using HongliangSoft.Utilities.Gui;
using Meebey.SmartIrc4net;

namespace Cookiesound_kari_
{
    using IrcConnection = Irc.IrcBot;
    

    public partial class Form1 : Form
    {
        public static ArrayList names_array = new ArrayList();
        public static string[] names_list;
        int count = 0;
        Boolean hook = false;
        Boolean mute = false;
        Boolean ignore = false;
        Irc.Irc_server irc_server = Ini.Read<Irc.Irc_server>("Irc_server", "config.ini");
        Irc_server irc = Ini.Read<Irc_server>("Irc_server", "config.ini");
        Hook h = Ini.Read<Hook>("Hook", "config.ini");
        Other other = Ini.Read<Other>("Other", "config.ini");
        Irc.resive res;

        private readonly dynamic _wmp = Activator.CreateInstance(Type.GetTypeFromProgID("WMPlayer.OCX.7"));
        private Boolean player_stats = false;

        //Invokeを呼び出すためのdelegate作成
        private delegate void DelSetLabelText(string str);

        public Form1()
        {
            h.hookkey = h.hookkey.ToUpper();
            InitializeComponent();
        }
        //Form1オブジェクトを保持するためのフィールド
        private static Form1 _form1Instance;

        //Form1オブジェクトを取得、設定するためのプロパティ
        public static Form1 Form1Instance
        {
            get
            {
                return _form1Instance;
            }
            set
            {
                _form1Instance = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label1.Text.ToLower() == "stop")
            {
                Program.engine.StopAllSounds();
                if (player_stats)
                {
                    _wmp.controls.stop();
                    player_stats = false;
                }
            }
            else if (label1.Text.ToLower() == "mute")
            {
                if (mute == false)
                {
                    mute = true;
                    Program.engine.SoundVolume = 0;
                    if (player_stats)
                    {
                        _wmp.controls.stop();
                        player_stats = false;
                    }
                }
                else
                {
                    mute = false;
                    Program.engine.SoundVolume = (float)((float)other.volume / (float)100);

                }
            }
            else if (label1.Text.ToLower() == "list")
            {
                IrcConnection.irc.RfcList(irc.channel);
            }
            else if (label1.Text.ToLower() == "ignore")
            {
                IrcConnection.irc.RfcNames(irc.channel);
                mute = true;
                Program.engine.SoundVolume = 0;
                if (player_stats)
                {
                    _wmp.controls.stop();
                    player_stats = false;
                }
                ignore = true;
                textBox1.Focus();
            }
            else if (BinarySearch(textBox1.Text) != -1)
            {
                Soundplay(label1.Text.ToLower(), irc.nickname);
            }
            textBox1.Text = label1.Text = "";
            if (ignore && irc.nickname != names_list[count])
                label1.Text += names_list[count];
            hook = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!ignore && hook == true && (e.KeyChar == (char)Keys.Enter))
            {
                //EnterやEscapeキーでビープ音が鳴らないようにする
                e.Handled = true;
                textBox1.Text = label1.Text = "";
                hook = false;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                hook = false;
                textBox1.Text = label1.Text = "";
                if (ignore && irc.nickname != names_list[count])
                    label1.Text += names_list[count];
            }
        }
        
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void keyboardHook1_KeyboardHooked_1(object sender, KeyboardHookedEventArgs e)
        {
            if (!ignore && e.UpDown == KeyboardUpDown.Down)
            {
                if (hook == false && (char)e.KeyCode == h.hookkey[0])//Keys.J)
                {
                    hook = true;
                    if (h.hooksound == "" || mute == true)
                    {
                        System.Media.SystemSounds.Beep.Play();
                    }
                    else
                    {
                        if (BinarySearch(h.hooksound) != -1)
                        {
                            try
                            {
                                Program.engine.Play2D("./sound/" + h.hooksound + ".ogg");
                            }
                            catch (System.IO.FileNotFoundException)
                            {
                                return;
                            }
                            textBox1.Text = label1.Text = "";
                        }
                        else
                        {
                            System.Media.SystemSounds.Beep.Play();
                        }
                    }
                    textBox1.Text = label1.Text = "";
                }
                else if (hook == true && e.KeyCode == Keys.Enter)
                {
                    if (label1.Text.ToLower() == "stop")
                    {
                        Program.engine.StopAllSounds();
                        if (player_stats)
                        {
                            _wmp.controls.stop();
                            player_stats = false;
                        }
                    }
                    else if (label1.Text.ToLower() == "mute")
                    {
                        if (mute == false)
                        {
                            mute = true;
                            Program.engine.SoundVolume = 0;
                            if (player_stats)
                            {
                                _wmp.controls.stop();
                                player_stats = false;
                            }
                        }
                        else
                        {
                            mute = false;
                            Program.engine.SoundVolume = (float)((float)other.volume / (float)100);

                        }
                    }
                    else if (label1.Text.ToLower() == "list")
                    {
                        IrcConnection.irc.RfcList(irc.channel);
                    }
                    else if (label1.Text.ToLower() == "ignore")
                    {
                        IrcConnection.irc.RfcNames(irc.channel);
                        mute = true;
                        Program.engine.SoundVolume = 0;
                        if (player_stats)
                        {
                            _wmp.controls.stop();
                            player_stats = false;
                        }
                        ignore = true;
                        textBox1.Focus();
                    }
                    else if (BinarySearch(label1.Text.ToLower()) != -1)
                    {
                        Soundplay(label1.Text.ToLower(), irc.nickname);
                    }
                    textBox1.Text = label1.Text = "";
                    hook = false;
                }
                else if (hook == true && Keys.A <= e.KeyCode && e.KeyCode <= Keys.Z)
                {
                    label1.Text += e.KeyCode;
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
                else if (hook == true && e.KeyCode == Keys.Oem7)
                {
                    label1.Text += "^";
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
                else if (hook == true && e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
                {
                    label1.Text += "-";
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
                else if (hook == true && Keys.D0 <= e.KeyCode && e.KeyCode <= Keys.D9)
                {
                    label1.Text += e.KeyCode;
                    label1.Text = label1.Text.Remove(label1.Text.Length - 2, 1);
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
                else if (hook == true && Keys.NumPad0 <= e.KeyCode && e.KeyCode <= Keys.NumPad9)
                {
                    label1.Text += e.KeyCode;
                    label1.Text = label1.Text.Remove(label1.Text.Length - 7, 6);
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
                else if (hook == true && label1.Text.Length > 0 && e.KeyCode == Keys.Back)
                {
                    label1.Text = label1.Text.Remove(label1.Text.Length - 1, 1);
                    //textBox1.Select(textBox1.Text.Length, 0);
                }
            }
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (ignore)
            {
                if (e.KeyCode == Keys.Up && count != 0)
                {
                    count--;
                    label1.Text = ""; 
                    if (irc.nickname != names_list[count])
                        label1.Text += names_list[count];
                }
                else if (e.KeyCode == Keys.Down && count < names_array.Count - 1)
                {
                    count++;
                    label1.Text = "";
                    if (irc.nickname != names_list[count])
                        label1.Text += names_list[count];
                }
                else if (e.KeyCode == Keys.Left)
                {
                    label1.Text = "";
                    ignore = false;
                    mute = false;
                    Program.engine.SoundVolume = (float)((float)other.volume / (float)100);
                    count = 0;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    if (label1.Text != "")
                        Program.ignores.Add(names_list[count]);
                    label1.Text = "";
                    ignore = false;
                    mute = false;
                    Program.engine.SoundVolume = (float)((float)other.volume / (float)100);
                    count = 0;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1.Form1Instance = this;
            res = Program.res;
            Program.engine.SoundVolume = (float)((float)other.volume / (float)100);
            _wmp.settings.volume = other.volume;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (player_stats)
            {
                _wmp.controls.stop();
                player_stats = false;
            }
            // Close all streams
            IrcConnection.irc.Disconnect();
            res.Abort();
        }

        public void Soundplay(string filename, string nickname)
        {
            Boolean hit = false;
            for (int i = 0; i < Program.ignores.Count; i++) 
            {
                if (nickname.IndexOf((string)Program.ignores[i] ,0) != -1)
                {
                    hit = true; break;
                }
            }
            // play a sound file
            if(hit == false)
            {
                try
                {
                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"^csr[0-9]+");
                    System.Text.RegularExpressions.Match m = r.Match(filename);
                    if (m.Success && other.bgm.ToLower() == "true" && !mute) 
                    {
                        if (player_stats)
                            _wmp.controls.stop();
                        _wmp.URL = "./sound/csr/" + filename + ".mp3";
                        _wmp.controls.play();
                        player_stats = true;
                    }
                    else if (!m.Success)
                        Program.engine.Play2D("./sound/" + filename + ".ogg");
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
                catch (System.UriFormatException)
                {
                    return;
                }
                catch (System.Exception)
                {
                    return;
                }
            }
            if (irc.nickname == nickname && mute == false)
            {
                IrcConnection.irc.SendMessage(SendType.Message, irc_server.channel, filename);
                label2.Text += irc.nickname + " : " + filename + Environment.NewLine;
                panel1.AutoScrollPosition = new Point(0, -this.AutoScrollPosition.Y + label2.Height);
            }
        }

        public void SetLabelText(string str)
        {
            //Invokeが必要な場合(プログラムが勝手に判断してくれる)
            if (this.label2.InvokeRequired)
            {
                //デリゲートからInvoke呼び出し
                DelSetLabelText d = new DelSetLabelText(SetLabelText);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                this.label2.Text += str;
            }
        }

        public int BinarySearch(string input)
        {
            int head = 0; int half = 0;
            int tail = Program.files.Count - 1;
            while (head <= tail)
            {
                half = (head + tail) / 2;
                int result = string.Compare(input, ""+Program.files[half]);
                if (result == 0) { return 0; }
                else if (result == 1) { head = half + 1; }
                else if (result == -1) { tail = half - 1; }
            }
            return -1;
        }
    }

    public class Irc_server
    {
        public string address;
        public int port;
        public string channel;
        public string nickname;
    }
    public class Hook
    {
        public string hookkey;
        public string hooksound;
    }
    public class Other
    {
        public int volume;
        public string bgm;
    }
}
