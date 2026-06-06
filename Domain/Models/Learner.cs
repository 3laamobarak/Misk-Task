using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Models
{
    public class Learner : BaseEntity
    {
        [Required]
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        // custome validation to check nid is unique every time before store in DB 
        public string NationalId { get; set; }
        public string Department { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
