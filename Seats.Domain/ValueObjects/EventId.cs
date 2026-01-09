using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct EventId
    {
        public Guid Value { get; }
        private EventId(Guid value) => Value = value;

        public static EventId Create(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("El ID del evento no puede estar vacío.");
            return new EventId(value);
        }

        public override string ToString() => Value.ToString();
    }
}