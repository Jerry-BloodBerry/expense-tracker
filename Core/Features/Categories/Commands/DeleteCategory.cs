using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest<Unit>;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public DeleteCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Category with ID {request.Id} not found");

    _categoryRepository.Delete(category);
    await _categoryRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}