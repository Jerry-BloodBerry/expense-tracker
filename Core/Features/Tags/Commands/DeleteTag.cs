using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Tags.Commands;

public record DeleteTagCommand(int Id) : IRequest<ErrorOr<Success>>;

public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
  private readonly IGenericRepository<Tag> _tagRepository;

  public DeleteTagHandler(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<Success>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
  {
    var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
    if (tag is null)
      return TagErrors.NotFound(request.Id);

    _tagRepository.Delete(tag);
    await _tagRepository.SaveChangesAsync(cancellationToken);

    return Result.Success;
  }
}