using System;

namespace UberPopug.SchemaRegistry
{
    public class EventMetaData
    {
        public Guid EventId { get; set; }

        public string EventName { get; set; }

        public string EventVersion { get; set; }

        public DateTime EventTime { get; set; }

        public string Producer { get; set; }
    }
}