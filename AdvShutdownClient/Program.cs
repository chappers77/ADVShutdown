using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;

namespace AdvShutdownClient
{
    class Program
    {
        static private string strActivate, strAction, strReason, strCommandReceived;
        static private double dblDelay;
        static private bool boolContinue;
        static private StreamReader streamInput;
        static private StreamWriter streamOutput;
        static private NamedPipeServerStream streamServer;
        static private Form formWindow;

        static void Main(string[] args)
        {
            strActivate = Hardware.HardwareID();
            strActivate += DateTime.Today.ToLocalTime().ToBinary().ToString();
            Application.EnableVisualStyles();
            StartServer();
        }

        static public void StartServer()
        {
            streamServer = new NamedPipeServerStream("ADVShutdown", PipeDirection.InOut, 5);
            streamServer.WaitForConnection();
            streamInput = new StreamReader(streamServer);
            streamOutput = new StreamWriter(streamServer);
            streamOutput.AutoFlush = true;
            strCommandReceived = streamInput.ReadLine().ToString();
            CheckCommand();
        }

        static private void RestartServer()
        {
            streamServer.Close();
            streamServer = null;
            StartServer();
        }

        static private void CheckCommand()
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


                    if (!String.Equals(arrCommands[3], ""))
                    {
                        strReason = arrCommands[3];
                        strResult += "Reason set¬";
                    }
                    else strResult += "Using default reason text¬";

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
                }

                if (boolContinue)
                {
                    if (String.Equals(strResult, "Cancelled"))
                    {
                        if (formWindow != null)
                        {
                            formWindow.Dispose();
                            formWindow = null;
                        }
                    }
                    else
                    {
                        if (formWindow != null)
                        {
                            formWindow.Dispose();
                            formWindow = null;
                        }
                        formWindow = new ADVShutdown(strAction, dblDelay, strReason);
                        formWindow.Show();
                        Application.DoEvents();
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
    }
}
