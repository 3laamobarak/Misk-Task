using Domain.Models;
using DTO.DTO.Learner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ILearnerService
    {
        Task<IEnumerable<Learner>> GetAllLearnersAsync(int skip, int take);
        Task<Learner> GetByIdAsync(int id);
        Task<CreateLearnerDTO> CreateAsync(CreateLearnerDTO learnerDto);
    }
}