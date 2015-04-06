using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigEditor
{

    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
                if (System.IO.File.Exists(@"./config.ini"))
                {
                    try
                    {
                        string bgm = string.Empty;
                        System.IO.StreamReader cReader = (new System.IO.StreamReader(@"./config.ini", System.Text.Encoding.Default));
                        string stResult = string.Empty;
                        while (cReader.Peek() >= 0)
                        {
                            string stBuffer = cReader.ReadLine();
                            if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, "^address="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^address=", "");
                                textBox1.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^port="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^port=", "");
                                int port = int.Parse(stBuffer);
                                textBox2.Text = port.ToString();
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^channel="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^channel=", "");
                                textBox3.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^nickname="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^nickname=", "");
                                textBox4.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^hookkey="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^hookkey=", "");
                                textBox5.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^hooksound="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^hooksound=", "");
                                textBox6.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^volume="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^volume=", "");
                                int volume = int.Parse(stBuffer);
                                trackBar1.Value = volume;
                                label8.Text = stBuffer;
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(stBuffer, @"^bgm="))
                            {
                                stBuffer = System.Text.RegularExpressions.Regex.Replace(stBuffer, @"^bgm=", "");
                                bgm = stBuffer;
                                if (bgm.ToLower() == "true")
                                    checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
                                else
                                    checkBox1.CheckState = System.Windows.Forms.CheckState.Unchecked;
                            }
                        }
                        cReader.Close();
                    }
                    catch (System.IO.FileNotFoundException ex)
                    {
                        return;
                    }
                }
                else
                {
                    int port = (int)Properties.Settings.Default["port"];
                    Properties.Settings.Default.Reload();
                    textBox1.Text = (string)Properties.Settings.Default["address"];
                    textBox2.Text = port.ToString();
                    textBox3.Text = (string)Properties.Settings.Default["channel"];
                    textBox4.Text = (string)Properties.Settings.Default["nickname"];
                    textBox5.Text = (string)Properties.Settings.Default["hookkey"];
                    textBox6.Text = (string)Properties.Settings.Default["hooksound"];
                    trackBar1.Value = (int)Properties.Settings.Default["volume"];
                    int volume = (int)Properties.Settings.Default["volume"];
                    label8.Text = volume.ToString();
                    string bgm = (string)Properties.Settings.Default["bgm"];
                    if (bgm.ToLower() == "true")
                        checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
                    else
                        checkBox1.CheckState = System.Windows.Forms.CheckState.Unchecked;
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["address"] = textBox1.Text;
            Properties.Settings.Default["port"] = int.Parse(textBox2.Text);
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox3.Text, @"^#") == false)
            { textBox3.Text = "#" + textBox3.Text; }
            Properties.Settings.Default["channel"] = textBox3.Text;
            Properties.Settings.Default["nickname"] = textBox4.Text;
            Properties.Settings.Default["hookkey"] = textBox5.Text;
            Properties.Settings.Default["hooksound"] = textBox6.Text;
            Properties.Settings.Default["volume"] = trackBar1.Value;
            if (checkBox1.Checked)
               Properties.Settings.Default["bgm"]= "true";
            else
                Properties.Settings.Default["bgm"] = "false";
            Properties.Settings.Default.Save();
            var irc = new Irc_server();
            irc.address = (string)Properties.Settings.Default["address"];
            irc.port = (int)Properties.Settings.Default["port"];
            irc.channel = (string)Properties.Settings.Default["channel"];
            irc.nickname = (string)Properties.Settings.Default["nickname"];
            var hook = new Hook();
            hook.hookkey = (string)Properties.Settings.Default["hookkey"];
            hook.hooksound = (string)Properties.Settings.Default["hooksound"];
            var other = new Other();
            other.volume = (int)Properties.Settings.Default["volume"];
            other.bgm = (string)Properties.Settings.Default["bgm"];
            Ini.Write("Irc_server", irc, "./config.ini");
            Ini.Write("Hook", hook, "./config.ini");
            Ini.Write("Other", other, "./config.ini");
            System.Media.SystemSounds.Beep.Play();
            System.Windows.Forms.MessageBox.Show("保存完了しました");
            this.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label8.Text = String.Concat(trackBar1.Value.ToString());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, @"[0-9]$") == false)
            {
                textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1, 1);
                textBox2.Select(textBox2.Text.Length, 0);
            }
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
