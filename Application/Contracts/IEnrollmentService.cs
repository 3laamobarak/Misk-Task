using Domain.Enums;
using Domain.Models;
using DTO.DTO.Enrollment;

namespace Application.Contracts

{   
    public interface IEnrollmentService
    {
        Task<Enrollment> SubmitEnrollmentAsync(CreateEnrollmentDTO dto);
        Task<Enrollment> ReviewEnrollmentAsync(UpdateEnrollmentStatusDTO dto);
        Task<IEnumerable<Enrollment>> GetLearnerEnrollmentsAsync(int learnerId);
        Task<IEnumerable<Enrollment>> GetPendingEnrollmentsAsync();
        
        Task<IEnumerable<EnrollmentResponseDTO>> GetFilteredEnrollmentsAsync(
            int? learnerId, int? courseId, EnrollmentStatus? status, DateTime? fromDate, DateTime? toDate);
    }

}