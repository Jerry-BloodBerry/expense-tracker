using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Categories.Commands;

public record UpdateCategoryCommand : IRequest<Unit>
{
  public int Id { get; init; }
  public string Name { get; init; }
  public string? Description { get; init; }
}

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public UpdateCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Category with ID {request.Id} not found");

    category.UpdateName(request.Name);
    category.UpdateDescription(request.Description);

    await _categoryRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}