using Core.Features.Categories.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Categories;

public class GetCategoryEndpoint : EndpointWithoutRequest<SingleResponse<CategoryResponse>>
{
  private readonly IMediator _mediator;

  public GetCategoryEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/categories/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get category by ID")
        .Produces<SingleResponse<CategoryResponse>>(200)
        .ProducesProblem(404)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    var query = new GetCategoryQuery(id);
    var result = await _mediator.Send(query, ct);

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