using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seats.Domain.Entities;
using Seats.Domain.ValueObjects;

namespace Seats.Infrastructure.Database.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("seats");

            builder.HasKey(s => new { s.SeatId, s.EventId, s.FunctionId, s.ZoneId, s.VenueId });

            builder.Property(s => s.SeatId)
                .HasConversion(id => id.Value, value => SeatId.Create(value))
                .IsRequired();

            builder.Property(s => s.EventId)
                .HasConversion(id => id.Value, value => EventId.Create(value))
                .IsRequired();

            builder.Property(s => s.FunctionId)
                .HasConversion(id => id.Value, value => FunctionId.Create(value))
                .IsRequired();

            builder.Property(s => s.ZoneId)
                .HasConversion(id => id.Value, value => ZoneId.Create(value))
                .IsRequired();

            builder.Property(s => s.VenueId)
                .HasConversion(id => id.Value, value => VenueId.Create(value))
                .IsRequired();

            builder.Property(s => s.UserId)
                .HasConversion(id => id.HasValue ? id.Value.Value : (Guid?)null, value => value.HasValue ? UserId.Create(value.Value) : null)
                .IsRequired(false);

            builder.Property(s => s.Row)
                .HasConversion(row => row.Value, value => SeatRow.Create(value))
                .IsRequired();

            builder.Property(s => s.Number)
                .HasConversion(num => num.Value, value => SeatNumber.Create(value))
                .IsRequired();

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();
                
            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.CreatedBy)
                .IsRequired(false);

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            builder.Property(s => s.UpdatedBy)
                .IsRequired(false);

            builder.Property(s => s.IsDeleted)
                .IsRequired();

            builder.HasIndex(s => new { s.FunctionId, s.ZoneId });
            builder.HasIndex(s => s.EventId);
        }
    }
}
