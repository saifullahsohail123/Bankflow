namespace BankFlow.Models;

public class Transaction
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // credit or debit
    public string Status { get; set; } = "completed";
    
    public int AccountId { get; set; }
    public Account? Account { get; set; }
}
