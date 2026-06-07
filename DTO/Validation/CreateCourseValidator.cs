using DTO.DTO.Course;
using FluentValidation;

namespace DTO.Validation
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDTo>
    {
        public CreateCourseValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Course title is required.")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

            RuleFor(c => c.DurationHours)
                .GreaterThan(0).WithMessage("Duration hours must be greater than 0.");
        }
    }
}