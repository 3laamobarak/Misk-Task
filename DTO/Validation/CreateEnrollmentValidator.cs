using DTO.DTO.Enrollment;
using FluentValidation;

namespace DTO.Validation
{
    public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentDTO>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(e => e.LearnerId)
                .GreaterThan(0).WithMessage("Valid Learner ID is required.");

            RuleFor(e => e.CourseId)
                .GreaterThan(0).WithMessage("Valid Course ID is required.");
        }
    }
}