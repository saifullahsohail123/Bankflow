using BankFlow.Models;

namespace BankFlow.Services;

public interface IStripePaymentService
{
    Task<bool> ProcessChargeAsync(StripeChargeRequest request);
}
