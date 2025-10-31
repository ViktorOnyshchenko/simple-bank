namespace BankDL.Models
{
	public class UserEntity : PersistedEntity
	{
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string PhoneNumber { get; set; }

        public AccountEntity? Account { get; set; }
    }
}
