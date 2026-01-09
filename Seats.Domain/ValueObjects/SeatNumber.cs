using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public sealed class SeatNumber
    {
        private SeatNumber(int value) => Value = value;

        public static SeatNumber Create(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Number must be greater than 0", nameof(value));

            return new SeatNumber(value);
        }

        public int Value { get; init; }
        public override string ToString() => Value.ToString();
    }
}