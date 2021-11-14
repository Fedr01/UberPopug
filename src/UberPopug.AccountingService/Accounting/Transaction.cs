using System;
using UberPopug.AccountingService.Tasks;
using UberPopug.AccountingService.Users;

namespace UberPopug.AccountingService.Accounting
{
    public class Transaction
    {
        public Transaction(string userEmail, int taskId, decimal amount, TransactionType type)
        {
            PublicId = Guid.NewGuid();
            UserEmail = userEmail;
            Amount = amount;
            DateTime = DateTime.UtcNow;
            TaskId = taskId;
            Type = type;
        }

        public int Id { get; private set; }

        public Guid PublicId { get; private set; }

        public DateTime DateTime { get; private set; }

        public TransactionType Type { get; private set; }

        public User User { get; private set; }
        
        public string UserEmail { get; private set; }
        
        public TrackerTask Task { get; private set; }
        
        public int TaskId { get; private set; }
        
        public decimal Amount { get; private set; }
    }
}