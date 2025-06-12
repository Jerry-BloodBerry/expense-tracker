using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Tags.Queries;

public record GetTagQuery(int Id) : IRequest<TagDto>;

public class GetTagHandler : IRequestHandler<GetTagQuery, TagDto>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public GetTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<TagDto> Handle(GetTagQuery request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Tag with ID {request.Id} not found");

    return new TagDto
    {
      Id = tag.Id,
      Name = tag.Name
    };
  }
}