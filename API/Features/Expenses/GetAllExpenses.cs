using API.Common.Models;
using API.Utils.Response;
using Core.Features.Expenses.Queries;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public record GetAllExpensesRequest : PaginatedRequest
{
  [QueryParam]
  public DateTime? StartDate { get; init; }
  [QueryParam]
  public DateTime? EndDate { get; init; }
  [QueryParam]
  public int? CategoryId { get; init; }
  [QueryParam]
  public List<int>? TagIds { get; init; }
  [QueryParam]
  public decimal? MinAmount { get; init; }
  [QueryParam]
  public decimal? MaxAmount { get; init; }
  [QueryParam]
  public bool? IsRecurring { get; init; }
}

public class GetAllExpensesEndpoint : Endpoint<GetAllExpensesRequest, PaginatedResult<ExpenseResponse>>
{
  private readonly IMediator _mediator;

  public GetAllExpensesEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/expenses");
    AllowAnonymous();
    Description(d => d
        .WithName("GetAllExpenses")
        .WithSummary("Get all expenses with pagination")
        .Produces<PaginatedResult<ExpenseResponse>>(200)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetAllExpensesRequest req, CancellationToken ct)
  {
    var query = new GetExpensesQuery
    {
      StartDate = req.StartDate,
      EndDate = req.EndDate,
      CategoryId = req.CategoryId,
      TagIds = req.TagIds ?? [],
      MinAmount = req.MinAmount,
      MaxAmount = req.MaxAmount,
      IsRecurring = req.IsRecurring,
      Page = req.Page ?? 1,
      PageSize = Math.Min(req.PageSize ?? 10, 50)
    };

    var result = await _mediator.Send(query, ct);
    var response = new PaginatedResult<ExpenseResponse>
    {
      Data = result.Items.Select(dto => dto.AsResponse()).ToList(),
      TotalCount = result.TotalCount,
      Page = result.Page,
      PageSize = result.PageSize
    };

    await SendOkAsync(response, ct);
  }
}