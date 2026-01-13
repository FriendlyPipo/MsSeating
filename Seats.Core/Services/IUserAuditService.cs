using Seats.Core.Dtos;

namespace Seats.Core.Services
{
    public interface IUserAuditService
    {
        Task AuditAsync(UserAuditDto auditDto);
    }
}
