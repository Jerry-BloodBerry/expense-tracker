using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;

namespace API.Features.Expenses;

public class UpdateExpenseEndpoint : Endpoint<UpdateExpenseCommand>
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
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(UpdateExpenseCommand req, CancellationToken ct)
  {
    await _mediator.Send(req, ct);
    await SendNoContentAsync(ct);
  }
}