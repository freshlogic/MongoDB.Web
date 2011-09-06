using System;
using System.Web.Hosting;
using System.Web.Management;

namespace MongoDB.Web.Providers
{
    internal class WebEvent
    {
        public string ApplicationPath { get; set; }
        public string ApplicationVirtualPath { get; set; }
        public string Details { get; set; }
        public int EventCode { get; set; }
        public int EventDetailCode { get; set; }
        public Guid EventID { get; set; }
        public long EventOccurrence { get; set; }
        public long EventSequence { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime EventTimeUtc { get; set; }
        public string EventType { get; set; }
        public string ExceptionType { get; set; }
        //public string MachineName { get; set; }
        public string Message { get; set; }
        //public string RequestUrl { get; set; }

        public static WebEvent FromWebBaseEvent(WebBaseEvent webBaseEvent)
        {
            var webEvent = new WebEvent();

            webEvent.ApplicationPath = HostingEnvironment.ApplicationPhysicalPath;
            webEvent.ApplicationVirtualPath = HostingEnvironment.ApplicationVirtualPath;
            webEvent.Details = webBaseEvent.ToString();
            webEvent.EventCode = webBaseEvent.EventCode;
            webEvent.EventDetailCode = webBaseEvent.EventDetailCode;
            webEvent.EventID = webBaseEvent.EventID;
            webEvent.EventOccurrence = webBaseEvent.EventOccurrence;
            webEvent.EventSequence = webBaseEvent.EventSequence;
            webEvent.EventTime = webBaseEvent.EventTime;
            webEvent.EventTimeUtc = webBaseEvent.EventTimeUtc;
            webEvent.EventType = webBaseEvent.GetType().Name;
            //webEvent.MachineName = HttpContext.Current.Server.MachineName;
            webEvent.Message = webBaseEvent.Message;
            //webEvent.RequestUrl = HttpContext.Current.Request.Url.ToString();

            if (webBaseEvent is WebBaseErrorEvent)
            {
                webEvent.ExceptionType = ((WebBaseErrorEvent)webBaseEvent).ErrorException.GetType().Name;
            }

            return webEvent;
        }
    }
}