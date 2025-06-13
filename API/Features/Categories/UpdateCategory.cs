using Core.Features.Categories.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Categories;

public class UpdateCategoryRequest
{
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}

public class UpdateCategoryEndpoint : Endpoint<UpdateCategoryRequest, SingleResponse<CategoryResponse>>
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
        .WithSummary("Update category")
        .Produces<SingleResponse<CategoryResponse>>(200)
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

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}