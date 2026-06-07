using Application.Contracts;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using DTO.DTO.Enrollment;

namespace Application.Services
{
public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Enrollment> SubmitEnrollmentAsync(CreateEnrollmentDTO dto)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(dto.CourseId);
            if (course == null || !course.IsActive)
                throw new ArgumentException("Course is unavailable or inactive.");

            var learner = await _unitOfWork.LearnerRepository.GetByIdAsync(dto.LearnerId);
            if (learner == null)
                throw new ArgumentException("Learner account not found.");

            var existing = await _unitOfWork.EnrollmentRepository.GetByExpressionAsync(
                e => e.LearnerId == dto.LearnerId && e.CourseId == dto.CourseId);
            
            if (existing != null && existing.Any())
                throw new InvalidOperationException("Learner is already enrolled or has an active request for this course.");

            var enrollment = new Enrollment
            {
                LearnerId = dto.LearnerId,
                CourseId = dto.CourseId,
                Reason = dto.Reason,
                Status = course.RequiresApproval ? EnrollmentStatus.PendingApproval : EnrollmentStatus.Approved,
                DecisionDate = course.RequiresApproval ? null : DateTime.UtcNow
            };

            await _unitOfWork.EnrollmentRepository.AddAsync(enrollment);
            await _unitOfWork.Completeasync();
            return enrollment;
        }

        public async Task<Enrollment> ReviewEnrollmentAsync(UpdateEnrollmentStatusDTO dto)
        {
            var enrollment = await _unitOfWork.EnrollmentRepository.GetByIdAsync(dto.EnrollmentId);
            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment record does not exist.");
            if (enrollment.Status != EnrollmentStatus.PendingApproval)
                throw new InvalidOperationException("Only pending enrollments can be reviewed.");
            if (dto.Status == EnrollmentStatus.Approved || dto.Status == EnrollmentStatus.Rejected)
            {
                if (dto.Status == EnrollmentStatus.Rejected && string.IsNullOrWhiteSpace(dto.Reason))
                    throw new ArgumentException("A reason must be provided for rejected enrollments.");

                enrollment.Status = dto.Status;
                enrollment.Reason = dto.Reason;
                enrollment.DecisionDate = DateTime.UtcNow;

                await _unitOfWork.EnrollmentRepository.UpdateAsync(enrollment);
                await _unitOfWork.Completeasync();
                return enrollment;
            }
            else
            {
                throw new ArgumentException("Invalid status. Only Approved or Rejected are allowed.");
            }
        }

        public async Task<IEnumerable<Enrollment>> GetLearnerEnrollmentsAsync(int learnerId)
        {
            return await _unitOfWork.EnrollmentRepository.GetByExpressionAsync(e => e.LearnerId == learnerId);
        }

        public async Task<IEnumerable<Enrollment>> GetPendingEnrollmentsAsync()
        {
            return await _unitOfWork.EnrollmentRepository.GetByExpressionAsync(e => e.Status == EnrollmentStatus.PendingApproval);
        }
        
        public async Task<IEnumerable<EnrollmentResponseDTO>> GetFilteredEnrollmentsAsync(
            int? learnerId, int? courseId, EnrollmentStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            // Define includes arrays to fetch the basic details from associated objects
            var includes = new System.Linq.Expressions.Expression<Func<Enrollment, object>>[]
            {
                e => e.Learner,
                e => e.Course
            };

            // Pull all values with the requested navigation collections tracked
            var items = await _unitOfWork.EnrollmentRepository.GetByExpressionAsync(e => true, includes);
            var query = items.AsQueryable();

            // Dynamically apply filtration
            if (learnerId.HasValue)
                query = query.Where(e => e.LearnerId == learnerId.Value);

            if (courseId.HasValue)
                query = query.Where(e => e.CourseId == courseId.Value);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            if (fromDate.HasValue)
                query = query.Where(e => e.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.CreatedAt <= toDate.Value);

            // Map to the safe clean response payload objects
            return query.Select(e => new EnrollmentResponseDTO
            {
                Id = e.Id,
                LearnerId = e.LearnerId,
                LearnerName = e.Learner.FullName ?? "N/A",
                LearnerEmail = e.Learner.Email ?? "N/A",
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title ?? "N/A",
                Status = e.Status,
                Reason = e.Reason,
                DecisionDate = e.DecisionDate,
                CreatedAt = e.CreatedAt
            }).ToList();
        }
    }
}