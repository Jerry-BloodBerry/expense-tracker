using Core.Features.Categories;
using Core.Features.Categories.Queries;
using Core.Domain;
using FastEndpoints;
using MediatR;

namespace API.Features.Categories;

public class GetCategoryEndpoint : EndpointWithoutRequest<CategoryDto>
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
        .WithName("GetCategory")
        .Produces<CategoryDto>(200)
        .ProducesProblem(404)
        .WithTags("Categories"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    var query = new GetCategoryQuery(id);

    try
    {
      var category = await _mediator.Send(query, ct);
      await SendOkAsync(category, ct);
    }
    catch (NotFoundException)
    {
      await SendNotFoundAsync(ct);
    }
  }
}