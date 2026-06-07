using Application.Contracts;
using Domain.Enums;
using DTO.DTO.Enrollment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace miskAssisment.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> Submit([FromBody] CreateEnrollmentDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            try
            {
                var result = await _enrollmentService.SubmitEnrollmentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("review")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Review([FromBody] UpdateEnrollmentStatusDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            try
            {
                var result = await _enrollmentService.ReviewEnrollmentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("learner/{learnerId}")]
        [Authorize (Roles = "Admin,Manager,Learner")]
        public async Task<IActionResult> GetByLearner([FromRoute] int learnerId)
        {
            var enrollments = await _enrollmentService.GetLearnerEnrollmentsAsync(learnerId);
            return Ok(enrollments);
        }

        [HttpGet("pending")]
        [Authorize (Roles = "Manager")]
        public async Task<IActionResult> GetPending()
        {
            var pending = await _enrollmentService.GetPendingEnrollmentsAsync();
            return Ok(pending);
        }
        
        [HttpGet]
        [Authorize (Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEnrollments(
            [FromQuery] int? learnerId,
            [FromQuery] int? courseId,
            [FromQuery] EnrollmentStatus? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var results = await _enrollmentService.GetFilteredEnrollmentsAsync(learnerId, courseId, status, fromDate, toDate);
            return Ok(results);
        }

        
    }
}