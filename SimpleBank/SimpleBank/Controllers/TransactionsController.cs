using BankAPI.DTO.TransactionDTO;
using BankDL.Entities;
using BankDL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public string AuthorizedUserId
		{
			get
			{
				return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			}
		}

		public TransactionsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpPatch("deposit")]
		public async Task<IActionResult> DepositFunds([FromBody] double funds, CancellationToken cancellationToken)
		{
			UserEntity? userEntity = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(AuthorizedUserId), cancellationToken, u => u.Account!);

			if (userEntity == null)
			{
				return NotFound("Account is not found.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(userEntity.Account!.Id, () => userEntity.Account.Balance += funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}

		[HttpPatch("withdraw")]
		public async Task<IActionResult> WithdrawFunds([FromBody] double funds, CancellationToken cancellationToken)
		{
			UserEntity? userEntity = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(AuthorizedUserId), cancellationToken, u => u.Account!);

			if (userEntity == null)
			{
				return NotFound("Account is not found.");
			}

			if (userEntity.Account!.Balance < funds)
			{
				return BadRequest("Not enough money.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(userEntity.Account.Id, () => userEntity.Account.Balance -= funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> TransferFunds([FromBody] TransferDTO transferDTO, CancellationToken cancellationToken)
		{
			UserEntity? userEntity = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(AuthorizedUserId), cancellationToken, u => u.Account!);

			AccountEntity? receiverAccountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(transferDTO.ReceiverAccountNumber, cancellationToken);

			if (receiverAccountEntity == null)
			{
				return NotFound("Receiver's account is not found.");
			}

			if (userEntity!.Account!.Id == receiverAccountEntity.Id)
			{
				return BadRequest("Received account can't be the same as sender account.");
			}

			if (userEntity.Account.Balance < transferDTO.Funds)
			{
				return BadRequest("Sender's account doesn't have enough money.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(userEntity.Account.Id, () => userEntity.Account.Balance -= transferDTO.Funds);
			await _unitOfWork.AccountRepository.UpdateFundsAsync(receiverAccountEntity.Id, () => receiverAccountEntity.Balance += transferDTO.Funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}
	}
}
