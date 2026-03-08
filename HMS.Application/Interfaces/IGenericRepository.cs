using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
        IQueryable<T> Query();
    }
}