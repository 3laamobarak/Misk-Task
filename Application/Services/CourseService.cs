using Application.Contracts;
using Domain.Interfaces;
using Domain.Models;
using DTO.DTO.Course;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class CourseService : ICourseService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
        }
        public async Task<List<Course>> GetAllCoursesAsync(int skip, int take)
        {
            var courses = await _unitOfWork.CourseRepository.GetAllAsync(skip, take);

            return courses.ToList();
        }
        public async Task<Course> GetByIdAsync(int id)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
            return course;
        }
        public async Task<CreateCourseDTo> CreateAsync(CreateCourseDTo course)
        {
            var newCourse = new Course
            {
                Title = course.Title,
                Description = course.Description,
                DurationHours = course.DurationHours,
                IsActive = course.IsActive,
                RequiresApproval = course.RequiresApproval,
                Enrollments = new List<Enrollment>()
            };

            await _unitOfWork.CourseRepository.AddAsync(newCourse);
            await _unitOfWork.Completeasync();
            return course;
        }
        public async Task<UpdateCourseDTO> UpdateAsync(UpdateCourseDTO course)
        {
            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(course.Id);
            if (existingCourse == null)
            {
                throw new Exception("Course not found");
            }
            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.DurationHours = course.DurationHours;
            existingCourse.IsActive = course.IsActive;
            existingCourse.RequiresApproval = course.RequiresApproval;
            await _unitOfWork.CourseRepository.UpdateAsync(existingCourse);
            await _unitOfWork.Completeasync();
            return course;
        }
        public async Task DeleteAsync(int id)
        {
            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(id);
            if (existingCourse == null)
            {
                throw new Exception("Course not found");
            }
            await _unitOfWork.CourseRepository.DeleteAsync(existingCourse);
            await _unitOfWork.Completeasync();
        }
    }
}
