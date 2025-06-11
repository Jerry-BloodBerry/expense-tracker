using Core.Domain;
using Core.Interfaces;
using MediatR;
using CategoryDto = Features.Expenses.CategoryDto;
using ExpenseDto = Features.Expenses.ExpenseDto;
using TagDto = Features.Expenses.TagDto;

namespace Core.Features.Expenses.Queries;

public record GetExpenseQuery(int Id) : IRequest<ExpenseDto>;

public class GetExpenseHandler : IRequestHandler<GetExpenseQuery, ExpenseDto>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
  {
    var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

    return new ExpenseDto
    {
      Id = expense.Id,
      Name = expense.Name,
      Amount = expense.Amount,
      Date = expense.Date,
      Description = expense.Description,
      Currency = expense.Currency,
      IsRecurring = expense.IsRecurring,
      RecurrenceInterval = expense.RecurrenceInterval,
      Category = new CategoryDto
      {
        Id = expense.Category.Id,
        Name = expense.Category.Name,
        Description = expense.Category.Description
      },
      Tags = expense.Tags.Select(t => new TagDto
      {
        Id = t.Id,
        Name = t.Name
      }).ToList()
    };
  }
}