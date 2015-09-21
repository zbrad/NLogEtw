using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using NLog;

namespace ZBrad.NLogEtw
{
    public abstract class NLogSource : EventSource
    {
        static NLogSource log;

        public static NLogSource Log { get { return log; } }

        public NLogSource()
        {
            log = this;
        }

        [Event(1, Level = EventLevel.Verbose)]
        public void Verbose(LogEventInfo info, string Message)
        {
            WriteEvent(1, info.LoggerName, Message, "classname");
        }

        [Event(2, Level = EventLevel.Informational)]
        public void Info(LogEventInfo info, string Message)
        {
            WriteEvent(2, info.LoggerName, Message);
        }

        [Event(3, Level = EventLevel.Warning)]
        public void Warn(LogEventInfo info, string Message)
        {
            WriteEvent(3, info.LoggerName, Message);
        }

        [Event(4, Level = EventLevel.Error)]
        public void Error(LogEventInfo info, string Message)
        {
            WriteEvent(4, info.LoggerName, Message);
        }

        [Event(5, Level = EventLevel.Critical)]
        public void Critical(LogEventInfo info, string Message)
        {
            WriteEvent(5, info.LoggerName, Message);
        }
    }


}
