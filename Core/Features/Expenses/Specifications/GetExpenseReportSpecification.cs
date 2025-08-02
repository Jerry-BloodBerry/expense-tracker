using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Specifications;

namespace Core.Features.Expenses.Specifications;

public class GetExpenseReportSpecification : BaseSpecification<Expense>
{
  public GetExpenseReportSpecification(GetExpenseReportQuery query)
      : base(e =>
          e.Date >= query.StartDate &&
          e.Date <= query.EndDate &&
          e.Currency == query.Currency &&
          (query.CategoryIds.Count == 0 || query.CategoryIds.Contains(e.Category.Id)) &&
          (query.TagIds.Count == 0 || e.Tags.Any(t => query.TagIds.Contains(t.Id))))
  {
    AddInclude(e => e.Category);
    AddInclude(e => e.Tags);
    AddOrderBy(e => e.Date);
  }
}