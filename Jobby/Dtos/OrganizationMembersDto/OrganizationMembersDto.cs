using System.ComponentModel.DataAnnotations;
using Jobby.Data.entities;

namespace Jobby.Dtos.OrganizationMembersDto
{
    public record OrganizationMemberDto(
     Guid Id,
   Guid OrganizationId,
    Guid UserId ,
   string Role ,
   DateTimeOffset JoinedAt 
      );
    public record CeateInviteRequestDto(
        string email,
       string role
        );
    public record AcceptInviteRequestDto(
       string token
        );
    public record DeclineInviteRequestDto(
        string token
        );
}
