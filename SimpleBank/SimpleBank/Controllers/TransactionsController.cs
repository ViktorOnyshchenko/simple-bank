using BankAPI.DTO.TransactionDTO;
using BankDL.Interfaces;
using BankDL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public TransactionsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpPatch("deposit")]
		public async Task<IActionResult> DepositFunds([FromBody] TransactionDTO transactionDTO, CancellationToken cancellationToken)
		{
			AccountEntity? accountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(transactionDTO.AccountNumber, cancellationToken);

			if (accountEntity == null)
			{
				return NotFound("Account is not found.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(accountEntity.Id, () => accountEntity.Balance += transactionDTO.Funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}

		[HttpPatch("withdraw")]
		public async Task<IActionResult> WithdrawFunds([FromBody] TransactionDTO transactionDTO, CancellationToken cancellationToken)
		{
			AccountEntity? accountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(transactionDTO.AccountNumber, cancellationToken);

			if (accountEntity == null)
			{
				return NotFound("Account is not found.");
			}

			if (accountEntity.Balance < transactionDTO.Funds)
			{
				return BadRequest("Not enough money.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(accountEntity.Id, () => accountEntity.Balance -= transactionDTO.Funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> TransferFunds([FromBody] TransferDTO transferDTO, CancellationToken cancellationToken)
		{
			AccountEntity? senderAccountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(transferDTO.SenderAccountNumber, cancellationToken);

			if (senderAccountEntity == null)
			{
				return NotFound("Sender's account is not found.");
			}

			AccountEntity? receiverAccountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(transferDTO.ReceiverAccountNumber, cancellationToken);

			if (receiverAccountEntity == null)
			{
				return NotFound("Receiver's account is not found.");
			}

			if (senderAccountEntity.Balance < transferDTO.Funds)
			{
				return BadRequest("Sender's account doesn't have enough money.");
			}

			await _unitOfWork.AccountRepository.UpdateFundsAsync(senderAccountEntity.Id, () => senderAccountEntity.Balance -= transferDTO.Funds);
			await _unitOfWork.AccountRepository.UpdateFundsAsync(receiverAccountEntity.Id, () => receiverAccountEntity.Balance += transferDTO.Funds);
			await _unitOfWork.CommitAsync();

			return Ok();
		}
	}
}
