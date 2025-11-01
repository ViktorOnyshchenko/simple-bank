using BankDL.Entities;

namespace BankDL.Interfaces
{
	public interface IUserRepository : IRepository<UserEntity>
	{
		public Task<UserEntity> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
	}
}
