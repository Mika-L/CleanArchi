namespace CleanArchi.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
