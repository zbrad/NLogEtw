using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using E = System.Diagnostics.Tracing;
using System.Reflection;

namespace ZBrad.NLogEtw
{
    [Target("EtwTarget")]
    public sealed class EtwTarget : TargetWithLayout
    {
        static NLogSource log;

        [RequiredParameter]
        public string Source { get; set; }

        protected override void InitializeTarget()
        {
            log = new NLogSource();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (!log.IsEnabled())
                return;

            log.EtwWrite(logEvent);
        }

        public static string GetManifest()
        {
            return NLogSource.GetManifest();
        }
    }
}
