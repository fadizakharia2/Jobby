using System.ComponentModel.DataAnnotations;
using Jobby.Data.entities;

namespace Jobby.Dtos.OrganizationDtos
{
    public class OrganizationDtos
    {

    // Requests
    public record CreateOrganizationRequest(string Name, string Slug);
    public record UpdateOrganizationRequest(Guid Id, string Name, string Slug);

    // Responses
    public record OrganizationDto(
        Guid Id,
        string Name,
        string Slug,
        Guid CreatedByUserId,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );
}
}
