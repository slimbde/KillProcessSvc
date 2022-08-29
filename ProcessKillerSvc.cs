using KillProcessSvc.Models.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Timers;


namespace KillProcessSvc
{
    public partial class ProcessKillerSvc : ServiceBase
    {
        Timer timer;                                // polling timer
        Delegates.LogHandler logger;
        IEnumerable<string> processesToKill;



        public ProcessKillerSvc(Delegates.LogHandler logger)
        {
            InitializeComponent();

            this.logger = logger;
            this.CanStop = true;
            this.CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (timer != null)
                    timer.Dispose();

                timer = new Timer();
                double timeScale = double.Parse(ConfigurationManager.AppSettings["TimeScale"], CultureInfo.InvariantCulture);
                timer.Interval = 1000 * timeScale;
                timer.AutoReset = bool.Parse(ConfigurationManager.AppSettings["InfinitePolling"]);        // true - timer should fire its event infinitely (false - once)

                timer.Elapsed += Run;
                timer.Start();

                if (logger != null)
                    logger.Invoke("Service started", EventLogEntryType.Information);

                Run(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Invoke("[Service OnStart]: " + ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Error);
            }
        }



        protected override void OnStop()
        {
            timer.Stop();
            timer.Dispose();

            if (logger != null)
                logger.Invoke("Service stopped", EventLogEntryType.Information);
        }



        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }



        void Run(object sender, EventArgs args)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("ProcessesToKill");

                double timeScale = double.Parse(ConfigurationManager.AppSettings["TimeScale"], CultureInfo.InvariantCulture);
                timer.Interval = 1000 * timeScale;
                timer.AutoReset = bool.Parse(ConfigurationManager.AppSettings["InfinitePolling"]);
                bool enableTrace = bool.Parse(ConfigurationManager.AppSettings["EnableTrace"]);

                processesToKill = (ConfigurationManager.GetSection("ProcessesToKill") as IEnumerable<ProcessNameConfig>).Select(e => e.ProcessName).ToList();

                foreach (string processName in processesToKill)
                {
                    if (logger != null && enableTrace)
                        logger.Invoke("Looking for '" + processName + "'", EventLogEntryType.Information);

                    foreach (var process in Process.GetProcessesByName(processName))
                    {
                        process.Kill();
                        if (logger != null)
                            logger.Invoke("'" + processName + "' spotted and was killed", EventLogEntryType.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Invoke("[Service OnElapsed]: " + ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Error);
            }

        }
    }
}
