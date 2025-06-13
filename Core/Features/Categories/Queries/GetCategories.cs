using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<ErrorOr<List<CategoryDto>>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, ErrorOr<List<CategoryDto>>>
{
  private readonly IGenericRepository<Category> _categoryRepository;

  public GetCategoriesHandler(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<ErrorOr<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
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