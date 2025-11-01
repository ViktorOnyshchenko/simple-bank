using BankDL.Entities;
using System.Linq.Expressions;

namespace BankDL.Interfaces
{
	public interface IRepository<T> where T : class, IPersistedEntity
	{
		Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, params Expression<Func<T, object>>[] includeProperties);

		Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includeProperties);

		Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(T entity, CancellationToken cancellationToken);
    }
}
