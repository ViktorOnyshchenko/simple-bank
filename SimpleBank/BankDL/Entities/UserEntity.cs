using Microsoft.AspNetCore.Identity;

namespace BankDL.Entities
{
	public class UserEntity : IdentityUser<Guid>, IPersistedEntity
	{
		public AccountEntity? Account { get; set; }
	}
}
