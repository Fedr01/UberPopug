using System;
using UberPopug.TaskTrackerService.Users;


namespace UberPopug.TaskTrackerService.Tasks
{
    public class Task
    {
        public Task(string description)
        {
            Description = description;
            PublicId = Guid.NewGuid();
            Status = TaskStatus.InProgress;
        }

        public int Id { get; private set; }

        public Guid PublicId { get; private set; }

        public string Description { get; private set; }

        public TaskStatus Status { get; private set; }

        public User User { get; private set; }

        public string AssignedToEmail { get; private set; }

        public void AssignTo(User user)
        {
            AssignedToEmail = user.Email;
        }

        public void Complete()
        {
            Status = TaskStatus.Completed;
        }
    }
}