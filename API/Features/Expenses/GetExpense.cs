using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Interfaces;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public record GetExpenseRequest
{
  public int Id { get; init; }
}

public class GetExpenseEndpoint : Endpoint<GetExpenseRequest, ExpenseResponse>
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
        .Produces<ExpenseResponse>(200)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetExpenseRequest req, CancellationToken ct)
  {
    var query = new GetExpenseQuery(req.Id);
    var expense = await _mediator.Send(query, ct);

    var response = new ExpenseResponse(
      Id: expense.Id,
      Name: expense.Name,
      Amount: expense.Amount,
      Date: expense.Date,
      Description: expense.Description,
      Currency: expense.Currency,
      IsRecurring: expense.IsRecurring,
      RecurrenceInterval: expense.RecurrenceInterval?.ToString(),
      Category: expense.Category.Id,
      Tags: expense.Tags.Select(t => t.Id).ToList()
    );

    await SendOkAsync(response, ct);
  }
}