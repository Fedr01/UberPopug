using System;
using UberPopug.TaskTrackerService.Users;


namespace UberPopug.TaskTrackerService.Tasks
{
    public class TrackerTask
    {
        public TrackerTask(string title, string jiraId)
        {
            if (title.Contains("[") || title.Contains("]"))
            {
                throw new ArgumentException("Task title should not contain Jira Id");
            }

            Title = title;
            JiraId = jiraId;
            PublicId = Guid.NewGuid();
            Status = TaskStatus.PtichkaVKletke;
        }

        public int Id { get; private set; }

        public Guid PublicId { get; private set; }

        public string Title { get; private set; }

        public string JiraId { get; private set; }

        public TaskStatus Status { get; private set; }

        public User User { get; private set; }

        public string AssignedToEmail { get; private set; }

        public void AssignTo(User user)
        {
            AssignedToEmail = user.Email;
        }

        public void Complete()
        {
            Status = TaskStatus.ProsoVMiske;
        }
    }
}