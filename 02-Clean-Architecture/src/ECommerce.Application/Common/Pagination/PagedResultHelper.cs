namespace ECommerce.Application.Common;

public static class PagedResultHelper
{
    public static PagedResult<T> Create<T>(List<T> data, int totalRecords, int page, int recordsPerPage)
    {
        return new PagedResult<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = page,
            RecordsPerPage = recordsPerPage
        };
    }
}

