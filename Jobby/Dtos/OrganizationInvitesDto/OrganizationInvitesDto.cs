using System.ComponentModel.DataAnnotations;
using Jobby.Data.entities;

namespace Jobby.Dtos.OrganizationMembersDto
{
    public record OrganizationInvitesDto (
     Guid Id,
     Guid OrganizationId,

     string Email,
    string InvitedRole ,
    // Store hashed token (not the raw token)
     //string TokenHash,
     DateTime ExpiresAt ,
    DateTime? AcceptedAt,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTimeOffset JoinedAt 
      );
    public record OrganizationInvitesListDto
    (OrganizationInvitesDto[] data,
     int pageNumber,
     int pageLimit,
     int total
     );

    public record CreateInviteRequestDto(
        string Email,
       string InvitedRole
        );
    public record AcceptInviteRequestDto(
       string token
        );
    public record DeclineInviteRequestDto(
        string token
        );
}
