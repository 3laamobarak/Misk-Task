using System.ComponentModel.DataAnnotations;

namespace DTO.DTO.Learner;

public class CreateLearnerDTO
{
    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "National ID is required.")]
    public string NationalId { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    public string Department { get; set; }

    // Optional: link an Identity ApplicationUser account if created during registration
    public string? ApplicationUserId { get; set; }
}