using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Mini_Dating_App_BE.Repositories.Interfaces
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<T> SingleOrDefaultAsync(
           Expression<Func<T, bool>> predicate = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<TResult> SingleOrDefaultAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<ICollection<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<ICollection<TResult>> GetListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);


        #region Insert

        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);
        #endregion

        #region Update

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        #endregion

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
