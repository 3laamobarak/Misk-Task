using Application.Contracts;
using Domain.Interfaces;
using Domain.Models;
using DTO.DTO.Learner;

namespace Application.Services
{
    public class LearnerService : ILearnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LearnerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Learner>> GetAllLearnersAsync(int skip, int take)
        {
            var learners = await _unitOfWork.LearnerRepository.GetAllAsync(skip, take);
            return learners.ToList();
        }

        public async Task<Learner> GetByIdAsync(int id)
        {
            return await _unitOfWork.LearnerRepository.GetByIdAsync(id);
        }

        public async Task<CreateLearnerDTO> CreateAsync(CreateLearnerDTO learnerDto)
        {
            var existing = await _unitOfWork.LearnerRepository.GetByExpressionAsync(l => l.NationalId == learnerDto.NationalId);
            for (int i=0; i < learnerDto.NationalId.Length; i++)
            {
                if (!char.IsDigit(learnerDto.NationalId[i]))
                {
                    throw new InvalidOperationException("National ID must contain only digits.");
                }
            }
            if (existing != null && existing.Any()|| learnerDto.NationalId.Length != 14)
            {
                throw new InvalidOperationException("This National ID not valid");
            }
            var newLearner = new Learner
            {
                FullName = learnerDto.FullName,
                Email = learnerDto.Email,
                NationalId = learnerDto.NationalId,
                Department = learnerDto.Department,
                ApplicationUserId = learnerDto.ApplicationUserId,
                Enrollments = new List<Enrollment>()
            };
            await _unitOfWork.LearnerRepository.AddAsync(newLearner);
            await _unitOfWork.Completeasync();
            return learnerDto;
        }

    }
}