using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.MainDiamonds.Commands.Create
{
    public record CreateMainDiamondCommand(JewelryModelId ModelId, MainDiamondSpec MainDiamondSpec) : IRequest<Result>;
    internal class CreateMainDiamondCommandHandler : IRequestHandler<CreateMainDiamondCommand, Result>
    {
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateMainDiamondCommandHandler(
            IMainDiamondRepository mainDiamondRepository,
            IUnitOfWork unitOfWork
            )
        {
            _mainDiamondRepository = mainDiamondRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateMainDiamondCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelId modelId, out MainDiamondSpec mainDiamondSpec);
            var mainDiamond = MainDiamondReq.Create(modelId, mainDiamondSpec.SettingType, mainDiamondSpec.Quantity);
            await _mainDiamondRepository.Create(mainDiamond, token);
            List<MainDiamondShape> mainDiamondShapes = mainDiamond.Shapes.Select(p => MainDiamondShape.Create(mainDiamond.Id, p.ShapeId, p.CaratFrom, p.CaratTo)).ToList();
            await _mainDiamondRepository.CreateShapes(mainDiamondShapes, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();
        }
    }
}
