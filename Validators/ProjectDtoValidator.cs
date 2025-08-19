using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class ProjectDtoValidator : AbstractValidator<ProjectDto>
    {
        public ProjectDtoValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");
        }
    }
}