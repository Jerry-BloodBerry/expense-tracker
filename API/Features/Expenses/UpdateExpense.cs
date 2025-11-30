using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Expenses;

public record UpdateExpenseRequest(
  int Id,
  string Name,
  int CategoryId,
  decimal Amount,
  DateTime Date,
  string? Description,
  string Currency,
  bool IsRecurring,
  string? RecurrenceInterval,
  List<int> TagIds);

public class UpdateExpenseEndpoint : Endpoint<UpdateExpenseRequest, SingleResponse<ExpenseResponse>>
{
  private readonly IMediator _mediator;

  public UpdateExpenseEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/expenses/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Update expense")
        .Produces<SingleResponse<ExpenseResponse>>(200)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(UpdateExpenseRequest req, CancellationToken ct)
  {
    var command = new UpdateExpenseCommand
    {
      Id = req.Id,
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

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}
