namespace Jobby.Dtos.Auth
{
    public record AuthResponseDto(
        string AccessToken,
        string RefreshToken,
        DateTimeOffset ExpiresAt
        );
}
