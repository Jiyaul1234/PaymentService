using Ecommerce.PaymentService.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.PaymentService.Application.Interface.IServices
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetPaymentAsync(int id);
        Task<IList<PaymentDto>> GetAllPaymentsAsync();
        Task CreatePaymentAsync(PaymentDto payment);
        Task UpdatePaymentAsync(PaymentDto payment);
        Task DeletePaymentAsync(int id);
    }
}
