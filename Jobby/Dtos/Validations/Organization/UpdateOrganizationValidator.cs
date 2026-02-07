using FluentValidation;
using Jobby.Data.context;
using Microsoft.EntityFrameworkCore;
using static Jobby.Dtos.OrganizationDtos.OrganizationDtos;

namespace Jobby.Dtos.Validations.Organization
{
    public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationRequest>
    {
        public UpdateOrganizationValidator(AppDbContext db)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Organization id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(100).WithMessage("Organization name must not exceed 100 characters.")
                // ✅ unique name excluding this org
                .MustAsync(async (req, name, ct) =>
                {
                    return !await db.Organizations
                        .AnyAsync(o => o.Name == name && o.Id != req.Id, ct);
                })
                .WithMessage("An organization with the same name already exists.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.")
                .Matches(@"^(?!-)(?!.*--)[a-z0-9-]{1,200}(?<!-)$")
                .WithMessage("Slug must be lowercase letters, numbers, or hyphens, and cannot start or end with a hyphen.")
                // ✅ unique slug excluding this org
                .MustAsync(async (req, slug, ct) =>
                {
                    return !await db.Organizations
                        .AnyAsync(o => o.Slug == slug && o.Id != req.Id, ct);
                })
                .WithMessage("An organization with the same slug already exists.");

            // ✅ optional: ensure org exists
            RuleFor(x => x)
                .MustAsync(async (req, ct) =>
                    await db.Organizations.AnyAsync(o => o.Id == req.Id, ct))
                .WithMessage("Organization not found.");

        }
    }
}
