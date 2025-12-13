using CleanArchi.Domain.Repositories;

namespace CleanArchi.Domain.Common
{
    public interface IUnitOfWork
    {
        IExpenseRepository Expenses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
