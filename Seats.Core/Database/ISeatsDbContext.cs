using Microsoft.EntityFrameworkCore;
using Seats.Domain.Entities;

namespace Seats.Core.Database
{
    public interface ISeatsDbContext
    {
        DbContext DbContext { get; }

        DbSet<Seat> Seat { get; set; }

        ISeatsDbContextTransactionProxy BeginTransaction();

        void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default);
    }
}
