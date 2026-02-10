using FluentValidation;
using Jobby.Data.context;
using Jobby.Dtos.OrganizationMembersDto;

namespace Jobby.Dtos.Validations.OrganizationInvites
{
    public class CreateInviteValidator:AbstractValidator<CreateInviteRequestDto>
    {
        public CreateInviteValidator(AppDbContext db)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.InvitedRole)
                .NotEmpty().WithMessage("Invited role is required.")
                .Must(role => new[] { "ADMIN", "RECRUITER" }.Contains(role))
                .WithMessage("Invited role must be either 'ADMIN' or 'RECRUITER'.");
        }
    }
}
