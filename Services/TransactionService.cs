using BankFlow.Data;
using BankFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BankFlow.Services;

public class TransactionService : ITransactionService
{
    private readonly BankFlowDbContext _context;

    public TransactionService(BankFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetAccountPortfolioAsync(int accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int accountId)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction> ProcessTransactionAsync(int accountId, decimal amount, string description, string type)
    {
        var account = await _context.Accounts.FindAsync(accountId) 
                      ?? throw new ArgumentException("Account not found");

        var transaction = new Transaction
        {
            TransactionId = "TX" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
            Date = DateTime.UtcNow,
            Description = description,
            Amount = amount,
            Type = type,
            Status = "completed",
            AccountId = accountId
        };

        if (type == "credit")
        {
            account.Balance += amount;
            account.MonthlyIncome += amount;
        }
        else
        {
            account.Balance -= amount;
            account.MonthlyExpenses += amount;
        }

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}
