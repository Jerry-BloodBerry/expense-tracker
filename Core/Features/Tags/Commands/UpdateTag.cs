using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Tags.Commands;

public record UpdateTagCommand : IRequest<ErrorOr<Success>>
{
  public int Id { get; init; }
  public required string Name { get; init; }
}

public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, ErrorOr<Success>>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public UpdateTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<Success>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
    if (tag is null)
      return TagErrors.NotFound(request.Id);

    var existingTags = await _tagRepository.ListAllAsync(cancellationToken);
    if (existingTags.Any(t => t.Name == request.Name && t.Id != request.Id))
      return TagErrors.NameAlreadyExists(request.Name);

    tag.UpdateName(request.Name);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return Result.Success;
  }
}