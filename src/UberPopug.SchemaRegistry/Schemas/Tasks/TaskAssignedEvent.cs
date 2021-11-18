using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskAssignedEvent : IEvent
    {
        public TaskAssignedEvent(Guid publicId, string email)
        {
            PublicId = publicId;
            AssignedToEmail = email;
        }

        public Guid PublicId { get; set; }
        public string AssignedToEmail { get; set; }

        public EventMetaData MetaData { get; set; } = new EventMetaData(nameof(TaskAssignedEvent), "v1");
    }
}