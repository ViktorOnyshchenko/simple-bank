using BankDL.Models;

namespace BankDL.Interfaces
{
	public interface IAccountRepository : IRepository<AccountEntity>
	{
		public Task<AccountEntity?> GetDetailsByAccountNumberAsync(int accountNumber, CancellationToken cancellationToken);

		public Task<bool> ExistAccountByUserPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);

		public Task UpdateFundsAsync(Guid id, Action fundOperation);
	}
}
