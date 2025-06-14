using Core.Domain;
using Core.Interfaces;

namespace Infrastructure.Seeding;

public class DataSeeder
{
  private readonly IGenericRepository<Expense> _expenseRepository;
  private readonly CategorySeeder _categorySeeder;
  private readonly TagSeeder _tagSeeder;
  private readonly ExpenseSeeder _expenseSeeder;

  public DataSeeder(
      IGenericRepository<Expense> expenseRepository,
      CategorySeeder categorySeeder,
      TagSeeder tagSeeder,
      ExpenseSeeder expenseSeeder)
  {
    _expenseRepository = expenseRepository;
    _categorySeeder = categorySeeder;
    _tagSeeder = tagSeeder;
    _expenseSeeder = expenseSeeder;
  }

  public async Task SeedAsync()
  {
    // Check if data already exists
    if (_expenseRepository.Exists(1))
    {
      return;
    }

    // Seed in correct order: Categories -> Tags -> Expenses
    await _categorySeeder.SeedAsync();
    await _tagSeeder.SeedAsync();
    await _expenseSeeder.SeedAsync();
  }
}