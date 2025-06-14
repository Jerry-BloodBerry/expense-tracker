using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Interfaces;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Expenses;

public record GetExpenseRequest
{
  public int Id { get; init; }
}

public class GetExpenseEndpoint : Endpoint<GetExpenseRequest, SingleResponse<ExpenseResponse>>
{
  private readonly IMediator _mediator;

  public GetExpenseEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/expenses/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get expense by ID")
        .Produces<SingleResponse<ExpenseResponse>>(200)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetExpenseRequest req, CancellationToken ct)
  {
    var query = new GetExpenseQuery(req.Id);
    var result = await _mediator.Send(query, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new ExpenseResponse(
      Id: result.Value.Id,
      Name: result.Value.Name,
      Amount: result.Value.Amount,
      Date: result.Value.Date,
      Description: result.Value.Description,
      Currency: result.Value.Currency,
      IsRecurring: result.Value.IsRecurring,
      RecurrenceInterval: result.Value.RecurrenceInterval?.ToString(),
      Category: result.Value.Category.Id,
      Tags: result.Value.Tags.Select(t => t.Id).ToList()
    );

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}