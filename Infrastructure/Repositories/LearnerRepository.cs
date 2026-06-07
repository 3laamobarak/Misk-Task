using ContextLayer;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories
{
    public class LearnerRepository :BaseRepository<Learner>, ILearnerRepository
    {
        public LearnerRepository(Context context) : base(context)
        {
        }

    }

}
