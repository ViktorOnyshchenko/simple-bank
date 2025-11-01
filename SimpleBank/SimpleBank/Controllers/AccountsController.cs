using BankAPI.DTO.AccountDTO;
using BankAPI.DTO.UserDTO;
using BankDL.Entities;
using BankDL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<UserEntity> _userManager;
		private readonly SignInManager<UserEntity> _userSignInManager;

		public AccountsController(
			IUnitOfWork unitOfWork,
			UserManager<UserEntity> userManager,
			SignInManager<UserEntity> userSignInManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_userSignInManager = userSignInManager;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAccounts(CancellationToken cancellationToken)
		{
			IEnumerable<AccountEntity> accountEntities = await _unitOfWork.AccountRepository.GetAllAsync(cancellationToken);

			if (!accountEntities.Any())
			{
				return NotFound("There are no accounts.");
			}

			List<AccountDTO> accountDTOs = new();

			foreach (AccountEntity accountEntity in accountEntities)
			{
				AccountDTO accountDTO = new()
				{
					Id = accountEntity.Id,
					AccountNumber = accountEntity.AccountNumber,
					HolderId = accountEntity.UserId
				};

				accountDTOs.Add(accountDTO);
			}

			return Ok(accountDTOs);
		}

		[HttpGet("{accountNumber}")]
		public async Task<IActionResult> GetAccountDetailsByNumber([FromRoute] int accountNumber, CancellationToken cancellationToken)
		{
			AccountEntity? accountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(accountNumber, cancellationToken);

			if (accountEntity == null)
			{
				return NotFound($"Account with {accountNumber} is not found.");
			}

			if (accountEntity.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
			{
				return Forbid("Not enough rights.");
			}

			AccountDetailedDTO accountDTO = new()
			{
				Id = accountEntity.Id,
				AccountNumber = accountEntity.AccountNumber,
				HolderName = accountEntity.User!.UserName!,
				HolderPhoneNumber = accountEntity.User.PhoneNumber!,
				Balance = accountEntity.Balance
			};

			return Ok(accountDTO);
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> CreateAccount([FromBody] UserDTO user, CancellationToken cancellationToken)
		{
			bool isExistAccount = await _unitOfWork.AccountRepository.ExistAccountByUserPhoneNumberAsync(user.PhoneNumber, cancellationToken);

			if (isExistAccount)
			{
				return Created();
			}

			UserEntity? userEntity = new()
			{
				UserName = string.Join(" ", user.FirstName, user.LastName),
				PhoneNumber = user.PhoneNumber
			};

			AccountEntity accountEnitity = new()
			{
				User = userEntity
			};

			await _unitOfWork.AccountRepository.CreateAsync(accountEnitity, cancellationToken);
			await _userManager.AddPasswordAsync(userEntity, user.Password);
			await _unitOfWork.CommitAsync();

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("signin")]
		public async Task<IActionResult> SignInAccount([FromBody] SignInUserDTO user, CancellationToken cancellationToken)
		{
			bool isExistAccount = await _unitOfWork.AccountRepository.ExistAccountByUserPhoneNumberAsync(user.PhoneNumber, cancellationToken);

			if (!isExistAccount)
			{
				return NotFound("Account is not found.");
			}

			UserEntity userEntity = await _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(user.PhoneNumber, cancellationToken);

			bool isPasswordCorrect = await _userManager.CheckPasswordAsync(userEntity, user.Password);

			if (!isPasswordCorrect)
			{
				return Unauthorized("Incorrect credentials");
			}

			await _userSignInManager.SignInAsync(userEntity, false);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("signout")]
		public async Task<IActionResult> SignOutAccount(CancellationToken cancellationToken)
		{
			string? authorizedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!string.IsNullOrWhiteSpace(authorizedUserId))
			{
				await _userSignInManager.SignOutAsync();
			}

			return Ok();
		}
	}
}
