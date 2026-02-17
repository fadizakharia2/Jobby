using FluentValidation;
using Jobby.Dtos.Application;

namespace Jobby.Dtos.Validations.ApplicationValidation
{
    public class ApplicationCreateValidation:AbstractValidator<ApplicationCreateDto>
    {
        public ApplicationCreateValidation() {
            RuleFor(x => x.Source).IsInEnum();
            RuleFor(x => x.CoverLetter).MinimumLength(25).MaximumLength(500);
        }
    }
}
