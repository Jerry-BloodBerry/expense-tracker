using Core.Features.Expenses.Queries;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public record GetAllExpensesRequest
{
  [QueryParam]
  public DateTime? StartDate { get; init; }
  [QueryParam]
  public DateTime? EndDate { get; init; }
  [QueryParam]
  public int? CategoryId { get; init; }
  [QueryParam]
  public List<int> TagIds { get; init; } = new();
  [QueryParam]
  public decimal? MinAmount { get; init; }
  [QueryParam]
  public decimal? MaxAmount { get; init; }
  [QueryParam]
  public bool? IsRecurring { get; init; }
  [QueryParam]
  public int Page { get; init; } = 1;
  [QueryParam]
  public int PageSize { get; init; } = 10;
}

public class GetAllExpensesEndpoint : Endpoint<GetAllExpensesRequest, ExpensesResult>
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
        .Produces<ExpensesResult>(200)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetAllExpensesRequest req, CancellationToken ct)
  {
    var query = new GetExpensesQuery
    {
      StartDate = req.StartDate,
      EndDate = req.EndDate,
      CategoryId = req.CategoryId,
      TagIds = req.TagIds,
      MinAmount = req.MinAmount,
      MaxAmount = req.MaxAmount,
      IsRecurring = req.IsRecurring,
      Page = req.Page,
      PageSize = Math.Min(req.PageSize, 50)
    };

    var result = await _mediator.Send(query, ct);
    await SendOkAsync(result, ct);
  }
}