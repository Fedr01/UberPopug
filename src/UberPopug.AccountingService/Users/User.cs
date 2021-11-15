using System.Collections.Generic;
using UberPopug.AccountingService.Accounting;

namespace UberPopug.AccountingService.Users
{
    public class User
    {
        public string Email { get; set; }

        public string Role { get; set; }

        public decimal Balance { get; set; }

        public List<Transaction> Transactions { get; private set; } = new();

        public void ApplyTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            Balance = transaction.Type == TransactionType.Credit
                ? Balance -= transaction.Amount
                : Balance += transaction.Amount;
        }
    }
}