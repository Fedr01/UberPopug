using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskCreatedEvent : IEvent
    {
        public TaskCreatedEvent(Guid publicId)
        {
            PublicId = publicId;
        }

        public Guid PublicId { get; set; }

        public EventMetaData MetaData { get; set; } = new EventMetaData(nameof(TaskCreatedEvent), "v1");
    }
}