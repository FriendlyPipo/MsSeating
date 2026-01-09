using System.Diagnostics.CodeAnalysis;

namespace Seats.Domain.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public readonly struct FunctionId
    {
        public Guid Value { get; }
        private FunctionId(Guid value) => Value = value;

        public static FunctionId Create(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentException("El ID de función no puede estar vacío.");
            return new FunctionId(value);
        }

        public override string ToString() => Value.ToString();
    }
}