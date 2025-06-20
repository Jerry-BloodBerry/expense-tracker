namespace API.Utils.Response;

public class PaginatedResult<T>
{
  public List<T> Data { get; set; } = [];
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}