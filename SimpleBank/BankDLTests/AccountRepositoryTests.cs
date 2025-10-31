using BankDL.DataAccess;
using BankDL.Models;
using BankDL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankDLTests
{
	public class AccountRepositoryTests
	{
		private DbContextOptions<BankDbContext> _dbContextOptions;
		private BankDbContext _dbContext;
		private AccountRepository _accountRepository;

		[SetUp]
		public void SetUp()
		{
			_dbContextOptions = new DbContextOptionsBuilder<BankDbContext>()
					   .UseInMemoryDatabase(Guid.NewGuid().ToString())
					   .Options;

			_dbContext = new BankDbContext(_dbContextOptions);
			_accountRepository = new AccountRepository(_dbContext);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetDetailsByAccountNumberAsync_WhenAccountExists_ReturnsAccountWithUser()
		{
			// Arrange
			UserEntity expectedUser = CreateUser(phone: "111-222");
			AccountEntity expectedAccount = CreateAccount(accNumber: 777777, initialBalance: 200d, user: expectedUser);

			_dbContext.Users.Add(expectedUser);
			_dbContext.Accounts.Add(expectedAccount);
			await _dbContext.SaveChangesAsync();

			// Act
			AccountEntity? actualAccount = await _accountRepository.GetDetailsByAccountNumberAsync(
										777777,
										CancellationToken.None);

			// Assert
			Assert.That(actualAccount, Is.Not.Null);
			Assert.That(actualAccount!.AccountNumber, Is.EqualTo(777777));
			Assert.That(actualAccount!.User, Is.Not.Null);
			Assert.That(actualAccount!.User!.PhoneNumber, Is.EqualTo("111-222"));
		}

		[Test]
		public async Task GetDetailsByAccountNumberAsync_WhenAccountDoesNotExist_ReturnsNull()
		{
			// Act
			AccountEntity? actualAccount = await _accountRepository.GetDetailsByAccountNumberAsync(
												999999,
												CancellationToken.None);

			// Assert
			Assert.That(actualAccount, Is.Null);
		}


		[Test]
		public async Task ExistAccountByUserPhoneNumberAsync_WhenPhoneExists_ReturnsTrue()
		{
			// Arrange
			UserEntity expectedUser = CreateUser(phone: "333-444");
			AccountEntity expectedAccount = CreateAccount(accNumber: 101010, user: expectedUser);

			_dbContext.Users.Add(expectedUser);
			_dbContext.Accounts.Add(expectedAccount);
			await _dbContext.SaveChangesAsync();

			// Act
			bool isExist = await _accountRepository.ExistAccountByUserPhoneNumberAsync(
										"333-444",
										CancellationToken.None);

			// Assert
			Assert.That(isExist, Is.True);
		}

		[Test]
		public async Task ExistAccountByUserPhoneNumberAsync_WhenPhoneDoesNotExist_ReturnsFalse()
		{
			// Act
			bool isExist = await _accountRepository.ExistAccountByUserPhoneNumberAsync(
										"000-000",
										CancellationToken.None);

			// Assert
			Assert.That(isExist, Is.False);
		}

		[Test]
		public async Task UpdateFundsAsync_ExecutesProvidedOperation()
		{
			// Arrange
			AccountEntity expectedAccount = CreateAccount(initialBalance: 100d);
			_dbContext.Accounts.Add(expectedAccount);
			await _dbContext.SaveChangesAsync();

			// Act
			await _accountRepository.UpdateFundsAsync(expectedAccount.Id, () => expectedAccount.Balance += 50d);
			await _dbContext.SaveChangesAsync();

			// Assert
			await using var assertCtx = new BankDbContext(_dbContextOptions);
			AccountEntity actualAccount = await assertCtx.Accounts
										  .SingleAsync(a => a.Id == expectedAccount.Id);

			Assert.That(actualAccount.Balance, Is.EqualTo(150m));
		}

		private static UserEntity CreateUser(Guid? id = null,
											 string? phone = null) =>
			new UserEntity
			{
				Id = id ?? Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				PhoneNumber = phone ?? "555-1234"
			};

		private static AccountEntity CreateAccount(Guid? id = null,
												   int? accNumber = null,
												   double? initialBalance = null,
												   UserEntity? user = null)
		{
			AccountEntity account = new AccountEntity
			{
				Id = id ?? Guid.NewGuid(),
				AccountNumber = accNumber ?? 123456,
				Balance = initialBalance ?? 0d,
				User = user
			};

			if (user is not null)
			{
				account.UserId = user.Id;
			}

			return account;
		}
	}
}