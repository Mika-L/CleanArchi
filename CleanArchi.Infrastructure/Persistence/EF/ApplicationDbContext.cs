using CleanArchi.Domain.Aggregates.ExpenseAggregate;
using CleanArchi.Domain.Entities;
using CleanArchi.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CleanArchi.Infrastructure.Persistence.EF
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Expense> Expenses => Set<Expense>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            // use migration instead
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Date).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.OccurredOn).IsRequired();
                entity.Property(e => e.ProcessedOn).IsRequired(false);
                entity.Property(e => e.Error).IsRequired(false).HasMaxLength(2000);
            });
        }
    }
}
