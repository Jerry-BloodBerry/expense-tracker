using Core.Features.Categories.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Categories;

public class DeleteCategoryEndpoint : EndpointWithoutRequest
{
  private readonly IMediator _mediator;

  public DeleteCategoryEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/categories/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Delete category")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    var command = new DeleteCategoryCommand(id);

    var result = await _mediator.Send(command, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    await SendNoContentAsync(ct);
  }
}