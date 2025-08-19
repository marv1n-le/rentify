using Rentify.Repositories.Interface;

namespace Rentify.Repositories.Implement;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IPostRepository PostRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRentalRepository RentalRepository { get; }
    IRentalItemRepository RentalItemRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IItemRepository ItemRepository { get; }
    IInquiryRepository InquiryRepository { get; }
    IOtpRepository OtpRepository { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}