using FluentValidation;
using Jobby.Dtos.Interviews;

namespace Jobby.Dtos.Validations.Interview
{
    public class InterviewCompleteReqDtoValidator : AbstractValidator<InterviewCompleteReqDto>
    {
        public InterviewCompleteReqDtoValidator()
        {
            RuleFor(x => x.Feedback)
                .MaximumLength(5000); // adjust to what you want
        }
    }
}
