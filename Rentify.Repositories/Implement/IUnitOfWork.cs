using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Implement;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IPostRepository PostRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRentalRepository RentalRepository { get; }
    ICategoryRepository CategoryRepository { get;  }
    IItemRepository ItemRepository { get; }
    
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}