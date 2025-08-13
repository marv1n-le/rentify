using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Implement;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}