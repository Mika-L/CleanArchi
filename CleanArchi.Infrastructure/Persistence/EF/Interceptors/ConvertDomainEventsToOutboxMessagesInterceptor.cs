using CleanArchi.Domain.Common;
using CleanArchi.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchi.Infrastructure.Persistence.EF.Interceptors
{
    public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
    {

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            dbContext.ChangeTracker.Entries()
                .Select(e => e.Entity)
                .OfType<AggregateRoot>()
                .SelectMany(ag =>
                {
                    var domainEvents = ag.GetDomainEvents();
                    ag.ClearDomainEvents();
                    return domainEvents;
                })
                .ToList()
                .ForEach(domainEvent =>
                {
                    var outboxMessage = new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = domainEvent.GetType().FullName ?? string.Empty,
                        Content = System.Text.Json.JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                        OccurredOn = DateTime.UtcNow
                    };
                    dbContext.Set<OutboxMessage>().Add(outboxMessage);
                });

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
