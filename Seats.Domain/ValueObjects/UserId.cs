using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct UserId
    {
        public Guid Value { get; }
        private UserId(Guid value) => Value = value;

        public static UserId Create(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("El ID de usuario no puede estar vacío.");
            return new UserId(value);
        }
        public override string ToString() => Value.ToString();
    }
}