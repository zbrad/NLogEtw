using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Diagnostics.Tracing;

namespace TestEtwPackage
{
    public class ConsoleEventData
    {
        public string Name;
        public string Message;
    }

    public class ConsoleEventListener : EventListener
    {
        static TextWriter Out = Console.Out;
        public List<ConsoleEventData> Messages = new List<ConsoleEventData>();

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var d = new ConsoleEventData();
            d.Name = eventData.EventName;
            d.Message = getMessage(eventData);
            Messages.Add(d);
        }

        static string getMessage(EventWrittenEventArgs d)
        {
            var objects = new object[d.Payload.Count];
            d.Payload.CopyTo(objects, 0);
            return string.Format(d.Message, objects);
        }
    }
}
