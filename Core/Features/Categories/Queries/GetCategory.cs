using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Categories.Queries;

public record GetCategoryQuery(int Id) : IRequest<CategoryDto>;

public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public GetCategoryHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Category with ID {request.Id} not found");

    return new CategoryDto
    {
      Id = category.Id,
      Name = category.Name,
      Description = category.Description
    };
  }
}