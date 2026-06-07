using Domain.Enums;

namespace DTO.DTO.Enrollment
{
    public class EnrollmentResponseDTO
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; }
        public string LearnerEmail { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public EnrollmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}