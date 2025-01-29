using System;
using FluentValidation.Results;

namespace ATTM.MeetingApp.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> ValidationErrors { get; set; }

        public ValidationException(ValidationResult result)
        {
            ValidationErrors = new List<string>();
            foreach (var error in result.Errors)
            {
                ValidationErrors.Add(error.ErrorMessage);
            }
        }
    }
}
