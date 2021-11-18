using System;
using System.Reflection;

namespace UberPopug.SchemaRegistry.Schemas
{
    public class EventMetaData
    {
        public EventMetaData(string eventName, string version)
        {
            EventId = Guid.NewGuid();
            EventTime = DateTime.UtcNow;
            EventName = eventName;
            Version = version;
            Producer = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string Version { get; set; }
        public DateTime EventTime { get; set; }
        public string Producer { get; set; }
    }
}