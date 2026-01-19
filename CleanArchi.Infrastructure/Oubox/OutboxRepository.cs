using CleanArchi.Infrastructure.Persistence.Outbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchi.Infrastructure.Oubox
{
    public class OutboxRepository : IOutboxRepository
    {
        public Task<List<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsProcessedAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
