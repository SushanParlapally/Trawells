using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class DepartmentDtoValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentDtoValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
        }
    }
}