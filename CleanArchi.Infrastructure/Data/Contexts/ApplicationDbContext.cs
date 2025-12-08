using CleanArchi.Application.Common.Interfaces;
using CleanArchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchi.Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<User> UsersDbSet => Set<User>();
        public DbSet<Expense> ExpensesDbSet => Set<Expense>();

        public IQueryable<User> Users => UsersDbSet;
        public IQueryable<Expense> Expenses => ExpensesDbSet;

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

        Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
