using DTO.DTO.Learner;
using FluentValidation;

namespace DTO.Validation
{
    public class CreateLearnerValidator : AbstractValidator<CreateLearnerDTO>
    {
        public CreateLearnerValidator()
        {
            RuleFor(l => l.FullName)
                .NotEmpty().WithMessage("Full name is required.");

            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(l => l.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Length(14).WithMessage("National ID must be exactly 14 characters long.")
                .Matches(@"^\d+$").WithMessage("National ID must contain only digits.");

            RuleFor(l => l.Department)
                .NotEmpty().WithMessage("Department specification is required.");
        }
    }

}