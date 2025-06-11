using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Categories.Commands;

public record CreateCategoryCommand : IRequest<int>
{
  public string Name { get; init; }
  public string? Description { get; init; }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public CreateCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = new Category(request.Name, request.Description);
    _categoryRepository.Add(category);
    await _categoryRepository.SaveChangesAsync(cancellationToken);

    return category.Id;
  }
}