using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;



namespace KillProcessSvc
{
    static class Program
    {
        static Delegates.LogHandler logger = GetLogger();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                ProcessKillerSvc service = new ProcessKillerSvc(logger);

                if (Environment.UserInteractive)
                    service.RunAsConsole(args);
                else
                    ServiceBase.Run(service);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Invoke(ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Error);
            }
        }


        static Delegates.LogHandler GetLogger()
        {
            if (Environment.UserInteractive)
                return delegate(string message, EventLogEntryType msgType)
                {
                    Console.WriteLine(DateTime.Now.ToString("G") + " " + message);
                };

            EventLog eventLog = new EventLog();
            eventLog.Source = "KillProcessSrc";
            eventLog.Log = "KillProcessLog";

            return eventLog.WriteEntry;
        }
    }
}
