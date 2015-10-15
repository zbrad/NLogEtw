using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using System.IO;

namespace TestEtw
{
    public class ConsoleEventListener : EventListener
    {
        static TextWriter Out = Console.Out;

        /// <summary>
        /// Override this method to get a list of all the eventSources that exist.  
        /// </summary>
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Because we want to turn on every EventSource, we subscribe to a callback that triggers
            // when new EventSources are created.  It is also fired when the EventListner is created
            // for all pre-existing EventSources.  Thus this callback get called once for every 
            // EventSource regardless of the order of EventSource and EventListener creation.  

            // For any EventSource we learn about, turn it on.   
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
        }

        /// <summary>
        /// We override this method to get a callback on every event we subscribed to with EnableEvents
        /// </summary>
        /// <param name="eventData"></param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // report all event information
            Out.Write("  Event {0} ", eventData.EventName);

            // Events can have formatting strings 'the Message property on the 'Event' attribute.  
            // If the event has a formatted message, print that, otherwise print out argument values.  
            if (eventData.Message != null)
                Out.WriteLine(eventData.Message, eventData.Payload != null ? eventData.Payload.ToArray() : null);
            else
            {
                string[] sargs = eventData.Payload != null ? eventData.Payload.Select(o => o.ToString()).ToArray() : null;
                Out.WriteLine("({0}).", sargs != null ? string.Join(", ", sargs) : "");
            }
        }

        #region Private members

        private static string ShortGuid(Guid guid)
        { return guid.ToString().Substring(0, 8); }

        #endregion
    }
}
