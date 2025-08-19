using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Repositories.Interface;

public interface IInquiryRepository : IGenericRepository<Inquiry>
{
    Task<List<Inquiry>> GetAllInquiryAsync();
    Task<Inquiry?> GetByIdAsync(string id);
    Task UpdateStatusAsync(string id, InquiryStatus status);
}
