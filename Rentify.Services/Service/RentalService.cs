using AutoMapper;
using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Repositories.Interface;
using Rentify.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rentify.Services.Service
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public RentalService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        IEnumerable<Rental> IRentalService.GetAll()
        {
            return _rentalRepository.GetAll();
        }

        async Task<IEnumerable<Rental>> IRentalService.GetAllAsync()
        {
            return await _rentalRepository.GetAllAsync();
        }

        Rental IRentalService.GetById(object id)
        {
            return _rentalRepository.GetById(id);
        }

        async Task<Rental> IRentalService.GetByIdAsync(object id)
        {
            return await _rentalRepository.GetByIdAsync(id);
        }
    }
}