using CleanArchi.Domain.Entities;

namespace CleanArchi.Domain.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
        Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
