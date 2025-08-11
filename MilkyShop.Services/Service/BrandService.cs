using MilkyShop.Repositories.Implement;
using MilkyShop.Services.Interface;

namespace MilkyShop.Services.Service;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BrandService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}