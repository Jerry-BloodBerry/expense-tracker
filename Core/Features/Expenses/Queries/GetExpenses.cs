using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Expenses.Queries;

public class GetExpensesQuery : IRequest<ExpensesResult>
{
  public DateTime? StartDate { get; init; }
  public DateTime? EndDate { get; init; }
  public int? CategoryId { get; init; }
  public List<int> TagIds { get; init; } = [];
  public decimal? MinAmount { get; init; }
  public decimal? MaxAmount { get; init; }
  public bool? IsRecurring { get; init; }
  public int Page { get; init; } = 1;
  public int PageSize { get; init; } = 10;
}

public class ExpensesResult
{
  public List<ExpenseDto> Items { get; set; } = [];
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class GetExpensesHandler : IRequestHandler<GetExpensesQuery, ExpensesResult>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpensesHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ExpensesResult> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
  {
    var getSpec = new GetAllExpensesSpecification(request);
    var expenses = await _expenseRepository.ListAsync(getSpec, cancellationToken);
    var totalCount = await _expenseRepository.CountAsync(getSpec, cancellationToken);
    return new ExpensesResult
    {
      Items = expenses.Select(e => new ExpenseDto
      {
        Id = e.Id,
        Name = e.Name,
        Amount = e.Amount,
        Date = e.Date,
        Description = e.Description,
        Currency = e.Currency,
        IsRecurring = e.IsRecurring,
        RecurrenceInterval = e.RecurrenceInterval,
        Category = new CategoryDto
        {
          Id = e.Category.Id,
          Name = e.Category.Name,
          Description = e.Category.Description
        },
        Tags = e.Tags.Select(t => new TagDto
        {
          Id = t.Id,
          Name = t.Name
        }).ToList()
      }).ToList(),
      TotalCount = totalCount,
      Page = request.Page,
      PageSize = request.PageSize
    };
  }
}