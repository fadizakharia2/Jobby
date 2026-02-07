namespace Jobby.Dtos.Auth
{
    public record RegisterRequestDto(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );

}
