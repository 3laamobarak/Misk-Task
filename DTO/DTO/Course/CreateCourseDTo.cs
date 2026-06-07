using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTO.Course
{
    public class CreateCourseDTo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationHours { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsActive { get; set; }
    }
}
