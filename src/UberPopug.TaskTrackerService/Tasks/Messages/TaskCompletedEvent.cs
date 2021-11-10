using System;
using UberPopug.Common;

namespace UberPopug.TaskTrackerService.Tasks.Messages
{
    public class TaskCompletedEvent : Event
    {
        public Guid PublicId { get; set; }

        public TaskCompletedEvent() : base(nameof(TaskCompletedEvent))
        {
        }
    }
}