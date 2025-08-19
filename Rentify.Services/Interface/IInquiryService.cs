using Rentify.BusinessObjects.DTO.Inquiry;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;

namespace Rentify.Services.Interface;

public interface IInquiryService
{
    Task<IEnumerable<Inquiry>> GetAllInquiries();
    Task<Inquiry?> GetInquiryById(string id);
    Task<string> CreateInquiry(InquiryCreationDto inquiryCreationDto);
    Task UpdateInquiry(Inquiry Inquiry);
    Task SoftDeleteInquiry(string id);
}
