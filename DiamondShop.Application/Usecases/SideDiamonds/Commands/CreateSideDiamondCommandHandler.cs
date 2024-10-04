using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands
{
    public record CreateSideDiamondCommand(JewelryModelId ModelId, SideDiamondRequestDto SideDiamondSpecs) : IRequest<Result>;
    internal class CreateSideDiamondCommandHandler : IRequestHandler<CreateSideDiamondCommand, Result>
    {
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateSideDiamondCommandHandler(
            ISideDiamondRepository sideDiamondRepository,
            IUnitOfWork unitOfWork
            ) 
        { 
            _sideDiamondRepository = sideDiamondRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateSideDiamondCommand request, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync(token);
            request.Deconstruct(out JewelryModelId modelId, out SideDiamondRequestDto sideDiamondSpec);
            var sideDiamond = SideDiamondReq.Create(modelId, DiamondShapeId.Parse(sideDiamondSpec.ShapeId), sideDiamondSpec.ColorMin, sideDiamondSpec.ColorMax, sideDiamondSpec.ClarityMin, sideDiamondSpec.ClarityMax, sideDiamondSpec.SettingType);
            await _sideDiamondRepository.Create(sideDiamond, token);

            List<SideDiamondOpt> sideDiamondOpts = sideDiamondSpec.OptSpecs.Select(p => SideDiamondOpt.Create(sideDiamond.Id, p.CaratWeight, p.Quantity)).ToList();
            await _sideDiamondRepository.CreateOpts(sideDiamondOpts, token);
            await _unitOfWork.SaveChangesAsync(token);
            return Result.Ok();
        }
    }
}
