using Core.Features.Tags.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;
using API.Swagger;

namespace API.Features.Tags;

public class UpdateTagEndpoint : Endpoint<UpdateTagRequest, SingleResponse<TagResponse>>
{
  private readonly IMediator _mediator;

  public UpdateTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/tags/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Update tag")
        .Produces<SingleResponse<TagResponse>>(200)
        .ProducesProblem(404)
        .WithTags("Tags"));
  }

  public override async Task HandleAsync(UpdateTagRequest req, CancellationToken ct)
  {
    var command = new UpdateTagCommand
    {
      Id = Route<int>("id"),
      Name = req.Name
    };

    var result = await _mediator.Send(command, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new TagResponse
    {
      Id = command.Id,
      Name = command.Name
    };

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}