using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Expenses;

public class CreateExpenseEndpoint : Endpoint<CreateExpenseCommand, SingleResponse<ExpenseResponse>>
{
  private readonly IMediator _mediator;

  public CreateExpenseEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/expenses");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Create expense")
        .Produces<SingleResponse<ExpenseResponse>>(201)
        .ProducesProblem(400)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(CreateExpenseCommand req, CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

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

    await SendCreatedAtAsync<GetExpenseEndpoint>(
      new { id = response.Id },
      SingleResponse.Of(response),
      generateAbsoluteUrl: true,
      cancellation: ct);
  }
}