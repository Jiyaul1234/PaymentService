using Ecommerce.PaymentService.Application.Interface.IRepository;
using Ecommerce.PaymentService.Domain.Model;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Ecommerce.PaymentService.Infrastructure.Repository
{
    public class PaymentRepository : BaseRepository<Ecommerce.PaymentService.Domain.Model.Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext dbContext, ILogger<BaseRepository<Ecommerce.PaymentService.Domain.Model.Payment>> logger) : base(dbContext, logger)
        {
        }
        public async Task<int> AddAsync(Ecommerce.PaymentService.Domain.Model.Payment entity)
        {
          await  base.AddAsync(entity);
          return entity.PaymentId;
        }

        public Task<IEnumerable<Ecommerce.PaymentService.Domain.Model.Payment>> FindAsync(Expression<System.Func<Ecommerce.PaymentService.Domain.Model.Payment, bool>> predicate) => base.FindAsync(predicate);

        public Task<IEnumerable<Ecommerce.PaymentService.Domain.Model.Payment>> GetAllAsync() => base.GetAllAsync();

        public Task<Ecommerce.PaymentService.Domain.Model.Payment> GetByIdAsync(object id) => base.GetByIdAsync(id);

        public Task Remove(Ecommerce.PaymentService.Domain.Model.Payment entity) => base.Remove(entity);

        public Task Update(Ecommerce.PaymentService.Domain.Model.Payment entity) => base.Update(entity);
    }
}
