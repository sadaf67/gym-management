using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IPaymentService
{
    Task<string> RequestPaymentAsync(int planId, string userId);
    Task<bool> VerifyPaymentAsync(string authority, string status, string userId);
    Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
}
