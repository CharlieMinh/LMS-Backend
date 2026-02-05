using System;
using System.Linq;
using System.Linq.Expressions;
using LMS.Application.DTOs;

namespace LMS.Application.Common
{
    public static class PaginationExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, PagedRequest request)
        {
            var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? GetDefaultSortProperty(typeof(T)) : request.SortBy;
            var property = typeof(T).GetProperty(sortBy ?? string.Empty, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property == null)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            var methodName = request.SortDesc ? "OrderByDescending" : "OrderBy";
            var resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
            return query.Provider.CreateQuery<T>(resultExp);
        }

        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query, PagedRequest request)
        {
            var total = query.Count();
            var skip = (request.PageNumber - 1) * request.PageSize;
            var items = query.Skip(skip).Take(request.PageSize).ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private static string? GetDefaultSortProperty(Type type)
        {
            var createdAt = type.GetProperty("CreatedAt");
            if (createdAt != null) return "CreatedAt";
            var updatedAt = type.GetProperty("UpdatedAt");
            if (updatedAt != null) return "UpdatedAt";
            var title = type.GetProperty("Title");
            if (title != null) return "Title";
            return "Id";
        }
    }
}
