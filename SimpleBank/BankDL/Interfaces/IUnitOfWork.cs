namespace BankDL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IUserRepository UserRepository { get; }

        Task CommitAsync();
    }
}
