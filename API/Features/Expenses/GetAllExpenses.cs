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

  private List<string>? _tagIds = [];
  [QueryParam]
  public List<string>? TagIds
  {
    get => _tagIds;
    set
    {
      _tagIds = value?.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
    }
  }

  [QueryParam]
  public decimal? MinAmount { get; init; }
  [QueryParam]
  public decimal? MaxAmount { get; init; }
  [QueryParam]
  public bool? IsRecurring { get; init; }
  [QueryParam]
  public string? Search { get; init; }
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
        .WithSummary("Get all expenses with pagination")
        .Produces<PaginatedResult<ExpenseResponse>>(200)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetAllExpensesRequest req, CancellationToken ct)
  {
    // Convert dates to UTC for PostgreSQL compatibility
    // For date-only values, treat StartDate as start of day and EndDate as end of day
    DateTime? startDate = null;
    DateTime? endDate = null;

    if (req.StartDate.HasValue)
    {
      var date = req.StartDate.Value;
      // If Kind is Unspecified, assume it's a date-only value and set to UTC
      if (date.Kind == DateTimeKind.Unspecified)
      {
        startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
      }
      else
      {
        startDate = date.ToUniversalTime();
      }
    }

    if (req.EndDate.HasValue)
    {
      var date = req.EndDate.Value;
      // If Kind is Unspecified, assume it's a date-only value and set to end of day UTC
      if (date.Kind == DateTimeKind.Unspecified)
      {
        endDate = DateTime.SpecifyKind(date.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
      }
      else
      {
        endDate = date.ToUniversalTime();
      }
    }

    var query = new GetExpensesQuery
    {
      StartDate = startDate,
      EndDate = endDate,
      CategoryId = req.CategoryId,
      TagIds = req.TagIds.Select(int.Parse).ToList() ?? [],
      MinAmount = req.MinAmount,
      MaxAmount = req.MaxAmount,
      IsRecurring = req.IsRecurring,
      Search = req.Search,
      Page = req.Page ?? 1,
      PageSize = Math.Min(req.PageSize ?? 10, 50)
    };

    var result = await _mediator.Send(query, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new PaginatedResult<ExpenseResponse>
    {
      Data = result.Value.Items.Select(dto => dto.AsResponse()).ToList(),
      TotalCount = result.Value.TotalCount,
      Page = result.Value.Page,
      PageSize = result.Value.PageSize
    };

    await SendOkAsync(response, ct);
  }
}