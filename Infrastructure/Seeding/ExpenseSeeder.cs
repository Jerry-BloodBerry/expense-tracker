using System.Text.Json;
using Core.Domain;
using Core.Interfaces;
using System.Globalization;

namespace Infrastructure.Seeding;

public class ExpenseSeeder : ISeeder
{
  private readonly IGenericRepository<Expense> _expenseRepository;
  private readonly IGenericRepository<Category> _categoryRepository;
  private readonly IGenericRepository<Tag> _tagRepository;
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public ExpenseSeeder(
      IGenericRepository<Expense> expenseRepository,
      IGenericRepository<Category> categoryRepository,
      IGenericRepository<Tag> tagRepository)
  {
    _expenseRepository = expenseRepository;
    _categoryRepository = categoryRepository;
    _tagRepository = tagRepository;
  }

  public async Task SeedAsync()
  {
    if (_expenseRepository.Exists(1))
    {
      return;
    }

    var expensesJson = await File.ReadAllTextAsync("../Infrastructure/Seeding/SeedData/expenses.json");
    var expenses = JsonSerializer.Deserialize<List<ExpenseSeedData>>(expensesJson, _jsonOptions)!;

    foreach (var expenseData in expenses)
    {
      var category = await _categoryRepository.GetByIdAsync(expenseData.CategoryId, CancellationToken.None);
      if (category == null) continue;

      var date = DateOnly.Parse(expenseData.Date, CultureInfo.InvariantCulture);

      var expense = new Expense(
          expenseData.Name,
          category,
          expenseData.Amount,
          date,
          expenseData.Currency
      );

      if (!string.IsNullOrEmpty(expenseData.Description))
      {
        expense.SetDescription(expenseData.Description);
      }

      if (expenseData.IsRecurring && !string.IsNullOrEmpty(expenseData.RecurrenceInterval))
      {
        expense.SetAsRecurring(Enum.Parse<RecurrenceInterval>(expenseData.RecurrenceInterval));
      }

      var tags = await _tagRepository.ListAllAsync(CancellationToken.None);
      var expenseTags = tags.Where(t => expenseData.TagIds.Contains(t.Id)).ToList();
      expense.UpdateTags(expenseTags);

      _expenseRepository.Add(expense);
    }

    await _expenseRepository.SaveChangesAsync(CancellationToken.None);
  }

  public sealed class ExpenseSeedData
  {
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Date { get; init; } = null!;
    public string? Description { get; init; }
    public string Currency { get; init; } = null!;
    public int CategoryId { get; init; }
    public List<int> TagIds { get; init; } = [];
    public bool IsRecurring { get; init; }
    public string? RecurrenceInterval { get; init; }
  }
}