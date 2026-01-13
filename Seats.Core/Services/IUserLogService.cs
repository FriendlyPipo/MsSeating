using Seats.Core.Dtos;

namespace Seats.Core.Services
{
    public interface IUserLogService
    {
        Task LogAsync(UserLogDto logDto);
    }
}
