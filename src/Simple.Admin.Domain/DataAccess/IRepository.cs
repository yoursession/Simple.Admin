﻿using System.Linq.Expressions;

using Simple.Admin.Domain.Entities;
using Simple.Admin.Domain.Shared.Models;

namespace Simple.Admin.Domain.DataAccess
{
    public interface IRepository<T> where T : EntityBase, new()
    {
        Task<int> AddAsync(T model);

        Task<int> UpdateAsync(T model);

        Task<int> UpdateAsync(long id, Func<Updatable<T>, Updatable<T>> updatable);

        Task<int> DeleteAsync(long id);

        Task<int> DeleteAsync(T model);

        Task<int> AddRangeAsync(IEnumerable<T> models);

        Task<int> UpdateRangeAsync(IEnumerable<T> models);

        Task<int> DeleteRangeAsync(IEnumerable<T> models);

        Task<T?> GetAsync(Expression<Func<T, bool>>? expression = default);

        Task<List<T>> GetListAsync(Expression<Func<T, bool>>? expression = default);

        Task<int> CountAsync(Expression<Func<T, bool>>? expression = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>>? expression = default);

        Task<PagingModel<T>> GetPagedAsync(Expression<Func<T, bool>> expression, int page, int size, IEnumerable<QuerySortField>? querySortFields = default);
    }
}