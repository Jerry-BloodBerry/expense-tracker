using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Tags.Commands;

public record CreateTagCommand : IRequest<int>
{
  public required string Name { get; init; }
}

public class CreateTagHandler : IRequestHandler<CreateTagCommand, int>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public CreateTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
  {
    var tag = new Tag(request.Name);
    _tagRepository.Add(tag);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return tag.Id;
  }
}