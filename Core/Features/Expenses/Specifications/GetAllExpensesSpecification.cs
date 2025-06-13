using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Specifications;

namespace Core.Features.Expenses.Specifications;

public class GetAllExpensesSpecification : BaseSpecification<Expense>
{
  public GetAllExpensesSpecification(GetExpensesQuery query)
      : base(e =>
          (!query.StartDate.HasValue || e.Date >= query.StartDate) &&
          (!query.EndDate.HasValue || e.Date <= query.EndDate) &&
          (!query.CategoryId.HasValue || e.Category.Id == query.CategoryId) &&
          (!query.MinAmount.HasValue || e.Amount >= query.MinAmount) &&
          (!query.MaxAmount.HasValue || e.Amount <= query.MaxAmount) &&
          (!query.IsRecurring.HasValue || e.IsRecurring == query.IsRecurring) &&
          (query.TagIds.Count == 0 || e.Tags.Any(t => query.TagIds.Contains(t.Id))))
  {
    AddOrderByDescending(e => e.Date);
    ApplyPaging((query.Page - 1) * query.PageSize, query.PageSize);
  }
}