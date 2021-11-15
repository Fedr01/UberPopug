using System;

namespace UberPopug.SchemaRegistry
{
    public class Event
    {
        public Event()
        {
        }

        protected Event(string eventName, string version)
        {
            MetaData = new EventMetaData
            {
                EventName = eventName,
                EventVersion = version,
                EventId = Guid.NewGuid(),
                EventTime = DateTime.UtcNow
            };
        }
        public EventMetaData MetaData { get; set; }
    }
}