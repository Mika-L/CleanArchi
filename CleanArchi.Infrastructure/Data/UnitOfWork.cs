using CleanArchi.Domain.Common;
using CleanArchi.Domain.Repositories;
using CleanArchi.Infrastructure.Data.Contexts;
using CleanArchi.Infrastructure.Data.Repositories;

namespace CleanArchi.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IExpenseRepository Expenses { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Expenses = new ExpenseRepository(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
