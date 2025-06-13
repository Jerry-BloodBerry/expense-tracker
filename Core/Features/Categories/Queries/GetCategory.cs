using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Categories.Queries;

public record GetCategoryQuery(int Id) : IRequest<ErrorOr<CategoryDto>>;

public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, ErrorOr<CategoryDto>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public GetCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<ErrorOr<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
    if (category is null)
      return CategoryErrors.NotFound(request.Id);

    return new CategoryDto
    {
      Id = category.Id,
      Name = category.Name,
      Description = category.Description
    };
  }
}