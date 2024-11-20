using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry
{
    public record AttachDiamondCommand(JewelryId JewelryId, List<Diamond> DiamondIds) : IRequest<Result>;
    internal class AttachDiamondCommandHandler : IRequestHandler<AttachDiamondCommand, Result>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryRepository _jewelryRepository;

        public AttachDiamondCommandHandler(IDiamondRepository diamondRepository, IUnitOfWork unitOfWork, IJewelryRepository jewelryRepository)
        {
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
            _jewelryRepository = jewelryRepository;
        }

        public async Task<Result> Handle(AttachDiamondCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryId jewelryId, out List<Diamond> diamonds);
            var jewelry = await _jewelryRepository.GetById(jewelryId);
            if(jewelry.Id == null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            foreach (var diamond in diamonds)
            {
                if (diamond.JewelryId != null)
                    return Result.Fail(DiamondErrors.DiamondAssignedToJewelryAlready(jewelry.SerialCode));
                diamond.SetForJewelry(jewelry);
            }

            _diamondRepository.UpdateRange(diamonds);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();            
        }


    }
}
