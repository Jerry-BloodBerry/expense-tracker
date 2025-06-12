using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Tags.Queries;

public record GetTagQuery(int Id) : IRequest<ErrorOr<TagDto>>;

public class GetTagHandler : IRequestHandler<GetTagQuery, ErrorOr<TagDto>>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public GetTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<TagDto>> Handle(GetTagQuery request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
    if (tag is null)
      return TagErrors.NotFound(request.Id);

    return new TagDto
    {
      Id = tag.Id,
      Name = tag.Name
    };
  }
}