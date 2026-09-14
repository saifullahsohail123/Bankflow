using BankFlow.Models;

namespace BankFlow.Services;

public interface ITransactionService
{
    Task<Account?> GetAccountPortfolioAsync(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int accountId);
    Task<Transaction> ProcessTransactionAsync(int accountId, decimal amount, string description, string type);
}
