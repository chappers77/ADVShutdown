using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;

namespace AdvShutdownClient
{
    public partial class ADVShutdown : Form
    {
        public ADVShutdown(string strAction, double dblDelay, string strReason)
        {
            InitializeComponent();
            this.MoveWindow();
            LoadSettings(strAction, dblDelay, strReason);
        }

        /// <summary>
        /// Load the options into the window
        /// </summary>
        public void LoadSettings(string strAction, double dblDelay, string strReason)
        {
            this.labelAction.Text = this.labelAction.Text.Replace("ACTION", strAction);
            strReason = strReason.Replace("&&&", "\r\n");
            this.textBoxReason.Text = strReason;
            labelTime.Text = DateTime.Now.AddSeconds(dblDelay).ToShortTimeString();
        }

        /// <summary>
        /// Move the window to the bottom right of the screen and make topmost
        /// </summary>
        private void MoveWindow()
        {
            this.SetTopLevel(true);
            this.Height = 275;
            this.Width = 230;
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            if (Screen.PrimaryScreen.WorkingArea.Top == 0)
            {
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            }
            else
            {
                this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height;
            }
        }
    }
}
