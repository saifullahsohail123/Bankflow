using BankFlow.Data;
using BankFlow.Models;
using BankFlow.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework Core with SQLite
builder.Services.AddDbContext<BankFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankFlow API", Version = "v1" });
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BankFlowDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Accounts.Any())
    {
        // Seed mock data
        var account = new Account 
        { 
            AccountNumber = "100234850", 
            AccountHolderName = "Saif Ullah",
            Balance = 124500.80m,
            Currency = "USD",
            MonthlyIncome = 15200.50m,
            MonthlyExpenses = 4300.25m,
            Trend = "+12.5%"
        };
        context.Accounts.Add(account);
        context.SaveChanges();

        context.Transactions.AddRange(
            new Transaction { AccountId = account.Id, TransactionId = "TX9012", Date = DateTime.Parse("2026-09-14"), Description = "Stripe Payout", Amount = 4500.00m, Type = "credit", Status = "completed" },
            new Transaction { AccountId = account.Id, TransactionId = "TX9011", Date = DateTime.Parse("2026-09-12"), Description = "AWS Cloud Services", Amount = 350.25m, Type = "debit", Status = "completed" },
            new Transaction { AccountId = account.Id, TransactionId = "TX9010", Date = DateTime.Parse("2026-09-10"), Description = "Github Copilot", Amount = 10.00m, Type = "debit", Status = "completed" },
            new Transaction { AccountId = account.Id, TransactionId = "TX9009", Date = DateTime.Parse("2026-09-08"), Description = "Client Retainer (TechCorp)", Amount = 8500.00m, Type = "credit", Status = "completed" },
            new Transaction { AccountId = account.Id, TransactionId = "TX9008", Date = DateTime.Parse("2026-09-05"), Description = "Office Rent", Amount = 1200.00m, Type = "debit", Status = "completed" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) // Always enable swagger for portfolio demo
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankFlow API v1"));
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
