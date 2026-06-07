using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<Course> CourseRepository { get; }
        IBaseRepository<Learner> LearnerRepository { get; }
        IBaseRepository<Enrollment> EnrollmentRepository { get; }
        Task Completeasync();
        void Dispose();
        //Task<int> SaveChangesAsync();
    }
}
