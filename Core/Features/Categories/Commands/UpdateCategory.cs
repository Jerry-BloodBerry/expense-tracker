using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Categories.Commands;

public record UpdateCategoryCommand : IRequest<ErrorOr<CategoryDto>>
{
  public int Id { get; init; }
  public string Name { get; init; }
  public string? Description { get; init; }
}

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ErrorOr<CategoryDto>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public UpdateCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<ErrorOr<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
    if (category is null)
      return CategoryErrors.NotFound(request.Id);

    try
    {
      category.UpdateName(request.Name);
      category.UpdateDescription(request.Description);

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