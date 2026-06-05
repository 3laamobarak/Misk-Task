using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task Completeasync();
        void Dispose();
        //Task<int> SaveChangesAsync();
    }
}
