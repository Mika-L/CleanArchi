using CleanArchi.Application.Common.Interfaces;

namespace CleanArchi.Infrastructure.Persistence.EF
{
    public class UnitOfWorkEF : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWorkEF(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}