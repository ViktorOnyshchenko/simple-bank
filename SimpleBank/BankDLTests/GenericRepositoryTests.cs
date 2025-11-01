using BankDL.DataAccess;
using BankDL.Entities;
using BankDL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankDLTests
{
	[TestFixture]
	public class GenericRepositoryTests
	{
		private DbContextOptions<BankDbContext> _dbContextOptions;
		private BankDbContext _dbContext;
		private GenericRepository<AccountEntity> _genericRepository;

		[SetUp]
		public void SetUp()
		{
			_dbContextOptions = new DbContextOptionsBuilder<BankDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

			_dbContext = new BankDbContext(_dbContextOptions);
			_genericRepository = new GenericRepository<AccountEntity>(_dbContext);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetAllAsync_WhenCalled_ReturnsAllEntities()
		{
			// Arrange
			AccountEntity expectedAccount1 = CreateAccount();
			AccountEntity expectedAccount2 = CreateAccount();

			_dbContext.Accounts.AddRange(expectedAccount1, expectedAccount2);
			await _dbContext.SaveChangesAsync();

			// Act
			IEnumerable<AccountEntity> actualAccounts = await _genericRepository.GetAllAsync(CancellationToken.None);

			// Assert
			Assert.That(actualAccounts.Count(), Is.EqualTo(2));
			Assert.That(actualAccounts.Any(a => a.Id == expectedAccount1.Id));
			Assert.That(actualAccounts.Any(a => a.Id == expectedAccount2.Id));
		}

		[Test]
		public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
		{
			// Arrange
			AccountEntity expectedAccount = CreateAccount();
			_dbContext.Accounts.Add(expectedAccount);
			await _dbContext.SaveChangesAsync();

			// Act
			AccountEntity? actualAccount = await _genericRepository.GetByIdAsync(expectedAccount.Id, CancellationToken.None);

			// Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount!.Id, Is.EqualTo(expectedAccount.Id));
		}

		[Test]
		public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
		{
			// Act
			AccountEntity? expectedAccount = await _genericRepository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

			// Assert
			Assert.That(expectedAccount, Is.Null);
		}

		[Test]
		public async Task ExistByIdAsync_WhenEntityExists_ReturnsTrue()
		{
			// Arrange
			AccountEntity expectedAccount = CreateAccount();
			_dbContext.Accounts.Add(expectedAccount);
			await _dbContext.SaveChangesAsync();

			// Act
			bool isExist = await _genericRepository.ExistByIdAsync(expectedAccount.Id, CancellationToken.None);

			// Assert
			Assert.That(isExist, Is.True);
		}

		[Test]
		public async Task ExistByIdAsync_WhenEntityDoesNotExist_ReturnsFalse()
		{
			// Act
			bool isExist = await _genericRepository.ExistByIdAsync(Guid.NewGuid(), CancellationToken.None);

			// Assert
			Assert.That(isExist, Is.False);
		}

		[Test]
		public async Task CreateAsync_AddsEntityToDatabase()
		{
			// Arrange
			AccountEntity expectedAccount = CreateAccount();

			// Act
			await _genericRepository.CreateAsync(expectedAccount, CancellationToken.None);
			await _dbContext.SaveChangesAsync();

			// Assert
			await using BankDbContext assertCtx = new BankDbContext(_dbContextOptions);
			bool isExist = await assertCtx.Accounts.AnyAsync(a => a.Id == expectedAccount.Id);

			Assert.That(isExist, Is.True);
		}

		private static AccountEntity CreateAccount(Guid? id = null) =>
			new AccountEntity
			{
				User = new()
				{
					UserName = "TestUserName",
					PhoneNumber = "TestPhoneNumber"
				}
			};
	}
}
