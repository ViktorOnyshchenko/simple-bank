using BankAPI.DTO.AccountDTO;
using BankAPI.DTO.UserDTO;
using BankDL.Interfaces;
using BankDL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        [HttpGet("/{accountNumber}")]
        public async Task<IActionResult> GetAccountDetailsByNumber([FromRoute] int accountNumber, CancellationToken cancellationToken)
        {
            AccountEntity? accountEntity = await _unitOfWork.AccountRepository.GetDetailsByAccountNumberAsync(accountNumber, cancellationToken);

            if (accountEntity == null) 
            {
                return NotFound($"Account with {accountNumber} is not found.");
            }

            AccountDetailedDTO accountDTO = new()
            {
                Id = accountEntity.Id,
                AccountNumber = accountEntity.AccountNumber,
                HolderName = string.Join(" ", accountEntity.User!.FirstName, accountEntity.User.LastName),
                HolderPhoneNumber = accountEntity.User.PhoneNumber,
                Balance = accountEntity.Balance
            };

            return Ok(accountDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] UserDTO user, CancellationToken cancellationToken)
        {
            bool isExistAccount = await _unitOfWork.AccountRepository.ExistAccountByUserPhoneNumberAsync(user.PhoneNumber, cancellationToken);

            if (isExistAccount)
            {
                return BadRequest("Account is already exist.");
            }

            UserEntity userEntity = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            AccountEntity accountEnitity = new()
            {
                User = userEntity
            };

            await _unitOfWork.AccountRepository.CreateAsync(accountEnitity, cancellationToken);
            await _unitOfWork.CommitAsync();

            return Ok();
        }
    }
}
