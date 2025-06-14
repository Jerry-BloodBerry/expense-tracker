using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public class CreateExpenseEndpoint : Endpoint<CreateExpenseCommand, int>
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
        .Produces<int>(201)
        .ProducesProblem(400)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(CreateExpenseCommand req, CancellationToken ct)
  {
    var expenseId = await _mediator.Send(req, ct);
    await SendCreatedAtAsync<GetExpenseEndpoint>(
        new { id = expenseId },
        expenseId,
        generateAbsoluteUrl: true,
        cancellation: ct);
  }
}