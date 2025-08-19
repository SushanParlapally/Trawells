using FluentValidation;
using TravelDesk.DTOs;

namespace TravelDesk.Validators
{
    /// <summary>
    /// Validator for UserCreateDto with comprehensive business rules
    /// </summary>
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
                .Matches("^[a-zA-Z\\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.LastName)
                .Length(0, 50).WithMessage("Last name cannot exceed 50 characters")
                .Matches("^[a-zA-Z\\s'-]*$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .Length(5, 100).WithMessage("Email must be between 5 and 100 characters");

            RuleFor(x => x.MobileNum)
                .NotEmpty().WithMessage("Mobile number is required")
                .Matches(@"^[\+]?[1-9][\d]{0,15}$").WithMessage("Please provide a valid mobile number")
                .Length(10, 20).WithMessage("Mobile number must be between 10 and 20 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .Length(10, 200).WithMessage("Address must be between 10 and 200 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Please select a valid role");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Please select a valid department");

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithMessage("Please select a valid manager")
                .When(x => x.ManagerId.HasValue);
        }
    }
}