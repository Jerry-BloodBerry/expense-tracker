using Core.Features.Categories;
using Core.Features.Categories.Queries;
using FastEndpoints;
using MediatR;

namespace API.Features.Categories;

public class GetCategoriesEndpoint : EndpointWithoutRequest<List<CategoryDto>>
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
        .WithName("GetCategories")
        .Produces<List<CategoryDto>>(200)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var query = new GetCategoriesQuery();
    var categories = await _mediator.Send(query, ct);
    await SendOkAsync(categories, ct);
  }
}