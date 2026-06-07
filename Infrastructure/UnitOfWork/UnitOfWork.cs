using ContextLayer;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            return new BaseRepository<T>(_context);
        }
        public IBaseRepository<Course> CourseRepository { get; private set;  }
        public IBaseRepository<Learner> LearnerRepository { get; private set; }
        public IBaseRepository<Enrollment> EnrollmentRepository { get; private set; }
        public UnitOfWork(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            CourseRepository = new BaseRepository<Course>(_context);
            LearnerRepository = new BaseRepository<Learner>(_context);
            EnrollmentRepository = new BaseRepository<Enrollment>(_context);
        }
        public async Task Completeasync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
