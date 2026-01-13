using System;

namespace Seats.Core.Exceptions
{
    public class EventZoneException : Exception
    {
        public EventZoneException(string message) : base(message)
        {
        }

        public EventZoneException(Guid zoneId) 
            : base($"La zona con ID {zoneId} no existe o no se pudo recuperar.")
        {
        }

        public EventZoneException(int capacity)
            : base($"No se puede crear el asiento. La capacidad de la zona ({capacity}) ha sido alcanzada.")
        {
        }
    }
}
