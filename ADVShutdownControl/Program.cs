using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;


namespace ADVShudownControl
{
    class Program
    {
        static void Main(string[] args)
        {
            string strAction = "";
            if (args.Count() > 0)
            {
                if (args[0].ToLower() != "reboot" && args[0].ToLower() != "shutdown" && args[0].ToLower() != "cancel")
                {
                    Error();
                }
                else
                {
                    strAction = "shutdown -";
                    switch (args[0])
                    {
                        case "shutdown":
                            strAction += "s -t ";
                            break;
                        case "reboot":
                            strAction += "r -t ";
                            break;
                        case "cancel":
                            strAction += "a";
                            break;
                    }
                    string strCommand = Hardware.HardwareID();
                    strCommand += DateTime.Today.ToLocalTime().ToBinary().ToString();
                    strCommand += "¬" + args[0];
                    bool boolCommandOK = true;
                    if (args.Count() > 1)
                    {
                        try
                        {
                            int i = int.Parse(args[1]);
                            strCommand += "¬" + args[1];
                            strAction += args[1];
                        }
                        catch
                        {
                            boolCommandOK = false;
                            Error();
                        }
                    }
                    else strCommand += "¬";

                    if (args.Count() > 2)
                    {
                        strCommand += "¬" + args[2];
                    }
                    else strCommand += "¬";

                    if (boolCommandOK)
                    {
                        NamedPipeClientStream streamClient = new NamedPipeClientStream("ADVShutdown");
                        streamClient.Connect();
                        StreamReader streamInput = new StreamReader(streamClient);
                        StreamWriter streamOutput = new StreamWriter(streamClient);
                        streamOutput.WriteLine(strCommand);
                        streamOutput.Flush();
                        Feedback(streamInput.ReadLine());
                        ProcessStartInfo procStartInfo = new ProcessStartInfo(strAction);
                    }
                }
            }
            else Error();
        }

        static void Feedback(string strFeedback)
        {
            string[] strReceived = strFeedback.Split('¬');
            for (int i = 0; i < strReceived.Count(); i++)
            {
                Console.WriteLine(strReceived[i]);
            }
        }

        static void Error()
        {
            Console.WriteLine("You must have at least one argument for this application to work.");
            Console.WriteLine("This must be the action - Shutdown, Reboot or Cancel");
            Console.WriteLine("The correct format for this command is:");
            Console.WriteLine("     ADVShutdown shutdown    -   this will shutdown the machine after the default 5 minutes.");
            Console.WriteLine("     ADVShutdown reboot      -   this will reboot the machine after the default 5 minutes.");
            Console.WriteLine("     ADVShutdown cancel      -   this will cancel any scheduled shutdown");
            Console.WriteLine();
            Console.WriteLine("You can also change the default time by adding a second argument in seconds.");
            Console.WriteLine("The arguments must be in the order 'action delay', e.g.");
            Console.WriteLine("     ADVShutdown shutdown 30    -   this will shutdown the machine after 30 seconds.");
            Console.WriteLine("     ADVShutdown reboot 90       -   this will reboot the machine after 90 seconds.");
            Console.WriteLine();
            Console.WriteLine("You can also add a 3rd argument for a reason to be displayed. You can include spaces, but if you do the entire reason must be in quotes.");
            Console.WriteLine("The correct format for this is:");
            Console.WriteLine("     ADVShutdown shutdown 10 \"Security Issue\"   -   this will shutdown the machine after 10 seconds and the user will see the reason Security Issue");
        }
    }
}
