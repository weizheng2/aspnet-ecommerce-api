
// using Microsoft.EntityFrameworkCore;

// namespace ECommerce.Application.Common;

// // Move to Infrastructure, but add a interface for usage
// public static class IQueryableExtensions
// {
//     public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
//     {
//         return queryable
//             .Skip((paginationDto.Page - 1) * paginationDto.RecordsPerPage)
//             .Take(paginationDto.RecordsPerPage);
//     }

//     public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationDto pagination)
//     {
//         var totalRecords = await query.CountAsync();
//         var data = await query.Page(pagination).ToListAsync();

//         return new PagedResult<T>
//         {
//             Data = data,
//             TotalRecords = totalRecords,
//             Page = pagination.Page,
//             RecordsPerPage = pagination.RecordsPerPage
//         };
//     }
// }
