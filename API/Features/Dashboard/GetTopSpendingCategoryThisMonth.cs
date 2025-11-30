using Core.Features.Expenses.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Dashboard;

public record TopCategoryResponse
{
  public int CategoryId { get; init; }
  public string CategoryName { get; init; } = string.Empty;
  public decimal TotalAmount { get; init; }
}

public class GetTopSpendingCategoryThisMonthEndpoint : EndpointWithoutRequest<SingleResponse<TopCategoryResponse>>
{
  private readonly IMediator _mediator;

  public GetTopSpendingCategoryThisMonthEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/dashboard/top-spending-category-this-month");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get top spending category for current month")
        .Produces<SingleResponse<TopCategoryResponse>>(200)
        .WithTags("Dashboard"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var startDate = new DateOnly(today.Year, today.Month, 1);
    var endDate = startDate.AddMonths(1).AddDays(-1);

    var query = new GetTopSpendingCategoryThisMonthQuery
    {
      StartDate = startDate,
      EndDate = endDate
    };

    var result = await _mediator.Send(query, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new TopCategoryResponse
    {
      CategoryId = result.Value.CategoryId,
      CategoryName = result.Value.CategoryName,
      TotalAmount = result.Value.TotalAmount
    };

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}

