using CleanArchi.Domain.Entities;
using CleanArchi.Domain.Repositories;
using CleanArchi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleanArchi.Infrastructure.Data.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Expenses.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await _context.Expenses.AddAsync(expense, cancellationToken);
        }

        public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            _context.Expenses.Update(expense);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var expense = await GetByIdAsync(id, cancellationToken);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }
        }
    }
}
