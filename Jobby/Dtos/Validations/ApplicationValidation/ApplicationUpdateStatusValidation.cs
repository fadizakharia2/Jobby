using FluentValidation;
using Jobby.Data.enums;
using Jobby.Dtos.Application;

namespace Jobby.Dtos.Validations.ApplicationValidation
{
    public class ApplicationUpdateStatusValidation : AbstractValidator<UpdateApplicationStatusDto>
    {
        public ApplicationUpdateStatusValidation() { 
        RuleFor(x => x.Status).NotEmpty().IsInEnum().NotEqual(ApplicationStatus.Interview);
        }
    }
}
