using CleanArchi.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CleanArchi.Infrastructure.Persistence.Dapper
{
    public class UnitOfWorkDapper : IUnitOfWork, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _transaction;

        public UnitOfWorkDapper(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            _transaction.Commit();
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
