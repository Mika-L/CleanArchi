using CleanArchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchi.Infrastructure.Persistence.EF
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> User;

        public DbSet<Expense> Expenses;

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
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
        }
    }
}
