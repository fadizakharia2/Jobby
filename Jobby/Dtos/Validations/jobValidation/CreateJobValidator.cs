using System.Globalization;
using FluentValidation;
using Jobby.Data.enums;
using Jobby.Dtos.jobs;

namespace Jobby.Dtos.Validations.jobValidation
{
    public class CreateJobValidator : AbstractValidator<JobsCreateRequestDto>
    {
        public CreateJobValidator()
        {
            RuleFor(x => x.Title).NotNull().MinimumLength(4).MaximumLength(100);
            RuleFor(x => x.Description).MinimumLength(25).MaximumLength(500);
            RuleFor(x => x.SalaryMin)
            .GreaterThan(0)
            .When(x => x.SalaryMin.HasValue);
            RuleFor(x => x.SalaryMax)
                .GreaterThan(0)
                .When(x => x.SalaryMax.HasValue);
            RuleFor(x => x.EmploymentType).NotNull().IsInEnum();
            RuleFor(x => x.Currency).NotEmpty()
            .Length(3)
            .Must(BeValidIsoCurrency)
            .WithMessage("Invalid currency code (ISO 4217)"); ;
            RuleFor(x => x.LocationType)
            .IsInEnum();

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Location));

            RuleFor(x => x.Location)
                .NotEmpty()
                .When(x => x.LocationType != JobLocationType.Remote)
                .WithMessage("Location is required for onsite or hybrid jobs");

            RuleFor(x => x.Location)
                .Empty()
                .When(x => x.LocationType == JobLocationType.Remote)
                .WithMessage("Location must be empty for remote jobs");

           RuleFor(x=>x.SalaryMax).GreaterThanOrEqualTo(x=>x.SalaryMin).When(x=>x.SalaryMin.HasValue &&  x.SalaryMax.HasValue);

        }
        private static bool BeValidIsoCurrency(string? currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return false;

            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(c =>
                {
                    try
                    {
                        return new RegionInfo(c.Name);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(r => r != null)
                .Any(r => r!.ISOCurrencySymbol.Equals(currency, StringComparison.OrdinalIgnoreCase));
        }
    }
}
