namespace BankDL.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

		Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(T entity, CancellationToken cancellationToken);
    }
}
