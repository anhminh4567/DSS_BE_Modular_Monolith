using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.Delete
{
    public record DeleteDiamondCommand(string diamondId) : IRequest<Result>;
    internal class DeleteDiamondCommandHandler : IRequestHandler<DeleteDiamondCommand,Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;

        public DeleteDiamondCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
        }

        public async Task<Result> Handle(DeleteDiamondCommand request, CancellationToken cancellationToken)
        {
            var diamondId = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(diamondId);
            if (getDiamond == null)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            if(getDiamond.Status == ProductStatus.Sold)
                return Result.Fail(DiamondErrors.SoldError());
            if (getDiamond.JewelryId is not null)
                return Result.Fail(DiamondErrors.DiamondAssignedToJewelryAlready(detail: "Không thể xóa"));
            if (getDiamond.Status != ProductStatus.Inactive)
                return Result.Fail(DiamondErrors.DeleteUnallowed());
            await _diamondRepository.Delete(getDiamond);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
