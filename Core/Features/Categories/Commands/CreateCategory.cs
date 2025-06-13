using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Categories.Commands;

public record CreateCategoryCommand : IRequest<ErrorOr<CategoryDto>>
{
  public string Name { get; init; }
  public string? Description { get; init; }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, ErrorOr<CategoryDto>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public CreateCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<ErrorOr<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var category = new Category(request.Name, request.Description);
      _categoryRepository.Add(category);
      await _categoryRepository.SaveChangesAsync(cancellationToken);

      return new CategoryDto
      {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
      };
    }
    catch (ArgumentException ex)
    {
      return Error.Validation(
        code: "Category.Validation",
        description: ex.Message);
    }
  }
}