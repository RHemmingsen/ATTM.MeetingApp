using FluentValidation.Results;

namespace ATTM.MeetingApp.Application.Exceptions;

public class ValidationException(IEnumerable<ValidationFailure> failures)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyList<string> ValidationErrors { get; } = failures.Select(f => f.ErrorMessage).ToList();
}
