using Jobby.Data.entities;

namespace Jobby.Auth
{
    public interface ITokenService
    {
        Task<(string accessToken, DateTimeOffset accessExp)> CreateAccessTokenAsync(User user);
        string CreateRefreshToken();
        string HashRefreshToken(string refreshToken);
    }
}
