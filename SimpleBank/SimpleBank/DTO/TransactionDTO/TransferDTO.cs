namespace BankAPI.DTO.TransactionDTO
{
	public class TransferDTO
	{
        public int ReceiverAccountNumber { get; set; }

        public double Funds { get; set; }
    }
}
