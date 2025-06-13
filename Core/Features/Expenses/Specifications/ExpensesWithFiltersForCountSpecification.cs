using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Specifications;

namespace Core.Features.Expenses.Specifications;

public class ExpensesWithFiltersForCountSpecification : BaseSpecification<Expense>
{
  public ExpensesWithFiltersForCountSpecification(GetExpensesQuery query)
      : base(e =>
          (!query.StartDate.HasValue || e.Date >= query.StartDate) &&
          (!query.EndDate.HasValue || e.Date <= query.EndDate) &&
          (!query.CategoryId.HasValue || e.Category.Id == query.CategoryId.Value) &&
          (!query.MinAmount.HasValue || e.Amount >= query.MinAmount) &&
          (!query.MaxAmount.HasValue || e.Amount <= query.MaxAmount) &&
          (!query.IsRecurring.HasValue || e.IsRecurring == query.IsRecurring) &&
          (query.TagIds.Count == 0 || e.Tags.Any(t => query.TagIds.Contains(t.Id))))
  {
  }
}