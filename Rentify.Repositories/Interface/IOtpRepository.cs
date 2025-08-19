using System.Linq.Expressions;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface;

public interface IOtpRepository : IGenericRepository<Otp>
{
    Task<IEnumerable<Otp>> FindAllAsync(Expression<Func<Otp, bool>> predicate);
    void DeleteRange(IEnumerable<Otp> entities);
}