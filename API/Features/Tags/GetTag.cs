using Core.Features.Tags.Queries;
using FastEndpoints;
using MediatR;
using API.Utils.Response;
using ErrorOr;

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

    if (result.IsError)
    {
      await ProblemResult.Of(result.Errors, HttpContext).ExecuteAsync(HttpContext);
      return;
    }

    var response = new TagResponse
    {
      Id = result.Value.Id,
      Name = result.Value.Name
    };

    await SendOkAsync(SingleResponse.Of(response), ct);
  }
}

public static class ErrorExtensions
{
  public static int StatusCode(this ErrorOr.Error error)
  {
    return error.Type switch
    {
      ErrorType.NotFound => StatusCodes.Status404NotFound,
      ErrorType.Validation => StatusCodes.Status400BadRequest,
      ErrorType.Conflict => StatusCodes.Status409Conflict,
      _ => StatusCodes.Status500InternalServerError
    };
  }
}