using FastEndpoints;
using Core.Features.Expenses.Commands;
using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Infrastructure.Data.Repository;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.HttpOverrides;
using API.Swagger;
using Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Expense Tracker API";
            s.Version = "v1";
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.OperationFilter<ResponseExampleSwaggerOperationFilter>();
    options.SchemaFilter<PascalCaseEnumSchemaFilter>();
    options.SupportNonNullableReferenceTypes();
});
// Configure HTTPS redirection
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Known networks that can forward headers
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddMediatR(typeof(CreateExpenseCommand).Assembly);
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add DataSeeder
builder.Services.AddScoped<DataSeeder>();

// Add Seeders
builder.Services.AddScoped<CategorySeeder>();
builder.Services.AddScoped<TagSeeder>();
builder.Services.AddScoped<ExpenseSeeder>();

builder.Services.AddAuthorization();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(c => c
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:8080", "https://localhost:8080")
);

// Enable forwarded headers
app.UseForwardedHeaders();

// Always use HTTPS redirection
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }

    if (app.Environment.IsDevelopment())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }
}

app.Run();
