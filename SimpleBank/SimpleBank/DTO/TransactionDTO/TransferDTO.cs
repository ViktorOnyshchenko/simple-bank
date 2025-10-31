namespace BankAPI.DTO.TransactionDTO
{
	public class TransferDTO
	{
        public int SenderAccountNumber { get; set; }

        public int ReceiverAccountNumber { get; set; }

        public double Funds { get; set; }
    }
}
