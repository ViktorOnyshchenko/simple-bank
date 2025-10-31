namespace BankAPI.DTO.TransactionDTO
{
	public class TransactionDTO
	{
        public required int AccountNumber { get; set; }
        public required double Funds { get; set; }
    }
}
