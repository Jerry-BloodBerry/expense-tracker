using Core.Features.Categories.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Categories;

public class GetCategoriesEndpoint : EndpointWithoutRequest<ListResponse<CategoryResponse>>
{
  private readonly IMediator _mediator;

  public GetCategoriesEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/categories");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get all categories")
        .Produces<ListResponse<CategoryResponse>>(200)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var query = new GetCategoriesQuery();
    var result = await _mediator.Send(query, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = result.Value.Select(c => new CategoryResponse
    {
      Id = c.Id,
      Name = c.Name,
      Description = c.Description
    }).ToList();

    await SendOkAsync(ListResponse.Of(response), ct);
  }
}