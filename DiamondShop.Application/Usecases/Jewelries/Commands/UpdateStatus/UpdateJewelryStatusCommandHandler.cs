using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.UpdateStatus
{
    public record UpdateJewelryStatusCommand(string JewelryId) : IRequest<Result<Jewelry>>;
    internal class UpdateJewelryStatusCommandHandler : IRequestHandler<UpdateJewelryStatusCommand, Result<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateJewelryStatusCommandHandler(IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Jewelry>> Handle(UpdateJewelryStatusCommand request, CancellationToken token)
        {
            request.Deconstruct(out string jewelryId);
            await _unitOfWork.BeginTransactionAsync(token);
            var jewelry = await _jewelryRepository.GetById(JewelryId.Parse(jewelryId));
            if (jewelry == null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            if (jewelry.Status == ProductStatus.Active)
                jewelry.SetInactive();
            else if (jewelry.Status == ProductStatus.Inactive)
                jewelry.SetSell();
            else
                return Result.Fail(JewelryErrors.InCorrectState("Đang bán hoặc Không bán"));
            await _jewelryRepository.Update(jewelry);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return jewelry; 
        }
    }
}
