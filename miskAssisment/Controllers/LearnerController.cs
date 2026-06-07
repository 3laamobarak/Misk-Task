using Application.Contracts;
using DTO.DTO.Learner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace miskAssisment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearnerController : ControllerBase
    {
        private readonly ILearnerService _learnerService;

        public LearnerController(ILearnerService learnerService)
        {
            _learnerService = learnerService;
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll(int skip = 0, int take = 10)
        {
            var learners = await _learnerService.GetAllLearnersAsync(skip, take);
            return Ok(learners);
        }

        [HttpGet("{id}")]
        [Authorize (Roles = "Admin,Manager,Learner")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var learner = await _learnerService.GetByIdAsync(id);
            if (learner == null) return NotFound();
            return Ok(learner);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Learner")]
        public async Task<IActionResult> Create([FromBody] CreateLearnerDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            try
            {
                var result = await _learnerService.CreateAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}