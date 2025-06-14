using Core.Features.Tags.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Tags;

public class GetTagsEndpoint : EndpointWithoutRequest<ListResponse<TagResponse>>
{
  private readonly IMediator _mediator;

  public GetTagsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/tags");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get all tags")
        .Produces<ListResponse<TagResponse>>(200)
        .WithTags("Tags"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var query = new GetTagsQuery();
    var result = await _mediator.Send(query, ct);

    var response = result.Select(t => new TagResponse
    {
      Id = t.Id,
      Name = t.Name
    }).ToList();

    await SendOkAsync(ListResponse.Of(response), ct);
  }
}