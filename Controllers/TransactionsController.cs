using BankFlow.Models;
using BankFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IStripePaymentService _stripePaymentService;

    public TransactionsController(ITransactionService transactionService, IStripePaymentService stripePaymentService)
    {
        _transactionService = transactionService;
        _stripePaymentService = stripePaymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        // Mocking user ID 1 for demonstration
        var transactions = await _transactionService.GetTransactionHistoryAsync(1);
        
        var response = transactions.Select(t => new {
            id = t.TransactionId,
            date = t.Date.ToString("yyyy-MM-dd"),
            description = t.Description,
            amount = t.Amount,
            type = t.Type,
            status = t.Status
        });
        
        return Ok(response);
    }

    [HttpPost("charge")]
    public async Task<IActionResult> ProcessCharge([FromBody] StripeChargeRequest request)
    {
        var success = await _stripePaymentService.ProcessChargeAsync(request);
        
        if (!success)
            return BadRequest("Payment processing failed");

        // Mocking user ID 1
        var transaction = await _transactionService.ProcessTransactionAsync(1, request.Amount, request.Description, "credit");
        
        return Ok(new { success = true, transactionId = transaction.TransactionId });
    }
}
