namespace ECommerce.Application.Common;

public class PagedResult<T>
{
    public PagedResult(List<T> data, int totalRecords, int page, int recordsPerPage)
    {
        Data = data;
        TotalRecords = totalRecords;
        Page = page;
        RecordsPerPage = recordsPerPage;
    }
    
    public List<T> Data { get; set; } = [];
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int RecordsPerPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);  
}
