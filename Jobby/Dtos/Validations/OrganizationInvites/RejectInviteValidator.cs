using FluentValidation;
using Jobby.Dtos.OrganizationMembersDto;

namespace Jobby.Dtos.Validations.OrganizationInvites
{
    public class RejectInviteValidator:AbstractValidator<DeclineInviteRequestDto>
    {
        public RejectInviteValidator() { 
        RuleFor(x=>x.token)
            .NotEmpty().WithMessage("Token is required.");
        }
    }
}
