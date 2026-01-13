using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct VenueId
    {
        public Guid Value { get; }
        private VenueId(Guid value) => Value = value;

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
