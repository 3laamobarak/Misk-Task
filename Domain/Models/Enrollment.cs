using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Enrollment : BaseEntity
    {
        public int LearnerId { get; set; }
        public Learner Learner { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public EnrollmentStatus Status { get; set; }

        public string? Reason { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
