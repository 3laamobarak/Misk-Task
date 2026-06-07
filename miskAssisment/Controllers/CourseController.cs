using Application.Contracts;
using DTO.DTO.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace miskAssisment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Authorize (Roles = "Admin,Manager,Learner")]
        public async Task<IActionResult> GetAllCoursesAsync(int skip = 0, int take = 10)
        {
            var courses = await _courseService.GetAllCoursesAsync(skip, take);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        [Authorize (Roles = "Admin,Manager,Learner")]
        public async Task<IActionResult> GetCourseByIdAsync([FromRoute]int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();
            return Ok(course);
        }

        [HttpPost]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateCourseAsync([FromBody] CreateCourseDTo course)
        {
            if (!ModelState.IsValid || course.DurationHours<=0)
                return BadRequest(ModelState);

            var createdCourse = await _courseService.CreateAsync(course);
            if (createdCourse == null)
                return BadRequest("Failed to create course");

            return Ok(createdCourse);
        }
        [HttpPut("{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> UpdateCourseAsync([FromRoute] int id, [FromBody] UpdateCourseDTO course)
        {
            if (!ModelState.IsValid || course.DurationHours <= 0)
                return BadRequest(ModelState);
            if (id != course.Id)
                return BadRequest("Route ID does not match the Body ID.");

            var updatedCourse = await _courseService.UpdateAsync(course);
            if (updatedCourse == null)
                return NotFound("Course not found");
            return Ok(updatedCourse);
        }
        [HttpDelete("{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteCourseAsync(int id)
        {
            await _courseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
