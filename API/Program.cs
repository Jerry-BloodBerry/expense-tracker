using FastEndpoints;
using Core.Features.Expenses.Commands;
using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Infrastructure.Data.Repository;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.HttpOverrides;

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

// Configure HTTPS redirection to use port 5001
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable forwarded headers
app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

// Apply migrations on startup
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
}

app.Run();
