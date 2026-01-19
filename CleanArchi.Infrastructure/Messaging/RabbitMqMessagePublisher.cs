using CleanArchi.Application.Outbox;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CleanArchi.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = "events-exchange";
        public string ExchangeType { get; set; } = "topic";
    }

    public sealed class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqMessagePublisher> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public RabbitMqMessagePublisher(
            IOptions<RabbitMqSettings> settings,
            ILogger<RabbitMqMessagePublisher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }


        public async Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqMessagePublisher));

            await EnsureConnectionAsync();

            try
            {
                var messageType = message.GetType();
                var routingKey = GetRoutingKey(messageType);

                var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    Type = messageType.Name,
                    MessageId = Guid.NewGuid().ToString(), // message.Id for idempotency ?
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                    Headers = new Dictionary<string, object?>
                    {
                        ["message-type"] = messageType.FullName!,
                        ["published-at"] = DateTime.UtcNow.ToString("O")
                    }
                };

                await _channel!.BasicPublishAsync(
                     exchange: _settings.ExchangeName,
                     routingKey: routingKey,
                     mandatory: true,
                     basicProperties: properties,
                     body: body);

                _logger.LogInformation(
                    "Published message {MessageType} with routing key {RoutingKey}",
                    messageType.Name,
                    routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
                throw;
            }
        }

        private async Task EnsureConnectionAsync()
        {
            if (_channel != null && _channel.IsOpen)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_channel != null && _channel.IsOpen)
                    return;

                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Déclarer l'exchange
                await _channel.ExchangeDeclareAsync(
                    exchange: _settings.ExchangeName,
                    type: _settings.ExchangeType,
                    durable: true,
                    autoDelete: false);

                _logger.LogInformation(
                    "RabbitMQ connection established to {HostName}:{Port}",
                    _settings.HostName,
                    _settings.Port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        private string GetRoutingKey(Type messageType)
        {
            // Convention: namespace.classname en lowercase
            // Example: Domain.Events.OrderCreatedEvent -> domain.events.ordercreatedevent
            return messageType.FullName?.ToLowerInvariant().Replace(".", "-")
                   ?? messageType.Name.ToLowerInvariant();
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            try
            {
                if (_channel != null)
                    await _channel.CloseAsync();

                if (_connection != null)
                    await _connection.CloseAsync();

                _initLock?.Dispose();

                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disposing RabbitMQ connection");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}