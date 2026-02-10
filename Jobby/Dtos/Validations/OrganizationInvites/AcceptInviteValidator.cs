using FluentValidation;
using Jobby.Dtos.OrganizationMembersDto;

namespace Jobby.Dtos.Validations.OrganizationInvites
{
    public class AcceptInviteValidator:AbstractValidator<AcceptInviteRequestDto>
    {
        public AcceptInviteValidator() { 
        RuleFor(x => x.token)
            .NotEmpty().WithMessage("Token is required.");
        }
    }
}
