using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public GetCategoriesHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
  {
    var categories = await _categoryRepository.ListAllAsync(cancellationToken);

    return categories.Select(c => new CategoryDto
    {
      Id = c.Id,
      Name = c.Name,
      Description = c.Description
    }).ToList();
  }
}