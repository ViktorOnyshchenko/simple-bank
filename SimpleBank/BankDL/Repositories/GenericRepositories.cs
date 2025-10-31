using Microsoft.EntityFrameworkCore;
using BankDL.Interfaces;
using BankDL.DataAccess;
using BankDL.Models;

namespace BankDL.Repositories
{
	public class GenericRepository<T> : IRepository<T> where T : PersistedEntity
    {
        private readonly BankDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BankDbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.SingleOrDefaultAsync(acc => acc.Id == id, cancellationToken);
        }

        public async Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(acc => acc.Id == id, cancellationToken);
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken)
        {
			await _dbSet.AddAsync(entity, cancellationToken);
        }
	}
}
