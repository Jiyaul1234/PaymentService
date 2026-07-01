using Ecommerce.PaymentService.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace Ecommerce.PaymentService.Infrastructure.Repository
{
    public class BaseRepository<T> where T : class
    {
        protected readonly AppDbContext dbContext;
        protected readonly ILogger<BaseRepository<T>> logger;

        public BaseRepository(AppDbContext dbContext, ILogger<BaseRepository<T>> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                logger.LogInformation("Adding entity of type {TypeName}", typeof(T).Name);
                await dbContext.Set<T>().AddAsync(entity);
                await dbContext.SaveChangesAsync();


                logger.LogInformation("Added entity of type {TypeName}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding entity of type {TypeName}", typeof(T).Name);
                throw;
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            try
            {
                logger.LogInformation("Finding entities of type {TypeName} with predicate", typeof(T).Name);
                return await dbContext.Set<T>().Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error finding entities of type {TypeName}", typeof(T).Name);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                logger.LogInformation("Getting all entities of type {TypeName}", typeof(T).Name);
                return await dbContext.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all entities of type {TypeName}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T> GetByIdAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            try
            {
                logger.LogInformation("Getting entity of type {TypeName} by id {Id}", typeof(T).Name, id);
                var entity = await dbContext.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    logger.LogInformation("Entity of type {TypeName} with id {Id} not found", typeof(T).Name, id);
                }
                else
                {
                    logger.LogInformation("Found entity of type {TypeName} with id {Id}", typeof(T).Name, id);
                }
                return entity == null ? null : entity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting entity of type {TypeName} by id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public async Task Remove(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                logger.LogInformation("Removing entity of type {TypeName}", typeof(T).Name);
                dbContext.Set<T>().Remove(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Removed entity of type {TypeName}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing entity of type {TypeName}", typeof(T).Name);
                throw;
            }
        }

        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                logger.LogInformation("Updating entity of type {TypeName}", typeof(T).Name);
                dbContext.Set<T>().Update(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Updated entity of type {TypeName}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating entity of type {TypeName}", typeof(T).Name);
                throw;
            }
        }
    }
}
