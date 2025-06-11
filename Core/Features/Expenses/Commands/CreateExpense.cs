using Core.Domain;
using Core.Features.Tags.Specifications;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Expenses.Commands;

public record CreateExpenseCommand : IRequest<int>
{
  public string Name { get; init; }
  public int CategoryId { get; init; }
  public decimal Amount { get; init; }
  public DateTime Date { get; init; }
  public string Description { get; init; }
  public string Currency { get; init; }
  public bool IsRecurring { get; init; }
  public RecurrenceInterval? RecurrenceInterval { get; init; }
  public List<int> TagIds { get; init; } = new();
}

public class CreateExpenseHandler : IRequestHandler<CreateExpenseCommand, int>
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

  public async Task<int> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
        ?? throw new NotFoundException($"Category with ID {request.CategoryId} not found");

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

    if (request.TagIds.Any())
    {
      var tags = await _tagRepository.ListAsync(new TagsWithIdsSpecification(request.TagIds), cancellationToken);
      foreach (var tag in tags)
      {
        expense.AddTag(tag);
      }
    }

    _expenseRepository.Add(expense);
    await _expenseRepository.SaveChangesAsync(cancellationToken);

    return expense.Id;
  }
}