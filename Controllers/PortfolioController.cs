using BankFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(ITransactionService transactionService, ILogger<PortfolioController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPortfolio()
    {
        _logger.LogInformation("Retrieving portfolio data for current user");
        
        // Mocking user ID 1 for demonstration
        var account = await _transactionService.GetAccountPortfolioAsync(1);
        
        if (account == null)
            return NotFound("Account not found");

        return Ok(new {
            balance = account.Balance,
            currency = account.Currency,
            monthlyIncome = account.MonthlyIncome,
            monthlyExpenses = account.MonthlyExpenses,
            trend = account.Trend,
            // Mock chart data for UI
            chartData = new[] { 100000, 105000, 103000, 110000, 115000, 120000, account.Balance }
        });
    }
}
