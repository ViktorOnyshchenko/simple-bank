namespace BankAPI.DTO.AccountDTO
{
    public class AccountDetailedDTO
    {
        public Guid Id { get; set; }

        public required int AccountNumber { get; set; }

        public required string HolderName { get; set; }

        public required string HolderPhoneNumber { get; set; }

        public required double Balance { get; set; }
    }
}
