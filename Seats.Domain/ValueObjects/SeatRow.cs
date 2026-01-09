using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public sealed class SeatRow
    {
        private SeatRow(string value) => Value = value;

        public static SeatRow Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Row is required", nameof(value));

            return new SeatRow(value);
        }

        public string Value { get; init; }
        public override string ToString() => Value;
    }
}