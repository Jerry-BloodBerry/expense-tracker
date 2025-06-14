# Expense Tracker

A modern, robust expense tracking application built with .NET 8 that helps you manage and analyze your personal finances. Track expenses, categorize spending, and gain insights into your financial habits.

## Features

- 💰 **Expense Management**: Create, update, and delete expenses with detailed information
- 📊 **Categorization**: Organize expenses into customizable categories
- 🏷️ **Tagging System**: Add multiple tags to expenses for better organization
- 💫 **Recurring Expenses**: Support for tracking recurring expenses
- 🔍 **Advanced Filtering**: Search and filter expenses by date, amount, category, and tags
- 📱 **Modern API**: RESTful API built with FastEndpoints for optimal performance
- 🔒 **Data Integrity**: PostgreSQL database with proper relationships and constraints

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop/) (for running PostgreSQL)
- [Docker Compose](https://docs.docker.com/compose/install/) (usually included with Docker Desktop)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/expense-tracker.git
   cd expense-tracker
   ```

2. Set up HTTPS development certificate (Windows):
   ```bash
   # Generate HTTPS development certificate
   dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password
   
   # Trust the certificate
   dotnet dev-certs https --trust
   ```
   Note: Replace `password` with the same value as `ASPNETCORE_Kestrel__Certificates__Default__Password` in docker-compose.yml

3. Start the PostgreSQL database using Docker Compose:
   ```bash
   docker-compose up -d db
   ```

4. Run the application:
   ```bash
   # Using Docker Compose (recommended)
   docker-compose up

   # Or run directly with .NET
   dotnet run --project API
   ```

5. The API will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

## API Documentation

Once the application is running, you can access the Swagger documentation at:
- http://localhost:5000/swagger
- https://localhost:5001/swagger

## Project Structure

- `API/` - Web API project with endpoints and configuration
- `Core/` - Domain models, interfaces, and business logic
- `Infrastructure/` - Data access, migrations, and external services

## Development

### Database Migrations

The project uses Entity Framework Core for database management. To update the database:

```bash
# From the root directory
dotnet ef database update --project Infrastructure --startup-project API
```

To create a new migration:

```bash
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

### Environment Variables

The application uses the following environment variables (configured in docker-compose.yml):

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Commit your changes: `git commit -am 'Add feature'`
4. Push to the branch: `git push origin feature-name`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. 