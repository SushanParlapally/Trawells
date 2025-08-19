using FluentValidation;
using TravelDesk.DTO;

namespace TravelDesk.Validators
{
    public class CommentDtoValidator : AbstractValidator<CommentDto>
    {
        public CommentDtoValidator()
        {
            RuleFor(x => x.Comments)
                .NotEmpty().WithMessage("Comments cannot be empty.")
                .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters.");
        }
    }
}