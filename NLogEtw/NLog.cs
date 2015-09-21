using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using System.Diagnostics.Tracing;

namespace ZBrad.NLogEtw
{
    [Target("EventTracing")]
    public class EtwTarget : TargetWithLayout
    {
        static Dictionary<LogLevel, Action<LogEventInfo, string>> levels;

        NLogSource log;

        public EtwTarget()
        {
            log = NLogSource.Log;
            levels = new Dictionary<LogLevel, Action<LogEventInfo, string>>();
            levels.Add(LogLevel.Debug, log.Verbose);
            levels.Add(LogLevel.Trace, log.Verbose);
            levels.Add(LogLevel.Info, log.Info);
            levels.Add(LogLevel.Warn, log.Warn);
            levels.Add(LogLevel.Error, log.Error);
            levels.Add(LogLevel.Fatal, log.Critical);
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (!log.IsEnabled())
                return;

            if (levels == null)
                return;

            levels[logEvent.Level](logEvent, Layout.Render(logEvent));
            }
        }

       
}
