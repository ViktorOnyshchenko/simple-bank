using BankDL.DataAccess;
using BankDL.Interfaces;
using BankDL.Models;

namespace BankDL.Repositories
{
	public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        public UserRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
        }
    }
}
