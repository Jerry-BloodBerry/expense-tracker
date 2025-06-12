using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Tags.Commands;

public record CreateTagCommand : IRequest<ErrorOr<TagDto>>
{
  public required string Name { get; init; }
}

public class CreateTagHandler : IRequestHandler<CreateTagCommand, ErrorOr<TagDto>>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public CreateTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<TagDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
  {
    var existingTags = await _tagRepository.ListAllAsync(cancellationToken);
    if (existingTags.Any(t => t.Name == request.Name))
    {
      return TagErrors.NameAlreadyExists(request.Name);
    }

    var tag = new Tag(request.Name);
    _tagRepository.Add(tag);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return new TagDto
    {
      Id = tag.Id,
      Name = tag.Name
    };
  }
}