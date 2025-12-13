using CleanArchi.Domain.Entities;
using CleanArchi.Domain.Repositories;

namespace CleanArchi.Infrastructure.Persistence.Dapper.Repositorie
{
    public class ExpenseRepositoryDapper : IExpenseRepository
    {
        private readonly UnitOfWorkDapper _uow;

        public ExpenseRepositoryDapper(UnitOfWorkDapper uow)
        {
            _uow = uow;
        }

        public Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
