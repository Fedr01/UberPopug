using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskCompletedEvent : Event
    {
        public Guid PublicId { get; set; }

        public TaskCompletedEvent() : base(nameof(TaskCompletedEvent),"v1")
        {
        }
    }
}