using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Models
{
    public class Course : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Must be greater than 0.")]
        public int DurationHours { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
