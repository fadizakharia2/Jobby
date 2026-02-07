using System.Text.RegularExpressions;
using FluentValidation;
using Jobby.Data.context;
using Microsoft.EntityFrameworkCore;
using static Jobby.Dtos.OrganizationDtos.OrganizationDtos;

namespace Jobby.Dtos.Validations.Organization
{
    public class CreateOrganizationValidator : AbstractValidator<CreateOrganizationRequest>
    {
        public CreateOrganizationValidator(AppDbContext db)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(100).WithMessage("Organization name must not exceed 100 characters.")
                .MustAsync(async (name, cancellation) =>
                {
                    return !await db.Organizations.AnyAsync(o => o.Name == name, cancellation);
                }).WithMessage("An organization with the same name already exists.");

            RuleFor(x => x.Slug)
                .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.")
                .Matches(new Regex(@"^(?!-)(?!.*--)[a-z0-9-]{1,200}(?<!-)$", RegexOptions.Compiled))
                .MustAsync(async (slug,cancellation)=>!await db.Organizations.AnyAsync(o => o.Slug == slug,cancellation))
                .WithMessage("An organization with the same slug already exists.");
        }
    }
}
