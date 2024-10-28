namespace PurchasingSystemDeveloper.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAccountRepository User { get; }
        public IRoleRepository Role { get; }

        public UnitOfWork(IAccountRepository user, IRoleRepository role)
        {
            User = user;
            Role = role;
        }
    }

    public interface IUnitOfWork
    {
        IAccountRepository User { get; }
        IRoleRepository Role { get; }
    }
}
