using System;

namespace UberPopug.SchemaRegistry.Schemas.Tasks
{
    public class TaskAssignedEvent : Event
    {
        public Guid PublicId { get; set; }

        public string AssignedToEmail { get; set; }

        public TaskAssignedEvent() : base(nameof(TaskAssignedEvent), "v1")
        {
        }
    }
}