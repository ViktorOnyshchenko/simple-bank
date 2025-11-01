using BankDL.DataAccess;
using BankDL.Entities;
using BankDL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankDL.Repositories
{
	public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        public UserRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }

		public async Task<UserEntity> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
		{
			return await _dbSet.SingleAsync(u => u.PhoneNumber!.Equals(phoneNumber));
		}
	}
}
