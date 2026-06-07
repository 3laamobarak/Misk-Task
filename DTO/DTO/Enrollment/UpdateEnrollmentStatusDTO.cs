using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace DTO.DTO.Enrollment
{
    public class UpdateEnrollmentStatusDTO
    {
        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        public EnrollmentStatus Status { get; set; }

        public string? Reason { get; set; } 
    }
}
