using CleanArchi.Domain.Common;
using CleanArchi.Domain.Events;

namespace CleanArchi.Domain.Aggregates.ExpenseAggregate
{
    public class Expense : AggregateRoot
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public Guid UserId { get; private set; }

        private Expense() { } // EF

        public static Expense Create(
            string description,
            decimal amount,
            DateTime date,
            Guid userId)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Description = description,
                Amount = amount,
                Date = date,
                UserId = userId
            };

            expense.Raise(new ExpenseCreatedDomainEvent(
               expense.Id
           ));

            return expense;
        }
    }
}
