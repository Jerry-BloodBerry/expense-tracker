using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Features.Tags.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Commands;

public record CreateExpenseCommand : IRequest<ErrorOr<ExpenseDto>>
{
  public required string Name { get; init; }
  public int CategoryId { get; init; }
  public decimal Amount { get; init; }
  public DateTime Date { get; init; }
  public string? Description { get; init; }
  public required string Currency { get; init; }
  public bool IsRecurring { get; init; }
  public RecurrenceInterval? RecurrenceInterval { get; init; }
  public List<int> TagIds { get; init; } = [];
}

public class CreateExpenseHandler : IRequestHandler<CreateExpenseCommand, ErrorOr<ExpenseDto>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;
  private readonly IGenericRepository<Category> _categoryRepository;
  private readonly IGenericRepository<Tag> _tagRepository;

  public CreateExpenseHandler(
      IGenericRepository<Expense> expenseRepository,
      IGenericRepository<Category> categoryRepository,
      IGenericRepository<Tag> tagRepository)
  {
    _expenseRepository = expenseRepository;
    _categoryRepository = categoryRepository;
    _tagRepository = tagRepository;
  }

  public async Task<ErrorOr<ExpenseDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
      if (category is null)
        return ExpenseErrors.CategoryNotFound(request.CategoryId);

      var expense = new Expense(
          request.Name,
          category,
          request.Amount,
          request.Date,
          request.Currency);

      if (!string.IsNullOrEmpty(request.Description))
      {
        expense.SetDescription(request.Description);
      }

      if (request.IsRecurring && request.RecurrenceInterval.HasValue)
      {
        expense.SetAsRecurring(request.RecurrenceInterval.Value);
      }

      if (request.TagIds.Count != 0)
      {
        var tags = await _tagRepository.ListAsync(new TagsWithIdsSpecification(request.TagIds), cancellationToken);
        if (tags.Count != request.TagIds.Count)
          return ExpenseErrors.TagNotFound(request.TagIds.First(t => !tags.Any(tt => tt.Id == t)));

        foreach (var tag in tags)
        {
          expense.AddTag(tag);
        }
      }

      _expenseRepository.Add(expense);
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