using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ErrorMessages;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.Create
{
    public record CreateDiamondRequestDto(Diamond_4C diamond4c, Diamond_Details details, Diamond_Measurement measurement, string shapeId, string? sku, Certificate? Certificate = Certificate.GIA, decimal priceOffset = 1);
    public record CreateDiamondCommand(Diamond_4C diamond4c, Diamond_Details details, Diamond_Measurement measurement, string shapeId, string? sku,Certificate? Certificate = Certificate.GIA, decimal priceOffset = 1) :IRequest<Result<Diamond>>;
    internal class CreateDiamondCommandHandler : IRequestHandler<CreateDiamondCommand, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondServices _diamondServices;

        public CreateDiamondCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondShapeRepository diamondShapeRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondServices diamondServices)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _optionsMonitor = optionsMonitor;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondServices = diamondServices;
        }

        public async Task<Result<Diamond>> Handle(CreateDiamondCommand request, CancellationToken cancellationToken)
        {
            DiamondRule diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            request.Deconstruct(out var diamond4c, out var details, out var measurement, out string shapeGivenId, out string? sku, out var certificate, out var priceOffset);
            DiamondShapeId shapeId = DiamondShapeId.Parse(shapeGivenId);
            var getShapes = await _diamondShapeRepository.GetAll();
            DiamondShape? getShape = getShapes.FirstOrDefault(x => x.Id == shapeId);
            if (getShape is null)
                return Result.Fail(DiamondShapeErrors.NotFoundError);
            
            List<Diamond> diamondFromSku = await _diamondRepository.GetBySkus(new string[] { sku });
            if (diamondFromSku.Count > 0)
                return Result.Fail(DiamondErrors.DiamondExistError($"mã code của KC là #{diamondFromSku.First().SerialCode}, mã id là - {diamondFromSku.First().Id.Value}"));

            Diamond newDiamond = Diamond.Create(getShape, diamond4c, details, measurement, priceOffset,sku, certificate.Value);
            var isDiamondBelongToACriteraGroup = await _diamondServices.IsMainDiamondFoundInCriteria(newDiamond);
            if (isDiamondBelongToACriteraGroup == false )
                return Result.Fail(DiamondErrors.DiamondNotExistInAnyCriteria);

            await _diamondRepository.Create(newDiamond);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok(newDiamond);
        }
    }
}
