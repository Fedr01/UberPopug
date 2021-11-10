using System;
using UberPopug.Common;

namespace UberPopug.TaskTrackerService.Tasks.Messages
{
    public class TaskCreatedEvent : Event

    {
        public Guid PublicId { get; set; }

        public string Description { get; set; }

        public TaskCreatedEvent() : base(nameof(TaskCreatedEvent))
        {
        }
    }
}