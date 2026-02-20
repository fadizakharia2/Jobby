using FluentValidation;
using Jobby.Dtos.Interviews;

namespace Jobby.Dtos.Validations.Interview
{
    public class InterviewCancelReqDtoValidator : AbstractValidator<InterviewCancelReqDto>
    {
        public InterviewCancelReqDtoValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
