using MilkyShop.BusinessObjects.Entities;
using MilkyShop.Repositories.Implement;
using MilkyShop.Services.Interface;

namespace MilkyShop.Services.Service;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public AddressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Address>> GetAllAddresses()
    {
        var addresses = _unitOfWork.AddressRepository.GetAllAsync();
        if (addresses == null)
        {
            throw new Exception("No addresses found.");
        }
        return await addresses;
    }
    
    public async Task CreateAsync(Address address)
    {
        await _unitOfWork.AddressRepository.InsertAsync(address);
        await _unitOfWork.SaveChangesAsync();
    }
}