using BankDL.Interfaces;
using BankDL.Repositories;

namespace BankDL.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankDbContext _dbContext;
        private IAccountRepository? _accountRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAccountRepository AccountRepository
        {
            get { return _accountRepository ??= new AccountRepository(_dbContext); }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(_dbContext); }
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
