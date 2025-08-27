using ECommerceApi.DTOs;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
        {
            return queryable
                .Skip((paginationDto.Page - 1) * paginationDto.RecordsPerPage)
                .Take(paginationDto.RecordsPerPage);
        }

        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationDto pagination)
        {
            var totalRecords = await query.CountAsync();
            var data = await query.Page(pagination).ToListAsync();

            return new PagedResult<T>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = pagination.Page,
                RecordsPerPage = pagination.RecordsPerPage
            };
        }
    }
}