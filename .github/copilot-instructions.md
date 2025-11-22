# AI Coding Instructions for Expense Tracker

## Architecture Overview

This is a **clean architecture** .NET 8 full-stack application:

- **API Layer** (`API/`): FastEndpoints-based REST API with Swagger documentation
- **Core Layer** (`Core/`): Domain models, MediatR commands/queries, repository interfaces, and business logic
- **Infrastructure Layer** (`Infrastructure/`): Entity Framework Core data access, PostgreSQL mappings, and seeding
- **Client** (`client/`): Angular 19 frontend with PrimeNG and TailwindCSS

### Data Flow Pattern
1. **FastEndpoint** (Request/Response) → **MediatR Command/Query** (Core) → **RequestHandler** (Core) → **IGenericRepository** → **EF Core/DbContext** → **Database**
2. Commands return `ErrorOr<TResult>` for functional error handling
3. All domain entities inherit `BaseEntity` (base Id property)

## Critical Patterns & Conventions

### Domain Entities (Core/Domain/*.cs)
- Use **aggregate roots** with private field collections: `private readonly List<Tag> _tags;` exposed via `IReadOnlyCollection<Tag> Tags`
- Entity state changes use methods: `expense.SetDescription()`, `expense.AddTag()`, `expense.SetAsRecurring()`
- Constructors validate invariants (non-null categories, positive amounts)
- EF Core uses **primary constructor** pattern: `public class StoreContext(DbContextOptions options)`

### Feature Structure
**Command example** (`Core/Features/Expenses/Commands/CreateExpense.cs`):
- Record class for command: `public record CreateExpenseCommand : IRequest<ErrorOr<ExpenseDto>>`
- Handler: `public class CreateExpenseHandler : IRequestHandler<CreateExpenseCommand, ErrorOr<ExpenseDto>>`
- Returns `ErrorOr<T>` using custom error definitions from `ExpenseErrors.cs`

**Endpoint example** (`API/Features/Expenses/CreateExpense.cs`):
- FastEndpoint class: `public class CreateExpenseEndpoint : Endpoint<CreateExpenseCommand, SingleResponse<ExpenseResponse>>`
- Route configuration in `Configure()` method
- Error handling via `ProblemResult.Of(result.Errors, HttpContext)`
- Response wrapping: `SingleResponse.Of(response)`

### Repository & Specifications
- Use `IGenericRepository<T>` for all data access
- Complex queries use **Specification pattern** (`Core/Features/.../Specifications/*.cs`):
  - `new TagsWithIdsSpecification(request.TagIds)` filters in repository method
  - `ApplyCriteria()` method chains LINQ where conditions
- Always pass `CancellationToken` to async methods

### Error Handling
- Define error types in feature error files: `Core/Features/{Feature}/{Feature}Errors.cs`
- Example: `ExpenseErrors.CategoryNotFound(categoryId)` returns `ErrorOr<ExpenseDto>`
- FastEndpoint endpoints check `result.IsError` and use `ProblemResult.Of()` for responses

## Key Build & Development Workflows

### Database Setup
```powershell
# Start PostgreSQL only (development)
docker-compose up -d db

# Run migrations
dotnet ef database update --project Infrastructure --startup-project API

# Create new migration (from root)
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

### Run Application
```powershell
# Full stack (API + Client + DB) - recommended
docker-compose up

# API only (requires running `docker-compose up -d db` first)
dotnet run --project API

# Client only
cd client && npm start
```

### HTTPS Development Certificate (Windows)
```powershell
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust
```

## Integration Points

### API-Client Communication
- API listens on `http://localhost:5000` and `https://localhost:5001`
- Client (Angular) at `http://localhost:4200` and `https://localhost:4200`
- CORS configured in `API/Program.cs` - update if adding new origins
- Client configured in `docker-compose.yml` for Docker deployments

### Database Configuration
- PostgreSQL 16 Alpine in Docker
- Connection string: `Host=db;Database=ExpenseTracker;Username=postgres;Password=postgres`
- Managed via EF Core in `Infrastructure/Data/StoreContext.cs`
- Seeding via `DataSeeder` and feature-specific seeders on app startup

### Swagger Documentation
- Auto-generated from FastEndpoint metadata
- Customizations in `API/Swagger/` (enum filters, response examples, error handling)
- Accessible at `/swagger` endpoint

## Project-Specific Standards

1. **Nullable References**: Project uses `#nullable enable` - respect non-null annotations
2. **Records over Classes**: Use `record` for request/command/query objects when immutable
3. **No Null Propagation**: Validate and throw early in handlers before repository calls
4. **Tag/Category Consistency**: Expenses must have exactly 1 Category, 0+ Tags (check specifications)
5. **Recurring Expenses**: Only valid if `IsRecurring=true` AND `RecurrenceInterval` is set
6. **Response Wrapping**: All API responses use `SingleResponse<T>` or `PaginatedResponse<T>` (see `API/Utils/Response/`)

## When Adding Features

1. Create domain entity in `Core/Domain/` if needed
2. Add entity configuration in `Infrastructure/Config/` (EF mappings)
3. Create command/query in `Core/Features/{Feature}/Commands|Queries/`
4. Create request handler in same file as command/query
5. Create FastEndpoint in `API/Features/{Feature}/`
6. Define feature-specific errors in `Core/Features/{Feature}/{Feature}Errors.cs`
7. Add specification in `Core/Features/{Feature}/Specifications/` if needed for complex queries
