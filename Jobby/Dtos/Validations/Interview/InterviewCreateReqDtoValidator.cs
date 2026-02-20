using FluentValidation;
using Jobby.Dtos.Interviews;

namespace Jobby.Dtos.Validations.Interview
{
    public class InterviewCreateReqDtoValidator : AbstractValidator<InterviewCreateReqDto>
    {
        public InterviewCreateReqDtoValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty();

            RuleFor(x => x.Stage)
                .IsInEnum();

            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .Must(BeInFutureOrNow)
                .WithMessage("StartsAt must be now or in the future.");

            RuleFor(x => x.EndsAt)
                .NotEmpty()
                .GreaterThan(x => x.StartsAt)
                .WithMessage("EndsAt must be after StartsAt.");

            RuleFor(x => x.Location)
                .MaximumLength(200);

            RuleFor(x => x.MeetingUrl)
                .MaximumLength(500)
                .Must(BeValidUrlOrNull)
                .WithMessage("MeetingUrl must be a valid URL (http/https).");
        }

        private static bool BeValidUrlOrNull(string? url)
            => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out var u) &&
               (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);

        private static bool BeInFutureOrNow(DateTimeOffset dt)
            => dt >= DateTimeOffset.UtcNow.AddMinutes(-1); // small grace window
    }
}
