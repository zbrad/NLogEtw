using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEtw
{
    class Program
    {
        static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            using (var listener = new ConsoleEventListener())
            {
                log.Trace("trace message");
                log.Debug("debug message");
                log.Info("info message");
                log.Warn("warn message");
                log.Error("error message");
                log.Fatal("fatal message");

                var e = new ApplicationException("test exception");
                log.Trace(e, "trace message");
                log.Debug(e, "debug message");
                log.Info(e, "info message");
                log.Warn(e, "warn message");
                log.Error(e, "error message");
                log.Fatal(e, "fatal message");

                Console.ReadLine();
            }
        }
    }
}
