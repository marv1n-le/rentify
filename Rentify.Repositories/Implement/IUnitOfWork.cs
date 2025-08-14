using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Implement;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IPostRepository PostRepository { get; }
    IRentalRepository RentalRepository { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}