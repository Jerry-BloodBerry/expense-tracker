using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest<ErrorOr<Unit>>;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ErrorOr<Unit>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public DeleteCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<ErrorOr<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
    if (category is null)
      return CategoryErrors.NotFound(request.Id);

    _categoryRepository.Delete(category);
    await _categoryRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}