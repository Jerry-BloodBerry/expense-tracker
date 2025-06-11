using Core.Features.Categories.Commands;
using Core.Domain;
using FastEndpoints;
using MediatR;

namespace API.Features.Categories;

public class UpdateCategoryRequest
{
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}

public class UpdateCategoryEndpoint : Endpoint<UpdateCategoryRequest>
{
  private readonly IMediator _mediator;

  public UpdateCategoryEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/categories/{id}");
    AllowAnonymous();
    Description(d => d
        .WithName("UpdateCategory")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
  {
    var id = Route<int>("id");
    var command = new UpdateCategoryCommand
    {
      Id = id,
      Name = req.Name,
      Description = req.Description
    };

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