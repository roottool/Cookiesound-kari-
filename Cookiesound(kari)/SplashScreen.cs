using System;
using System.Drawing;
using System.Windows.Forms;

namespace Cookiesound_kari_
{
    public partial class SplashScreen : Form
    {
        private readonly dynamic _wmp = Activator.CreateInstance(Type.GetTypeFromProgID("WMPlayer.OCX.7"));

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_FormClosing(object sender, FormClosingEventArgs e)
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

        //Omake
        private void PlayButton_Click(object sender, EventArgs e)
        {
            //When the user push the play button, Cookie☆ is played.
            if (System.IO.File.Exists("sound/csr/csr0.mp3"))
            {
                _wmp.URL = "./sound/csr/csr0.mp3";
                _wmp.controls.play();
            }
        }
    }
}
