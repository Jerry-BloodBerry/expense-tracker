using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Tags.Commands;

public record UpdateTagCommand : IRequest<Unit>
{
  public int Id { get; init; }
  public required string Name { get; init; }
}

public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Unit>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public UpdateTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Tag with ID {request.Id} not found");

    tag.UpdateName(request.Name);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}