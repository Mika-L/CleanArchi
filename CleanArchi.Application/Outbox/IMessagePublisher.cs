namespace CleanArchi.Application.Outbox
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default)
            where T : class;
    }
}
