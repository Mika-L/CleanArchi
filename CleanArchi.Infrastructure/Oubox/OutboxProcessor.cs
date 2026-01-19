using CleanArchi.Application.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArchi.Infrastructure.Oubox
{
    public sealed class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                    var messages = await repo.GetUnprocessedAsync(20, stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            await publisher.PublishAsync(message, stoppingToken);
                            await repo.MarkAsProcessedAsync(message.Id, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to publish OutboxMessage {Id}", message.Id);
                            await repo.MarkAsFailedAsync(message.Id, ex.Message, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Outbox processor crashed");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
