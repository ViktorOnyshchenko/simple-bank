namespace BankDL.Models
{
	public class AccountEntity : PersistedEntity
    {
		public int AccountNumber { get; set; }

		public Guid UserId { get; set; }

		public double Balance { get; set; } = 0.0d;

        public UserEntity? User { get; set; }
    }
}
