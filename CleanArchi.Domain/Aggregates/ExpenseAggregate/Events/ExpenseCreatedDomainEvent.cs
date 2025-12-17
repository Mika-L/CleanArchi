using CleanArchi.Domain.Common;

namespace CleanArchi.Domain.Events
{
    public record ExpenseCreatedDomainEvent(Guid ExpenseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
