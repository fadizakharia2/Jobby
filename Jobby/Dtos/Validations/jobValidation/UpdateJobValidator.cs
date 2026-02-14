using System.Globalization;
using FluentValidation;
using Jobby.Data.enums;
using Jobby.Dtos.jobs;

namespace Jobby.Dtos.Validations.jobValidation
{
    public class UpdateJobValidator:AbstractValidator<JobsUpdateRequestDto>
    {
        public  UpdateJobValidator()
        {
            // Title (only validate when provided)
            RuleFor(x => x.Title)
                .MinimumLength(4)
                .MaximumLength(100)
                .When(x => x.Title != null);

            // Description (only validate when provided)
            RuleFor(x => x.Description)
                .MinimumLength(25)
                .MaximumLength(500)
                .When(x => x.Description != null);

            // EmploymentType (only validate when provided)
            RuleFor(x => x.EmploymentType)
                .IsInEnum()
                .When(x => x.EmploymentType.HasValue);

            // LocationType (only validate when provided)
            RuleFor(x => x.LocationType)
                .IsInEnum()
                .When(x => x.LocationType.HasValue);

            // Location length (only when provided)
            RuleFor(x => x.Location)
                .MaximumLength(200)
                .When(x => x.Location != null);

            // Salary rules (nullable)
            RuleFor(x => x.SalaryMin)
                .GreaterThan(0)
                .When(x => x.SalaryMin.HasValue);

            RuleFor(x => x.SalaryMax)
                .GreaterThan(0)
                .When(x => x.SalaryMax.HasValue);

            RuleFor(x => x.SalaryMax)
                .GreaterThanOrEqualTo(x => x.SalaryMin!.Value)
                .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue);

            // If one salary is provided, require the other (PATCH-safe)
            RuleFor(x => x.SalaryMin)
                .NotNull()
                .When(x => x.SalaryMax.HasValue);

            RuleFor(x => x.SalaryMax)
                .NotNull()
                .When(x => x.SalaryMin.HasValue);

            // Currency:
            // - Validate format only if provided
            RuleFor(x => x.Currency)
                .Length(3)
                .Must(BeValidIsoCurrency)
                .When(x => !string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage("Invalid currency code (ISO 4217)");

            // - Require currency if salary is being set
            RuleFor(x => x.Currency)
                .NotEmpty()
                .When(x => x.SalaryMin.HasValue || x.SalaryMax.HasValue)
                .WithMessage("Currency is required when salary is specified");

            // Location business rule (only enforce when updating either field)
            RuleFor(x => x)
                .Must(x =>
                    x.LocationType == null && x.Location == null
                    || (x.LocationType == JobLocationType.Remote
                        ? string.IsNullOrWhiteSpace(x.Location)
                        : !string.IsNullOrWhiteSpace(x.Location))
                )
                .When(x => x.LocationType.HasValue || x.Location != null)
                .WithMessage("Location must be empty for remote jobs and required for onsite or hybrid jobs");
        }
        private static bool BeValidIsoCurrency(string? currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return false;

            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(c => new RegionInfo(c.LCID))
                .Any(r => r.ISOCurrencySymbol == currency.ToUpperInvariant());
        }
    }
}
