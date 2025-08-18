using AutoMapper;
using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.DTO.Inquiry;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Helper;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class InquiryService : IInquiryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMapper _mapper;

    public InquiryService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
        _mapper = mapper;
    }
    public async Task<IEnumerable<Inquiry>> GetAllInquiries()
    {
        return await _unitOfWork.InquiryRepository.GetAllAsync();
    }

    public async Task<Inquiry?> GetInquiryById(string id)
    {
        return await _unitOfWork.InquiryRepository.GetByIdAsync(id);
    }

    public async Task<string> CreateInquiry(InquiryCreationDto inquiryCreationDto)
    {
        Console.WriteLine("HERE");
        var userId = _contextAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            throw new Exception("User not authenticated.");

        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception($"Please log in first");

        inquiryCreationDto.UserId = userId;
        var inquiry = _mapper.Map<Inquiry>(inquiryCreationDto);

        await _unitOfWork.InquiryRepository.InsertAsync(inquiry);
        await _unitOfWork.SaveChangesAsync();
        return inquiry.Id;
    }


    public async Task SoftDeleteInquiry(object id)
    {
        var inquiry = _unitOfWork.InquiryRepository.GetByIdAsync(id);
        await _unitOfWork.InquiryRepository.SoftDeleteAsync(inquiry.Result);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateInquiry(Inquiry Inquiry)
    {
        var existingInquiry = _unitOfWork.InquiryRepository.GetByIdAsync(Inquiry.Id);
        await _unitOfWork.InquiryRepository.UpdateAsync(Inquiry);
        await _unitOfWork.SaveChangesAsync();
    }
}
