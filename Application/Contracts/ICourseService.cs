using Domain.Models;
using DTO.DTO.Course;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync(int skip, int take);
        Task<Course> GetByIdAsync(int id);
        Task<CreateCourseDTo> CreateAsync(CreateCourseDTo course);
        Task<UpdateCourseDTO> UpdateAsync(UpdateCourseDTO course);
        Task DeleteAsync(int id);

    }
}
