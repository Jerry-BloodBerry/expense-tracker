using Core.Features.Categories.Commands;
using FastEndpoints;
using MediatR;

namespace API.Features.Categories;

public class CreateCategoryRequest
{
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}

public class CreateCategoryEndpoint : Endpoint<CreateCategoryRequest, int>
{
  private readonly IMediator _mediator;

  public CreateCategoryEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/categories");
    AllowAnonymous();
    Description(d => d
        .WithName("CreateCategory")
        .Produces<int>(201)
        .ProducesProblem(400)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
  {
    var command = new CreateCategoryCommand
    {
      Name = req.Name,
      Description = req.Description
    };

    var categoryId = await _mediator.Send(command, ct);
    await SendCreatedAtAsync<GetCategoryEndpoint>(
        new { id = categoryId },
        categoryId,
        cancellation: ct);
  }
}