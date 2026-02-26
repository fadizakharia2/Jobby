namespace Jobby.Dtos.Auth
{
    public record UserResponseDto(
           string FirstName,
           string LastName,
           bool EnableNotifications,
           bool IsActive ,
           DateTimeOffset CreatedAt,
           DateTimeOffset UpdatedAt 
        );
}
