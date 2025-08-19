using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class TravelRequestDtoValidator : AbstractValidator<TravelRequestDto>
    {
        public TravelRequestDtoValidator()
        {
            RuleFor(x => x.ReasonForTravel)
                .NotEmpty().WithMessage("Reason for travel is required.")
                .MaximumLength(500).WithMessage("Reason for travel cannot exceed 500 characters.");

            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage("From date is required.")
                .LessThanOrEqualTo(x => x.ToDate).WithMessage("From date must be before or equal to To date.");

            RuleFor(x => x.ToDate)
                .NotEmpty().WithMessage("To date is required.")
                .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("To date must be after or equal to From date.");

            RuleFor(x => x.FromLocation)
                .NotEmpty().WithMessage("From location is required.")
                .MaximumLength(100).WithMessage("From location cannot exceed 100 characters.");

            RuleFor(x => x.ToLocation)
                .NotEmpty().WithMessage("To location is required.")
                .MaximumLength(100).WithMessage("To location cannot exceed 100 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.");

            RuleFor(x => x.User)
                .SetValidator(new UserDtoValidator());

            RuleFor(x => x.Project)
                .SetValidator(new ProjectDtoValidator());
        }
    }
}