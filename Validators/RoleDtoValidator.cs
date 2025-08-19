using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class RoleDtoValidator : AbstractValidator<RoleDto>
    {
        public RoleDtoValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
        }
    }
}