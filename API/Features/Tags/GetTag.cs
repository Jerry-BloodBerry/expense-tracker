using Core.Features.Tags.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Tags;

public record GetTagRequest
{
  public int Id { get; init; }
}

public class GetTagEndpoint : Endpoint<GetTagRequest, SingleResponse<TagResponse>>
{
  private readonly IMediator _mediator;

  public GetTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/tags/{id}");
    AllowAnonymous();
    Description(d => d
        .WithName("GetTag")
        .WithSummary("Get single tag")
        .Produces<SingleResponse<TagResponse>>(200)
        .ProducesProblem(404)
        .WithTags("Tags"));
  }

  public override async Task HandleAsync(GetTagRequest req, CancellationToken ct)
  {
    var query = new GetTagQuery(req.Id);
    var result = await _mediator.Send(query, ct);

    var response = new TagResponse
    {
      Id = result.Id,
      Name = result.Name
    };

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}