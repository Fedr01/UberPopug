using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskCompletedEvent : IEvent
    {
        public TaskCompletedEvent(Guid publicId)
        {
            PublicId = publicId;
        }

        public Guid PublicId { get; set; }

        public EventMetaData MetaData { get; set; } = new EventMetaData(nameof(TaskCompletedEvent), "v1");
    }
}