using System;
using UberPopug.Common;

namespace UberPopug.TaskTrackerService.Tasks.Messages
{
    public class TaskAssignedEvent : Event
    {
        public Guid PublicId { get; set; }

        public string AssignedToEmail { get; set; }

        public TaskAssignedEvent() : base(nameof(TaskAssignedEvent))
        {
        }
    }
}