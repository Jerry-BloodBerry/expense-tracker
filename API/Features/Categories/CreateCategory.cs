using Core.Features.Categories.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Categories;

public class CreateCategoryRequest
{
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}

public class CreateCategoryEndpoint : Endpoint<CreateCategoryRequest, SingleResponse<CategoryResponse>>
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
        .WithSummary("Create new category")
        .Produces<SingleResponse<CategoryResponse>>(201)
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

    var result = await _mediator.Send(command, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new CategoryResponse
    {
      Id = result.Value.Id,
      Name = result.Value.Name,
      Description = result.Value.Description
    };

    await SendCreatedAtAsync<GetCategoryEndpoint>(
        new { id = response.Id },
        SingleResponse.Of(response),
        generateAbsoluteUrl: true,
        cancellation: ct);
  }
}