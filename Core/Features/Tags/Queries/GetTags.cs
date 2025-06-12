using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Tags.Queries;

public record GetTagsQuery : IRequest<List<TagDto>>;

public class GetTagsHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public GetTagsHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
  {
    var tags = await _tagRepository.ListAllAsync(cancellationToken);

    return tags.Select(t => new TagDto
    {
      Id = t.Id,
      Name = t.Name
    }).ToList();
  }
}