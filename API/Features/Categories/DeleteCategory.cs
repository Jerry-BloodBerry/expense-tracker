using Core.Features.Categories.Commands;
using Core.Domain;
using FastEndpoints;
using MediatR;

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
        .WithName("DeleteCategory")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    var command = new DeleteCategoryCommand(id);

    try
    {
      await _mediator.Send(command, ct);
      await SendNoContentAsync(ct);
    }
    catch (NotFoundException)
    {
      await SendNotFoundAsync(ct);
    }
  }
}