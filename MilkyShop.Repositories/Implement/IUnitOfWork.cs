using MilkyShop.Repositories.Interface;

namespace MilkyShop.Repositories.Implement;

public interface IUnitOfWork
{
    IBrandRepository BrandRepository { get; }
    IUserRepository UserRepository { get; }
    IAddressRepository AddressRepository { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}