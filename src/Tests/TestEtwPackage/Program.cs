using System.Diagnostics;
using System.Threading.Tasks;

namespace TestEtwPackage
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

                // briefly pause
                Task.Delay(100).Wait();

                // now validate

                Debug.Assert(listener.Messages[0].Message == "trace message");
                Debug.Assert(listener.Messages[1].Message == "debug message");
                Debug.Assert(listener.Messages[2].Message == "info message");
                Debug.Assert(listener.Messages[3].Message == "warn message");
                Debug.Assert(listener.Messages[4].Message == "error message");
                Debug.Assert(listener.Messages[5].Message == "fatal message");
            }
        }
    }
}
