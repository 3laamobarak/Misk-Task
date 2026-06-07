using System.ComponentModel.DataAnnotations;

namespace DTO.DTO.Enrollment
{
    public class CreateEnrollmentDTO
    {
        [Required]
        public int LearnerId { get; set; }

        [Required]
        public int CourseId { get; set; }
            
        public string? Reason { get; set; }
    }
}
