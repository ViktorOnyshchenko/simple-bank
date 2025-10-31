using BankDL.DataAccess;
using BankDL.Interfaces;
using BankDL.Models;
using Microsoft.EntityFrameworkCore;

namespace BankDL.Repositories
{
	public class AccountRepository : GenericRepository<AccountEntity>, IAccountRepository
    {
		public AccountRepository(BankDbContext bankDbContext) : base(bankDbContext)
		{
		}

		public async Task<AccountEntity?> GetDetailsByAccountNumberAsync(int accountNumber, CancellationToken cancellationToken)
		{
            return await _dbSet.Include(acc => acc.User)
				.SingleOrDefaultAsync(acc => acc.AccountNumber.Equals(accountNumber), cancellationToken);
        }

		public async Task<bool> ExistAccountByUserPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
		{
            return await _dbSet.Include(acc => acc.User)
                .AnyAsync(acc => acc.User!.PhoneNumber.Equals(phoneNumber), cancellationToken);
        }

		public async Task UpdateFundsAsync(Guid id, Action fundsOperation)
		{
			AccountEntity accountEntity = await _dbSet.SingleAsync(acc => acc.Id == id);
			fundsOperation();
		}
	}
}
