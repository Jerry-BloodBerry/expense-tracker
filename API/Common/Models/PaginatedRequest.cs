using FastEndpoints;

namespace API.Common.Models;

public record PaginatedRequest
{
  [QueryParam]
  public int? Page { get; init; }
  [QueryParam]
  public int? PageSize { get; init; }
}