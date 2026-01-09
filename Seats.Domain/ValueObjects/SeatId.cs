using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct SeatId
    {
        public Guid Value { get; }
        private SeatId(Guid value) => Value = value;

        public static SeatId Create(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("SeatId cannot be empty.");
            return new SeatId(value);
        }

        public static SeatId New() => new SeatId(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}