using Core.Features.Expenses.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

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
        .WithSummary("Delete expense")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(DeleteExpenseCommand req, CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    await SendNoContentAsync(ct);
  }
}