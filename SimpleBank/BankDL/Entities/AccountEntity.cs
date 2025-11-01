namespace BankDL.Entities
{
	public class AccountEntity : IPersistedEntity
	{
		public Guid Id {  get; set; }

		public int AccountNumber { get; set; }

		public Guid UserId { get; set; }

		public double Balance { get; set; } = 0.0d;

		public UserEntity? User { get; set; }
	}
}
