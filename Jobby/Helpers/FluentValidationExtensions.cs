using FluentValidation.Results;

namespace Jobby.Helpers
{
    public static class FluentValidationExtensions
    {
        public static Dictionary<string, string[]> ToDictionary(this ValidationResult result) =>
        result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
