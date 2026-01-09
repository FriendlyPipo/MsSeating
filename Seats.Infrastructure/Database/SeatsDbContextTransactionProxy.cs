using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Seats.Core.Database;
using System.Diagnostics.CodeAnalysis;

namespace Seats.Infrastructure.Database
{
    [ExcludeFromCodeCoverage]
    public class SeatsDbContextTransactionProxy : ISeatsDbContextTransactionProxy
    {
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        public SeatsDbContextTransactionProxy(DbContext context)
        {
            _transaction = context.Database.BeginTransaction();
        }

        public void Commit() => _transaction.Commit();

        public void Rollback() => _transaction.Rollback();

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
