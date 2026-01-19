using CleanArchi.Infrastructure.Persistence.Outbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchi.Infrastructure.Oubox
{
    public interface IOutboxRepository
    {
        Task<List<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken ct);
        Task MarkAsProcessedAsync(Guid id, CancellationToken ct);
        Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct);
    }
}
