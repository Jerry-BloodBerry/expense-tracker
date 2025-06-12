using Core.Features.Tags.Commands;
using FastEndpoints;
using MediatR;

namespace API.Features.Tags;

public record DeleteTagRequest
{
  public int Id { get; init; }
}

public class DeleteTagEndpoint : Endpoint<DeleteTagRequest>
{
  private readonly IMediator _mediator;

  public DeleteTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/tags/{id}");
    AllowAnonymous();
    Description(d => d
        .WithName("DeleteTag")
        .WithSummary("Delete tag")
        .Produces(204)
        .ProducesProblem(404)
        .WithTags("Tags"));
  }

  public override async Task HandleAsync(DeleteTagRequest req, CancellationToken ct)
  {
    var command = new DeleteTagCommand(req.Id);
    await _mediator.Send(command, ct);
    await SendNoContentAsync(ct);
  }
}