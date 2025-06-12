using Core.Features.Tags.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;

namespace API.Features.Tags;

public class CreateTagEndpoint : Endpoint<CreateTagRequest, SingleResponse<TagResponse>>
{
  private readonly IMediator _mediator;

  public CreateTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/tags");
    AllowAnonymous();
    Description(d => d
        .WithName("CreateTag")
        .WithSummary("Create new tag")
        .Produces<SingleResponse<TagResponse>>(201)
        .ProducesProblem(400)
        .WithTags("Tags"));
  }

  public override async Task HandleAsync(CreateTagRequest req, CancellationToken ct)
  {
    var command = new CreateTagCommand { Name = req.Name };
    var id = await _mediator.Send(command, ct);

    var response = new TagResponse
    {
      Id = id,
      Name = req.Name
    };

    await SendCreatedAtAsync<GetTagEndpoint>(
        new { id = response.Id },
        SingleResponse.Of(response),
        generateAbsoluteUrl: true,
        cancellation: ct);
  }
}