using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
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

        public AttachDiamondCommandHandler(IDiamondRepository diamondRepository, IUnitOfWork unitOfWork)
        {
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AttachDiamondCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryId jewelryId, out List<Diamond> diamonds);
            diamonds.ForEach(d => d.JewelryId = jewelryId);
            _diamondRepository.UpdateRange(diamonds);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();            
        }


    }
}
