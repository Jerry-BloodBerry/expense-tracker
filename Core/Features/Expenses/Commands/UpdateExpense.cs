using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Features.Tags.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Commands;

public record UpdateExpenseCommand : IRequest<ErrorOr<ExpenseDto>>
{
  public int Id { get; init; }
  public required string Name { get; init; }
  public int CategoryId { get; init; }
  public decimal Amount { get; init; }
  public DateOnly Date { get; init; }
  public string? Description { get; init; }
  public required string Currency { get; init; }
  public bool IsRecurring { get; init; }
  public string? RecurrenceInterval { get; init; }
  public List<int> TagIds { get; init; } = [];
}

public class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, ErrorOr<ExpenseDto>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;
  private readonly IGenericRepository<Category> _categoryRepository;
  private readonly IGenericRepository<Tag> _tagRepository;

  public UpdateExpenseHandler(
      IGenericRepository<Expense> expenseRepository,
      IGenericRepository<Category> categoryRepository,
      IGenericRepository<Tag> tagRepository)
  {
    _expenseRepository = expenseRepository;
    _categoryRepository = categoryRepository;
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<ExpenseDto>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);
      if (expense is null)
        return ExpenseErrors.NotFound(request.Id);

      var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
      if (category is null)
        return ExpenseErrors.CategoryNotFound(request.CategoryId);

      expense.UpdateName(request.Name);
      expense.UpdateCategory(category);
      expense.UpdateAmount(request.Amount);
      expense.SetDescription(request.Description);
      expense.SetDate(request.Date);
      expense.SetCurrency(request.Currency);

      if (request.IsRecurring && !string.IsNullOrEmpty(request.RecurrenceInterval))
      {
        if (!Enum.TryParse<RecurrenceInterval>(request.RecurrenceInterval, out var recurrenceInterval))
        {
          return ExpenseErrors.InvalidRecurrenceInterval(request.RecurrenceInterval);
        }
        expense.SetAsRecurring(recurrenceInterval);
      }
      else
      {
        expense.RemoveRecurrence();
      }

      if (request.TagIds.Count != 0)
      {
        var tags = await _tagRepository.ListAsync(new TagsWithIdsSpecification(request.TagIds), cancellationToken);
        if (tags.Count != request.TagIds.Count)
          return ExpenseErrors.TagNotFound(request.TagIds.First(t => !tags.Any(tt => tt.Id == t)));

        expense.UpdateTags(tags);
      }

      await _expenseRepository.SaveChangesAsync(cancellationToken);

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
        Category = new CategoryDto(expense.Category.Id),
        Tags = expense.Tags.Select(t => new TagDto(t.Id)).ToList()
      };
    }
    catch (ArgumentException ex)
    {
      return ExpenseErrors.Validation(ex.Message);
    }
  }
}