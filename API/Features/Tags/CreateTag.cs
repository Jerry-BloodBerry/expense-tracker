using Core.Features.Tags.Commands;
using FastEndpoints;
using MediatR;
using API.Utils.Response;
using ErrorOr;
using Microsoft.AspNetCore.Http;

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
    var result = await _mediator.Send(command, ct);

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }
    var tag = result.Value;

    var response = new TagResponse
    {
      Id = tag.Id,
      Name = tag.Name
    };

    await SendCreatedAtAsync<GetTagEndpoint>(
        new { id = response.Id },
        SingleResponse.Of(response),
        generateAbsoluteUrl: true,
        cancellation: ct);
  }
}