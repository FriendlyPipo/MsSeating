using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct ZoneId
    {
        public Guid Value { get; }
        private ZoneId(Guid value) => Value = value;

        public static ZoneId Create(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("El ID de la zona no puede estar vacío.");

            return new ZoneId(value);
        }

        public static ZoneId New() => new ZoneId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
