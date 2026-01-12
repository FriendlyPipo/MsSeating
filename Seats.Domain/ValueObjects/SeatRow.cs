using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public sealed class SeatRow
    {
        private SeatRow(int value) => Value = value;

        public static SeatRow Create(int value)
        {
            if (value < 0)
                throw new ArgumentException("Row must be a non-negative integer", nameof(value));

            return new SeatRow(value);
        }

        public int Value { get; init; }
        public override string ToString() => Value.ToString();

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not SeatRow other)
                return false;
            return Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}