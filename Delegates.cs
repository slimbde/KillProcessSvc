using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KillProcessSvc
{
    public class Delegates
    {
        /// <summary>
        /// The delegate to handle messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgType"></param>
        public delegate void LogHandler(string message, EventLogEntryType msgType);
    }
}
