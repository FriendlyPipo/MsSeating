namespace Seats.Core.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
