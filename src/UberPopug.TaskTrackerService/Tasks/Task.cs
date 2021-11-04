using System;
using UberPopug.TaskTrackerService.Users;


namespace UberPopug.TaskTrackerService.Tasks
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public User User { get; set; }

        public string UserEmail { get; set; }
    }
}