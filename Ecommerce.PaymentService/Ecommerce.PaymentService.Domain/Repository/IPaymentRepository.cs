using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.PaymentService.Domain.Model;
using System;
using System.Linq.Expressions;

namespace Ecommerce.PaymentService.Domain.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(object id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate);
        Task AddAsync(Payment entity);
        Task Update(Payment entity);
        Task Remove(Payment entity);
    }
}
