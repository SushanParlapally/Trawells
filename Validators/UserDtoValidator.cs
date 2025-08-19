using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Department)
                .SetValidator(new DepartmentDtoValidator());
        }
    }
}