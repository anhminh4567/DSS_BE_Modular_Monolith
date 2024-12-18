using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Delete
{
    public record DeleteJewelryModelCommand(string? ModelId) : IRequest<Result>;
    internal class DeleteJewelryModelCommandHandler : IRequestHandler<DeleteJewelryModelCommand, Result>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryModelFileService _jewelryModelFileService;

        public DeleteJewelryModelCommandHandler(IJewelryModelRepository jewelryModelRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, IJewelryModelFileService jewelryModelFileService)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
            _jewelryModelFileService = jewelryModelFileService;
        }

        public async Task<Result> Handle(DeleteJewelryModelCommand request, CancellationToken token)
        {
            request.Deconstruct(out string modelId);
            var model = await _jewelryModelRepository.GetById(JewelryModelId.Parse(modelId));
            if (model == null)
                return Result.Fail(JewelryModelErrors.JewelryModelNotFoundError);
            var isExistingFlag = await _jewelryRepository.Existing(model.Id);
            if (isExistingFlag)
                return Result.Fail(JewelryModelErrors.JewelryModelInUseError);
            //Delete gallery first
            await _jewelryModelFileService.DeleteAllModelFiles(model);
            await _unitOfWork.BeginTransactionAsync(token);
            model.Length = 100;
            model.ModelCode = $"DELETED_{DateTime.UtcNow.Ticks}";
            await _jewelryModelRepository.Update(model);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
