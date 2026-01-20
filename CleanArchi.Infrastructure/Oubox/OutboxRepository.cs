using CleanArchi.Infrastructure.Persistence.EF;
using CleanArchi.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchi.Infrastructure.Oubox
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(
            ApplicationDbContext context,
            ILogger<OutboxRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OutboxMessage>> GetUnprocessedAsync(
            int batchSize,
            CancellationToken ct)
        {
            try
            {
                var messages = await _context.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .OrderBy(m => m.OccurredOn)
                    .Take(batchSize)
                    .ToListAsync(ct);

                _logger.LogInformation(
                    "Retrieved {Count} unprocessed outbox messages",
                    messages.Count);

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unprocessed outbox messages");
                throw;
            }
        }

        public async Task MarkAsProcessedAsync(Guid id, CancellationToken ct)
        {
            try
            {
                var rowsAffected = await _context.OutboxMessages
                    .Where(m => m.Id == id)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(m => m.ProcessedOn, DateTime.UtcNow),
                        ct);

                if (rowsAffected == 0)
                {
                    _logger.LogWarning(
                        "Outbox message {MessageId} not found for processing",
                        id);
                }
                else
                {
                    _logger.LogInformation(
                        "Marked outbox message {MessageId} as processed",
                        id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error marking outbox message {MessageId} as processed",
                    id);
                throw;
            }
        }


        public async Task MarkAsFailedAsync(
           Guid id,
           string error,
           CancellationToken ct)
        {
            try
            {
                var rowsAffected = await _context.OutboxMessages
                    .Where(m => m.Id == id)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(m => m.Error, error)
                            .SetProperty(m => m.RetryCount, m => m.RetryCount + 1)
                            .SetProperty(m => m.LastRetryOn, DateTime.UtcNow),
                        ct);

                if (rowsAffected == 0)
                {
                    _logger.LogWarning(
                        "Outbox message {MessageId} not found for failure marking",
                        id);
                }
                else
                {
                    _logger.LogWarning(
                        "Marked outbox message {MessageId} as failed: {Error}",
                        id,
                        error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error marking outbox message {MessageId} as failed",
                    id);
                throw;
            }
        }
    }
}