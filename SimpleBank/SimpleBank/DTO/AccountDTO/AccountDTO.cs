namespace BankAPI.DTO.AccountDTO
{
	public class AccountDTO
	{
        public Guid Id { get; set; }

        public required int AccountNumber { get; set; }

        public Guid HolderId { get; set; }
    }
}
