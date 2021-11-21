using System;
using UberPopug.AccountingService.Accounting;
using UberPopug.AccountingService.Users;

namespace UberPopug.AccountingService.Tasks
{
    public class TrackerTask
    {
        public TrackerTask(string title, string jiraId, Guid publicId)
        {
            Title = title;
            JiraId = jiraId;
            PublicId = publicId;
            Status = TaskStatus.PtichkaVKletke;
        }

        public int Id { get; private set; }

        public Guid PublicId { get; private set; }

        public string Title { get; private set; }

        public string JiraId { get; private set; }

        public TaskStatus Status { get; private set; }

        public User Account { get; private set; }

        public string AssignedToEmail { get; private set; }

        public decimal AssignPrice { get; private set; }

        public decimal CompletePrice { get; private set; }

        public void UpdateTitle(string title, string jiraId)
        {
            Title = title;
            JiraId = jiraId;
        }
        
        public void AssignTo(User user)
        {
            if (AssignPrice == 0)
            {
                throw new InvalidOperationException("Failed to assign task, price is 0");
            }
            
            AssignedToEmail = user.Email;
            user.ApplyTransaction(new Transaction(user.Email, Id, AssignPrice, TransactionType.Credit));
        }

        public void Complete()
        {
            if (CompletePrice == 0)
            {
                throw new InvalidOperationException("Failed to complete task, price is 0");
            }
            
            Status = TaskStatus.ProsoVMiske;
            Account.ApplyTransaction(new Transaction(Account.Email, Id, AssignPrice, TransactionType.Debit));
        }

        public void Estimate()
        {
            var random = new Random();
            AssignPrice = random.Next(10, 20);
            CompletePrice = random.Next(20, 40);
        }
    }
}