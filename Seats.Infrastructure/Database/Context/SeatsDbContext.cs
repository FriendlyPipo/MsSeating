using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Seats.Core.Database;
using Seats.Domain.Entities;
using Seats.Domain.Primitives;

namespace Seats.Infrastructure.Database.Context
{
    public class SeatsDbContext : DbContext, ISeatsDbContext, IUnitOfWork 
    {
        public SeatsDbContext(DbContextOptions<SeatsDbContext> options)
            : base(options)
        {
        }

        public DbContext DbContext => this;

        public DbSet<Seat> Seat { get; set; } = null!;

        public ISeatsDbContextTransactionProxy BeginTransaction()
        {
            return new SeatsDbContextTransactionProxy(this);
        }

        public void ChangeEntityState<TEntity>(TEntity entity, EntityState state)
        {
            if (entity != null)
            {
                Entry(entity).State = state;
            }
        }

        public async Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is AggregateRoot && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (AggregateRoot)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedBy = user;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.UpdatedBy = user;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeatsDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
