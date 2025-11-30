using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Expenses;

public record CreateExpenseRequest(
  string Name,
  int CategoryId,
  decimal Amount,
  DateTime Date,
  string? Description,
  string Currency,
  bool IsRecurring,
  string? RecurrenceInterval,
  List<int> TagIds);

public class CreateExpenseEndpoint : Endpoint<CreateExpenseRequest, SingleResponse<ExpenseResponse>>
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

  public override async Task HandleAsync(CreateExpenseRequest req, CancellationToken ct)
  {
    var command = new CreateExpenseCommand
    {
      Name = req.Name,
      CategoryId = req.CategoryId,
      Amount = req.Amount,
      Date = DateOnly.FromDateTime(req.Date),
      Description = req.Description,
      Currency = req.Currency,
      IsRecurring = req.IsRecurring,
      RecurrenceInterval = req.RecurrenceInterval,
      TagIds = req.TagIds
    };

    var result = await _mediator.Send(command, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new ExpenseResponse(
      Id: result.Value.Id,
      Name: result.Value.Name,
      Amount: result.Value.Amount,
      Date: result.Value.Date.ToDateTime(TimeOnly.MinValue),
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
