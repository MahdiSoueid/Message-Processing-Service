using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace MyNewService
{
    public partial class MyNewService : ServiceBase
    {
        private int eventID = 1;
        public MyNewService(string[] args)
        {

            InitializeComponent();

            string eventSourceName = "MySource";
            string logName = "MyNewLog";
            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Length > 1)
            {
                logName = args[1];
            }
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";

        }

        protected override void OnStart(string[] args)
        {

            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


            eventLog1.WriteEntry("In OnStart.");

            Timer timer = new Timer
            {
                Interval = 10000
            };
            timer.Elapsed += (sender, e) => TimerElapsed(start); ;
            timer.AutoReset = true;
            timer.Start();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;

            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Innn OnStop.");
        }

        public void OnTimer(Object sender, ElapsedEventArgs args)
        {
            eventLog1.WriteEntry("Monitoring the system", EventLogEntryType.Information, eventID++);
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        public void start()
        {
            InsertMessages();
            ProcessMessages();

        }
        private void TimerElapsed(Action start)
        {
            start();
        }

        public static void ProcessMessages()
        {  
            string connectionString = "Server=DESKTOP-FI240MH\\SQLEXPRESS;Database=Test;User Id=sa;Password=P@ssw0rd;";
            Messages.processMessages(connectionString);
        }
        public static void InsertMessages()
        {
            string connectionString = "Server=DESKTOP-FI240MH\\SQLEXPRESS;Database=Test;User Id=sa;Password=P@ssw0rd;";

            try
            {
               
                bool noErrors = true;
                string successPath = Configuration.getSuccessFolder(connectionString);
                Console.WriteLine(successPath);
                string errorPath = Configuration.getErrorFolder(connectionString);
                Console.WriteLine(errorPath);
                EventLogs eventLog;

                ArrayList FoldersPaths = Configuration.getFolders(connectionString);
                foreach (string folderPath in FoldersPaths)
                {
                    string[] files = Directory.GetFiles(folderPath);


                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        if (Path.GetFileName(file) != "desktop.ini")
                        {
                            Console.WriteLine(file);

                            string input = File.ReadAllText(file);
                            string dateTimeFile = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            DateTime creationaTime = File.GetCreationTime(file);




                            MesgFiles msgfile = new MesgFiles(Path.GetFileName(file), creationaTime.ToString("yyyy-MM-dd HH:mm:ss"), input, dateTimeFile);



                            int fileid = MesgFiles.InsertMsgFiles(connectionString, msgfile);
                            if (fileid <= 0)
                            {
                                File.Move(file, errorPath + "\\" + Path.GetFileName(file));
                                eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File " + Path.GetFileName(file) + " Moved to error Folder ", "Moving Folder");
                                EventLogs.InsertEventLog(connectionString, eventLog);
                                break;
                            }


                            string[] messages = input.Split('$');
                            foreach (string messageString in messages)
                            {
                                DateTime date = DateTime.Now;
                                string dateTimeMessage = date.ToString("yyyy-MM-dd_HH-mm-ss-fff");


                                Messages message = new Messages(messageString, dateTimeMessage);


                                int messageid = Messages.InsertMessage(connectionString, message, msgfile, fileid);
                                if (messageid <= 0)
                                {
                                    File.Move(file, errorPath + "\\" + Path.GetFileName(file));
                                    eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File " + Path.GetFileName(file) + " Moved to error Folder ", "Moving Folder");
                                    EventLogs.InsertEventLog(connectionString, eventLog);
                                    break;
                                }



                                string dateTimeInterv = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");




                                MessageInterventions interv = new MessageInterventions("Message is being processed !!!!", dateTimeInterv);


                                int intervid = MessageInterventions.InsertMsgInterv(connectionString, message, interv, messageid);
                                if (intervid <= 0)
                                {

                                    File.Move(file, errorPath + "\\" + Path.GetFileName(file));
                                    eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File " + Path.GetFileName(file) + " Moved to error Folder ", "Moving Folder");
                                    EventLogs.InsertEventLog(connectionString, eventLog);
                                    break;
                                }


                            }

                            File.Move(file, successPath + "\\" + Path.GetFileName(file));

                            eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "File " + Path.GetFileName(file) + " Moved to Success Folder.No errors encountered. ", "Moving Folder");
                            EventLogs.InsertEventLog(connectionString, eventLog);


                        }

                    }

                }

            }
            catch(Exception e) {
                EventLogs eventLog = new EventLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Error while processing InsertMessagges() function:\n" + e.Message, "InsertMessagges() function");
                EventLogs.InsertEventLog(connectionString, eventLog);

            }
            

        }
    }
}
