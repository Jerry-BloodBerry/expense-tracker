using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public class DeleteExpenseEndpoint : Endpoint<DeleteExpenseCommand>
{
  private readonly IMediator _mediator;

  public DeleteExpenseEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/expenses/{Id:int}");
    AllowAnonymous();
    Description(d => d
        .WithName("DeleteExpense")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(DeleteExpenseCommand req, CancellationToken ct)
  {
    await _mediator.Send(req, ct);
    await SendNoContentAsync(ct);
  }
}