using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public sealed class VenueId
    {
        private VenueId(Guid value) => Value = value;

        public Guid Value { get; init; }

        public static VenueId Create(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("El ID del venue no puede estar vacío.");

            return new VenueId(value);
        }

        public static VenueId New() => new VenueId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
