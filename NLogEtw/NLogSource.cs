using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using NLog;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ZBrad.NLogEtw
{
    [EventSource(Name = "NLog")]
    internal sealed class NLogSource : EventSource
    {
        public class Tasks
        {
            public const EventTask Trace = (EventTask)1000;
            public const EventTask Debug = (EventTask)2000;
            public const EventTask Info = (EventTask)3000;
            public const EventTask Warn = (EventTask)4000;
            public const EventTask Error = (EventTask)5000;
            public const EventTask Fatal = (EventTask)6000;
        }

        public class Opcodes
        {
            public const EventOpcode Basic = (EventOpcode)101;
            public const EventOpcode Stack = (EventOpcode)102;
            public const EventOpcode Exception = (EventOpcode)103;
        }

        public class EventIds
        {
            public const int Trace_Basic = (int)Tasks.Trace + (int)Opcodes.Basic;
            public const int Trace_Stack = (int)Tasks.Trace + (int)Opcodes.Stack;
            public const int Trace_Exception = (int)Tasks.Trace + (int)Opcodes.Exception;

            public const int Debug_Basic = (int)Tasks.Debug + (int)Opcodes.Basic;
            public const int Debug_Stack = (int)Tasks.Debug + (int)Opcodes.Stack;
            public const int Debug_Exception = (int)Tasks.Debug + (int)Opcodes.Exception;

            public const int Info_Basic = (int)Tasks.Info + (int)Opcodes.Basic;
            public const int Info_Stack = (int)Tasks.Info + (int)Opcodes.Stack;
            public const int Info_Exception = (int)Tasks.Info + (int)Opcodes.Exception;

            public const int Warn_Basic = (int)Tasks.Warn + (int)Opcodes.Basic;
            public const int Warn_Stack = (int)Tasks.Warn + (int)Opcodes.Stack;
            public const int Warn_Exception = (int)Tasks.Warn + (int)Opcodes.Exception;

            public const int Error_Basic = (int)Tasks.Error + (int)Opcodes.Basic;
            public const int Error_Stack = (int)Tasks.Error + (int)Opcodes.Stack;
            public const int Error_Exception = (int)Tasks.Error + (int)Opcodes.Exception;

            public const int Fatal_Basic = (int)Tasks.Fatal + (int)Opcodes.Basic;
            public const int Fatal_Stack = (int)Tasks.Fatal + (int)Opcodes.Stack;
            public const int Fatal_Exception = (int)Tasks.Fatal + (int)Opcodes.Exception;

        }

        static Dictionary<LogLevel, EventTask> levels;
        static NLogSource()
        {
            levels = new Dictionary<LogLevel, EventTask>();
            levels.Add(LogLevel.Trace, Tasks.Trace);
            levels.Add(LogLevel.Debug, Tasks.Debug);
            levels.Add(LogLevel.Info, Tasks.Info);
            levels.Add(LogLevel.Warn, Tasks.Warn);
            levels.Add(LogLevel.Error, Tasks.Error);
            levels.Add(LogLevel.Fatal, Tasks.Fatal);
        }

        [NonEvent]
        public unsafe void EtwWrite(LogEventInfo data)
        {
            var opcode = getOpcode(data);
            var args = new List<GCHandle>();

            try
            {
                addHeader(args, data);
                addProperties(args, data);

                switch (opcode)
                {
                    case Opcodes.Exception:
                        addException(args, data);
                        break;
                    case Opcodes.Stack:
                        addStack(args, data);
                        break;
                    default:
                        break;
                }

                EventData* d = stackalloc EventData[args.Count];
                addEventData(d, args);

                var eventId = getId(data, opcode);
                WriteEventCore(eventId, args.Count, d);
            }
            finally
            {
                foreach (var p in args)
                    p.Free();
            }
        }

        int getId(LogEventInfo data, EventOpcode opcode)
        {
            var level = (int)levels[data.Level];
            var code = (int)opcode;
            return level + code;
        }

        // data is in 3 forms:
        //      basic info
        //      exception info
        //      stack info
        static EventOpcode getOpcode(LogEventInfo data)
        {
            // determine which log event this should be
            if (data.Exception != null)
                return Opcodes.Exception;
            if (data.HasStackTrace)
                return Opcodes.Stack;
            return Opcodes.Basic;
        }

        static void addHeader(List<GCHandle> args, LogEventInfo data)
        {
            args.Add(GCHandle.Alloc(data.FormattedMessage, GCHandleType.Pinned));
            args.Add(GCHandle.Alloc(data.LoggerName, GCHandleType.Pinned));
            args.Add(GCHandle.Alloc(data.SequenceID, GCHandleType.Pinned));
            args.Add(GCHandle.Alloc(data.TimeStamp.ToString("o"), GCHandleType.Pinned));
        }

        static void addException(List<GCHandle> args, LogEventInfo data)
        {
            // int hresult,
            // string type,
            // string exceptionMessage
            args.Add(GCHandle.Alloc(data.Exception.HResult, GCHandleType.Pinned));
            args.Add(GCHandle.Alloc(data.Exception.GetType().Name, GCHandleType.Pinned));
            args.Add(GCHandle.Alloc(data.Exception.Message, GCHandleType.Pinned));
        }

        static void addStack(List<GCHandle> args, LogEventInfo data)
        {
            args.Add(GCHandle.Alloc(data.StackTrace.ToString()));
        }


        static void addProperties(List<GCHandle> args, LogEventInfo data)
        {
            if (data.Properties == null)
                return;

            // add any properties
            foreach (var kv in data.Properties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(kv.Key);
                sb.Append('=');
                sb.Append(kv.Value);

                var s = sb.ToString();
                var p = GCHandle.Alloc(s, GCHandleType.Pinned);
                args.Add(p);
            }
        }

        unsafe static void addEventData(EventData* d, List<GCHandle> args)
        {
            for (var i = 0; i < args.Count; i++)
            {
                d[i].DataPointer = args[i].AddrOfPinnedObject();
                d[i].Size = 4;
            }
        }

        [NonEvent]
        static unsafe void addData(ref EventData* dp, char* data)
        {
            if (data != null)
            {
                (*dp).DataPointer = (IntPtr)data;
                (*dp).Size = 4;
                dp++;
            }
        }

        [Event(EventIds.Trace_Basic, Task = Tasks.Trace, Opcode = Opcodes.Basic, Level = EventLevel.LogAlways, Message = "{0}")]
        public void Trace(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Trace_Stack, Task = Tasks.Trace, Opcode = Opcodes.Stack, Level = EventLevel.LogAlways, Message = "{0}")]
        public void Trace_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Trace_Exception, Task = Tasks.Trace, Opcode = Opcodes.Exception, Level = EventLevel.LogAlways, Message = "{0}: Exception: {5}:{6}")]
        public void Trace_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
                )
        {

        }

        [Event(EventIds.Debug_Basic, Task = Tasks.Debug, Opcode = Opcodes.Basic, Level = EventLevel.Verbose, Message = "{0}")]
        public void Debug(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Debug_Stack, Task = Tasks.Debug, Opcode = Opcodes.Stack, Level = EventLevel.Verbose, Message = "{0}")]
        public void Debug_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Debug_Exception, Task = Tasks.Debug, Opcode = Opcodes.Exception, Level = EventLevel.Verbose, Message = "{0}: Exception: {5}:{6}")]
        public void Debug_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
                )
        {

        }

        [Event(EventIds.Info_Basic, Task = Tasks.Info, Opcode = Opcodes.Basic, Level = EventLevel.Informational, Message = "{0}")]
        public void Info(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Info_Stack, Task = Tasks.Info, Opcode = Opcodes.Stack, Level = EventLevel.Informational, Message = "{0}")]
        public void Info_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Info_Exception, Task = Tasks.Info, Opcode = Opcodes.Exception, Level = EventLevel.Informational, Message = "{0}: Exception: {5}:{6}")]
        public void Info_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
                )
        {

        }

        [Event(EventIds.Warn_Basic, Task = Tasks.Warn, Opcode = Opcodes.Basic, Level = EventLevel.Warning, Message = "{0}")]
        public void Warn(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Warn_Stack, Task = Tasks.Warn, Opcode = Opcodes.Stack, Level = EventLevel.Warning, Message = "{0}")]
        public void Warn_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Warn_Exception, Task = Tasks.Warn, Opcode = Opcodes.Exception, Level = EventLevel.Warning, Message = "{0}: Exception: {5}:{6}")]
        public void Warn_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
            )
        {

        }

        [Event(EventIds.Error_Basic, Task = Tasks.Error, Opcode = Opcodes.Basic, Level = EventLevel.Error, Message = "{0}")]
        public void Error(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Error_Stack, Task = Tasks.Error, Opcode = Opcodes.Stack, Level = EventLevel.Error, Message = "{0}")]
        public void Error_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Error_Exception, Task = Tasks.Error, Opcode = Opcodes.Exception, Level = EventLevel.Error, Message = "{0}: Exception: {5}:{6}")]
        public void Error_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
                )
        {

        }

        [Event(EventIds.Fatal_Basic, Task = Tasks.Fatal, Opcode = Opcodes.Basic, Level = EventLevel.Critical, Message = "{0}")]
        public void Fatal(
                string message,
                string logger,
                int sequence,
                string timestamp
                )
        {

        }

        [Event(EventIds.Fatal_Stack, Task = Tasks.Fatal, Opcode = Opcodes.Stack, Level = EventLevel.Critical, Message = "{0}")]
        public void Fatal_Stack(
                string message,
                string logger,
                int sequence,
                string timestamp,
                string stack
                )
        {

        }

        [Event(EventIds.Fatal_Exception, Task = Tasks.Fatal, Opcode = Opcodes.Exception, Level = EventLevel.Critical, Message = "{0}: Exception: {5}:{6}")]
        public void Fatal_Exception(
                string message,
                string logger,
                int sequence,
                string timestamp,
                int hresult,
                string type,
                string exceptionMessage
                )
        {

        }

        [NonEvent]
        public static string GetManifest()
        {
            return EventSource.GenerateManifest(typeof(NLogSource), Assembly.GetExecutingAssembly().FullName);
        }
    }
}
