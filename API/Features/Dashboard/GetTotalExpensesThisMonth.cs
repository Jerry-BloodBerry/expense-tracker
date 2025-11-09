using Core.Features.Expenses.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Dashboard;

public class GetTotalExpensesThisMonthEndpoint : EndpointWithoutRequest<SingleResponse<decimal>>
{
  private readonly IMediator _mediator;

  public GetTotalExpensesThisMonthEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/dashboard/total-expenses-this-month");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get total expenses for current month")
        .Produces<SingleResponse<decimal>>(200)
        .WithTags("Dashboard"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var now = DateTime.UtcNow;
    var startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
    var endDate = startDate.AddMonths(1).AddTicks(-1);

    var query = new GetTotalExpensesThisMonthQuery
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

    await SendOkAsync(SingleResponse.Of(result.Value), ct);
  }
}

