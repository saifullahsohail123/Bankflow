namespace BankFlow.Models;

public class StripeChargeRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public string Source { get; set; } = string.Empty; // e.g., "tok_visa"
    public string Description { get; set; } = string.Empty;
}
