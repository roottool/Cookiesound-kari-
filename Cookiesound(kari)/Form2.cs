using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cookiesound_kari_
{
    public partial class Form2 : Form
    {
        private readonly dynamic _wmp = Activator.CreateInstance(Type.GetTypeFromProgID("WMPlayer.OCX.7"));
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            _wmp.controls.stop();
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    break;
                case CloseReason.FormOwnerClosing:
                    break;
                case CloseReason.MdiFormClosing:
                    break;
                case CloseReason.TaskManagerClosing:
                    Environment.Exit(0);
                    break;
                case CloseReason.UserClosing:
                    Environment.Exit(0);
                    break;
                case CloseReason.WindowsShutDown:
                    Environment.Exit(0);
                    break;
                case CloseReason.None:
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("sound/csr/csr0.mp3"))
            {
                _wmp.URL = "./sound/csr/csr0.mp3";
                _wmp.controls.play();
            }
        }
    }
}
