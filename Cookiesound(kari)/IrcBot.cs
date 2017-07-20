using System;
using System.Threading.Tasks;
using System.Collections;
using Cookiesound_kari_;
using Meebey.SmartIrc4net;
using System.Windows.Forms;

namespace Irc
{
    // Empty constructor makes instance of Thread
	class resive
    {
        private Boolean checkpoint;
        
        public resive ()
        {
            checkpoint = false;
        }
        public async Task ListenMessageAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    IrcBot.irc.OnChannelMessage += new IrcEventHandler(OnChannelMessage);
                    // here we tell the IRC API to go into a receive mode, all events
                    // will be triggered by _this_ thread (main thread in this case)
                    // Listen() blocks by default, you can also use ListenOnce() if you
                    // need that does one IRC operation and then returns, so you need then
                    // an own loop
                    IrcBot.irc.Listen();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    return;
                }
                catch (System.FormatException)
                {
                    return;
                }
                finally
                {
                }
            });
        }

        private async void OnChannelMessage(object sender, IrcEventArgs e)
        {
            if (!checkpoint) checkpoint = true;
            else
            {
                Form1.Form1Instance.SetLabelText(e.Data.Nick.Remove(e.Data.Nick.Length - 1, 1) + " : " + e.Data.Message + Environment.NewLine);
                if (Program.files.IndexOf(e.Data.Message, 0) != -1)
                {
                    Form1.Form1Instance.Soundplay(e.Data.Message, e.Data.Nick.Remove(e.Data.Nick.Length - 1, 1));
                }
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
    public class Other
    {
        public int volume;
    }

    public class IrcBot
    {
        public static Boolean initialization_completed = false;
        // make an instance of the high-level API
        public static IrcClient irc = new IrcClient();
        // this method we will use to analyse queries (also known as private messages)
        public static void OnQueryMessage(object sender, IrcEventArgs e)
        {
            switch (e.Data.MessageArray[0])
            {
                // debug stuff
                case "dump_channel":
                    string requested_channel = e.Data.MessageArray[1];
                    // getting the channel (via channel sync feature)
                    Channel channel = irc.GetChannel(requested_channel);
                    // here we send messages
                    irc.SendMessage(SendType.Message, e.Data.Nick, "<channel '" + requested_channel + "'>");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Name: '" + channel.Name + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Topic: '" + channel.Topic + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Mode: '" + channel.Mode + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "Key: '" + channel.Key + "'");
                    irc.SendMessage(SendType.Message, e.Data.Nick, "UserLimit: '" + channel.UserLimit + "'");
                    // here we go through all users of the channel and show their
                    // hashtable key and nickname
                    string nickname_list = "";
                    nickname_list += "Users: ";
                    foreach (DictionaryEntry de in channel.Users)
                    {
                        string key = (string)de.Key;
                        ChannelUser channeluser = (ChannelUser)de.Value;
                        nickname_list += "(";
                        if (channeluser.IsOp)
                        {
                            nickname_list += "@";
                        }
                        if (channeluser.IsVoice)
                        {
                            nickname_list += "+";
                        }
                        nickname_list += ")" + key + " => " + channeluser.Nick + ", ";
                    }
                    irc.SendMessage(SendType.Message, e.Data.Nick, nickname_list);
                    irc.SendMessage(SendType.Message, e.Data.Nick, "</channel>");
                    break;
                case "gc":
                    GC.Collect();
                    break;
                // typical commands
                case "join":
                    irc.RfcJoin(e.Data.MessageArray[1]);
                    break;
                case "part":
                    irc.RfcPart(e.Data.MessageArray[1]);
                    break;
                case "die":
                    Exit();
                    break;
            }
        }
        // this method handles when we receive "ERROR" from the IRC server
        public static void OnError(object sender, ErrorEventArgs e)
        {
            System.Console.WriteLine("Error: " + e.ErrorMessage);
            Exit();
        }
        // this method will get all IRC messages
        public static void OnRawMessage(object sender, IrcEventArgs e)
        {
            string extraction = string.Empty;
            System.Console.WriteLine("Received: " + e.Data.RawMessage);
            System.Text.RegularExpressions.Regex list_r = new System.Text.RegularExpressions.Regex(@"#[\w-|~|^]+ [0-9]+ :$");
            System.Text.RegularExpressions.Match list_m = list_r.Match(e.Data.RawMessage);
            System.Text.RegularExpressions.Regex ignore_r = new System.Text.RegularExpressions.Regex(@" [=|@] #[\w-|~|^]+ :[@|\w]+");
            System.Text.RegularExpressions.Match ignore_m = ignore_r.Match(e.Data.RawMessage);
            System.Text.RegularExpressions.Regex already_r = new System.Text.RegularExpressions.Regex(@":Nickname is already in use.$");
            System.Text.RegularExpressions.Match already_m = already_r.Match(e.Data.RawMessage);
            if (list_m.Success)
            {
                string st = System.Text.RegularExpressions.Regex.Replace(e.Data.RawMessage, @" :$", "");
                st = System.Text.RegularExpressions.Regex.Replace(st, @"^:[\w-~|.~|^]+ [\d]+ [\w-|~|^]+ #[\w-|~|^]+ ", "");
                System.Windows.Forms.MessageBox.Show("The number of members is " + st + ".");
            }
            else if (ignore_m.Success)
            {
                //if (initialization_completed)
                //{
                string st = System.Text.RegularExpressions.Regex.Replace(e.Data.RawMessage, @"^:[\w-|.~|^]+ [\d]+ [\w-|~|^]+ [=|@] #[\w-|~|^]+ :", "");
                    st = System.Text.RegularExpressions.Regex.Replace(st, @"_ ", " ");
                    st = System.Text.RegularExpressions.Regex.Replace(st, @"^@", "");
                    System.Text.RegularExpressions.Regex extraction_r = new System.Text.RegularExpressions.Regex(@"^[\w-|~|^]+");
                    System.Text.RegularExpressions.Match extraction_m = extraction_r.Match(st);
                    Form1.names_array.Clear();
                    //while (st != string.Empty)
                    while (extraction_m.Success)
                    {
                        extraction = extraction_m.Value;
                        Form1.names_array.Add(extraction);
                        st = System.Text.RegularExpressions.Regex.Replace(st, @"^[\w-|~|^]+ ", "");
                        st = System.Text.RegularExpressions.Regex.Replace(st, @"^@", "");
                        extraction_m = extraction_r.Match(st);
                    }
                    Form1.names_list = (string[])Form1.names_array.ToArray(typeof(string));
                if (initialization_completed)
                {
                    Form1.Form1Instance.SetLabelText("「↑」または「↓」でニックネームを選択" + Environment.NewLine);
                    Form1.Form1Instance.SetLabelText("「→」で決定、「←」でキャンセル" + Environment.NewLine);
                    Form1.Form1Instance.SetLabelText("自ニックネーム選択時は空欄となり、" + Environment.NewLine);
                    Form1.Form1Instance.SetLabelText("「→」を押しても無効になります。" + Environment.NewLine);
                }
                else initialization_completed = true;
            }
            else if (already_m.Success)
            {
                System.Windows.Forms.MessageBox.Show("そのニックネームは既に使用されています。");
                Exit();
            }
        }
        public static async Task ConnectionAync(string[] args)
        {
            var irc_server = Ini.Read<Irc.Irc_server>("Irc_server","config.ini");
            irc.Encoding = System.Text.Encoding.GetEncoding("ISO-2022-JP");
            // wait time between messages, we can set this lower on own irc servers
            irc.SendDelay = 200;
            // we use channel sync, means we can use irc.GetChannel() and so on
            irc.ActiveChannelSyncing = true;
            // here we connect the events of the API to our written methods
            // most have own event handler types, because they ship different data
            irc.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
            irc.OnError += new ErrorEventHandler(OnError);
            irc.OnRawMessage += new IrcEventHandler(OnRawMessage);
            string server = irc_server.address;         //"irc.ircnet.ne.jp";
            int port = irc_server.port;                 //6664;
            string channel = irc_server.channel;        //"#cookie_channel";
            try
            {
                // here we try to connect to the server and exceptions get handled
                irc.Connect(server, port);
            }
            catch (ConnectionException e)
            {
                // something went wrong, the reason will be shown
                MessageBox.Show("接続出来ませんでした \n原因: " + e.Message);
                Exit();
            }
            try
            {
                // here we logon and register our nickname and so on
                irc.Login(irc_server.nickname + "_", "Cookiesound");
                // join the channel
                irc.RfcJoin(channel);
                /*for (int i = 0; i < 3; i++)
                {
                    // here we send just 3 different types of messages, 3 times for
                    // testing the delay and flood protection (messagebuffer work)
                    irc.SendMessage(SendType.Message, channel, "test message (" + i.ToString() + ")");
                    irc.SendMessage(SendType.Action, channel, "thinks this is cookie (" + i.ToString() + ")");
                    irc.SendMessage(SendType.Notice, channel, "SmartIrc4net rocks (" + i.ToString() + ")");
                }*/
                // here we tell the IRC API to go into a receive mode, all events
                // will be triggered by _this_ thread (main thread in this case)
                // Listen() blocks by default, you can also use ListenOnce() if you
                // need that does one IRC operation and then returns, so you need then
                // an own loop
                //Thread listen = new Thread(new ThreadStart(irc.Listen())); 
                //irc.Listen();
                irc.ListenOnce();
                // when Listen() returns our IRC session is over, to be sure we call
                // disconnect manually
                //irc.Disconnect();
            }
            catch (ConnectionException e)
            {
                // this exception is handled because Disconnect() can throw a not
                // connected exception
                MessageBox.Show("接続出来ませんでした \n原因: " + e.Message);
                Exit();
            }
            catch (Exception e)
            {
                // this should not happen by just in case we handle it nicely
                MessageBox.Show("接続出来ませんでした \n原因: " + e.Message);
                Exit();
            }
        }
        
        public static void Exit()
        {
            // we are done, lets exit...
            //System.Console.WriteLine("Exiting...");
            System.Environment.Exit(0);
        }

    }
}