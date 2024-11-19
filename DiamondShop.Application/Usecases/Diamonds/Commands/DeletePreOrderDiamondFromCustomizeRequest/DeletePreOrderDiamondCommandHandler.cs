using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.DeletePreOrderDiamondFromCustomizeRequest
{
    public record DeletePreOrderDiamondCommand (string diamondId, string customizeRequestId, string diamondRequestId) :  IRequest<Result>;
    internal class DeletePreOrderDiamondCommandHandler : IRequestHandler<DeletePreOrderDiamondCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DeletePreOrderDiamondCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, ICustomizeRequestRepository customizeRequestRepository, ICustomizeRequestService customizeRequestService, ISender sender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _customizeRequestRepository = customizeRequestRepository;
            _customizeRequestService = customizeRequestService;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task<Result> Handle(DeletePreOrderDiamondCommand request, CancellationToken cancellationToken)
        {
            var diamondId = DiamondId.Parse(request.diamondId);
            var customizeRequestId = CustomizeRequestId.Parse(request.customizeRequestId);
            var diamondRequestId = DiamondRequestId.Parse(request.diamondRequestId);
            var diamond = await _diamondRepository.GetById(diamondId);
            if (diamond is null)
                return Result.Fail(new NotFoundError("Không tìm thấy viên kim cương này"));
            
            var customizeRequest = await _customizeRequestRepository.GetById(customizeRequestId);
            if(customizeRequest is null)
                return Result.Fail(new NotFoundError("Không tìm thấy yêu cầu này"));
            
            var diamondRequest = customizeRequest.DiamondRequests.FirstOrDefault(x => x.DiamondRequestId == diamondRequestId);
            if (diamondRequest is null)
                return Result.Fail(new NotFoundError("Không tìm thấy yêu cầu diamond này"));

            await _unitOfWork.BeginTransactionAsync();
            _diamondRepository.Delete(diamond).Wait();
            diamondRequest.DiamondId = null;
            _customizeRequestRepository.Update(customizeRequest).Wait();
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return Result.Ok();
            throw new NotImplementedException();
        }
    }
}
