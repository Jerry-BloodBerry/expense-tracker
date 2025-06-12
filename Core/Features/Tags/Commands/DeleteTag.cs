using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Tags.Commands;

public record DeleteTagCommand(int Id) : IRequest<Unit>;

public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Unit>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public DeleteTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Tag with ID {request.Id} not found");

    _tagRepository.Delete(tag);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}