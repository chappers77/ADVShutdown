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
        static private string strActivate, strAction, strReason, strCommandReceived;
        static private double dblDelay;
        static private bool boolContinue;
        static private StreamReader streamInput;
        static private StreamWriter streamOutput;
        private NamedPipeServerStream streamServer;
        static private Form formWindow;

        public ADVShutdown()
        {
            InitializeComponent();
            strActivate = Hardware.HardwareID();
            strActivate += DateTime.Today.ToLocalTime().ToBinary().ToString();
            this.MoveWindow();
            this.StartServer();

        }

        static void Main(string[] args)
        {
            formWindow = new ADVShutdown();
            //formWindow.Visible = false;
            Application.EnableVisualStyles();
            Application.Run(formWindow);
        }

        public void StartServer()
        {
            streamServer = new NamedPipeServerStream("ADVShutdown", PipeDirection.InOut, 5);
            streamServer.WaitForConnection();
            streamInput = new StreamReader(streamServer);
            streamOutput = new StreamWriter(streamServer);
            streamOutput.AutoFlush = true;
            strCommandReceived = streamInput.ReadLine().ToString();
            this.CheckCommand();
        }

        private void RestartServer()
        {
            streamServer.Close();
            streamServer = null;
            this.StartServer();
        }

        private void CheckCommand()
        {
            string[] arrCommands = strCommandReceived.Split('¬');
            if (String.Equals(arrCommands[0], strActivate))
            {
                boolContinue = true;
                dblDelay = 300;
                strAction = "";
                strReason = "No Reason Given";
                string strResult = "";

                if (String.Equals(arrCommands[1].ToLower(), "cancel"))
                {
                    strAction = arrCommands[1].ToLower();
                    strResult += "Cancelled";
                }
                else
                {
                    if (!String.Equals(arrCommands[1], ""))
                    {
                        strAction = arrCommands[1].ToLower();
                        strResult += "Action set¬";
                    }
                    else
                    {
                        strResult += "No action specified¬";
                        boolContinue = false;
                    }

                    if (!String.Equals(arrCommands[2], ""))
                    {
                        try
                        {
                            dblDelay = Convert.ToDouble(arrCommands[2]);
                            strResult += "Timer set OK¬";
                        }
                        catch (FormatException)
                        {
                            strResult += "Unable to convert '{0}' to a Double. Using the default 5 minute timer¬";
                        }
                        catch (OverflowException)
                        {
                            strResult += "'{0}' is outside the range of a Double. Using the default 5 minute timer¬";
                        }
                    }
                    else strResult += "Using default 5 minute timer¬";

                    if (!String.Equals(arrCommands[3], ""))
                    {
                        strReason = arrCommands[3];
                        strResult += "Reason set¬";
                    }
                    else strResult += "Using default reason text¬";
                }

                if (boolContinue)
                {
                    if (String.Equals(strResult, "Cancelled"))
                    {
                        if (this.Visible == true)
                        {
                            this.Visible = false;
                        }
                    }
                    else
                    {
                        this.LoadSettings();
                        if (this.Visible == false)
                        {
                            this.Visible = true;
                        }
                    }
                    streamOutput.WriteLine(strResult);
                }
                else
                {
                    strResult += "Command " + strCommandReceived + "Not Accepted¬";
                    strResult += "Incorrect parameters - " + strAction + " cancelled";
                    streamOutput.WriteLine(strResult); 
                }
            }

            RestartServer();
        }

       

        /// <summary>
        /// Load the options into the window
        /// </summary>
        private void LoadSettings()
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
