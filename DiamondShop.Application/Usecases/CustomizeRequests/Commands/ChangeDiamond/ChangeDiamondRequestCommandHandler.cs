using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.ChangeDiamond
{
    public record ChangeDiamondRequestCommand(string DiamondRequestId, string DiamondId) : IRequest<Result<DiamondRequest>>;
    internal class ChangeDiamondRequestCommandHandler : IRequestHandler<ChangeDiamondRequestCommand, Result<DiamondRequest>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondRequestRepository _diamondRequestRepository;
        private readonly ICustomizeRequestService _customizeRequestService;
        private readonly IDiamondServices _diamondServices;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeDiamondRequestCommandHandler(IDiamondRepository diamondRepository, IDiamondRequestRepository diamondRequestRepository, ICustomizeRequestService customizeRequestService, IDiamondServices diamondServices, IUnitOfWork unitOfWork)
        {
            _diamondRepository = diamondRepository;
            _diamondRequestRepository = diamondRequestRepository;
            _customizeRequestService = customizeRequestService;
            _diamondServices = diamondServices;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<DiamondRequest>> Handle(ChangeDiamondRequestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
