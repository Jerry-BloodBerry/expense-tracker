using System.Linq;
using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Queries;

public class GetTopSpendingCategoryThisMonthQuery : IRequest<ErrorOr<TopCategoryResult>>
{
  public DateTime StartDate { get; init; }
  public DateTime EndDate { get; init; }
}

public class TopCategoryResult
{
  public int CategoryId { get; set; }
  public string CategoryName { get; set; } = string.Empty;
  public decimal TotalAmount { get; set; }
}

public class GetTopSpendingCategoryThisMonthHandler : IRequestHandler<GetTopSpendingCategoryThisMonthQuery, ErrorOr<TopCategoryResult>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetTopSpendingCategoryThisMonthHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<TopCategoryResult>> Handle(GetTopSpendingCategoryThisMonthQuery request, CancellationToken cancellationToken)
  {
    var spec = new GetAllExpensesSummarySpecification(request.StartDate, request.EndDate);
    var expenses = await _expenseRepository.ListAsync(spec, cancellationToken);

    if (expenses.Count == 0)
    {
      return new TopCategoryResult
      {
        CategoryId = 0,
        CategoryName = "N/A",
        TotalAmount = 0
      };
    }

    // Group expenses by category and sum amounts
    var categoryTotals = expenses
        .GroupBy(e => new { e.Category.Id, e.Category.Name })
        .Select(g => new TopCategoryResult
        {
          CategoryId = g.Key.Id,
          CategoryName = g.Key.Name,
          TotalAmount = g.Sum(e => e.Amount)
        })
        .OrderByDescending(c => c.TotalAmount)
        .FirstOrDefault();

    return categoryTotals ?? new TopCategoryResult
    {
      CategoryId = 0,
      CategoryName = "N/A",
      TotalAmount = 0
    };
  }
}

