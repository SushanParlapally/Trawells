using FluentValidation;
using TravelDesk.DTOs;

namespace TravelDesk.Validators
{
    /// <summary>
    /// Validator for LoginDto with security-focused validation
    /// </summary>
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .Length(5, 100).WithMessage("Email must be between 5 and 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(1).WithMessage("Password cannot be empty")
                .MaximumLength(100).WithMessage("Password is too long");
        }
    }
}