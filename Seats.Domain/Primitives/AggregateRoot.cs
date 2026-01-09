namespace Seats.Domain.Primitives
{
    public abstract class AggregateRoot
    {
        // Metadatos comunes (audit)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Contenedor de Domain Events
        private readonly List<DomainEvent> _domainEvents = new();

        // Lista de eventos de dominio
        public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

        // Registra un evento de dominio
        protected void Raise(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        // Limpia los eventos
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
