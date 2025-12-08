using CleanArchi.Domain.Entities;

namespace CleanArchi.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<User> Users { get; }
        IQueryable<Expense> Expenses { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
