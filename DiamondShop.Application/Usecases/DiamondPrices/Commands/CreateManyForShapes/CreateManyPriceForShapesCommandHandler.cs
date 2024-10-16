using DiamondShop.Application.Services.Data;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateManyForShapes
{
    public record PriceForShape(string shapeId, decimal price);
    public record CreateManyPriceForShapesCommand(string criteriaId, List<PriceForShape> pricesForShape)  : IRequest<Result<List<DiamondPrice>>>;
    internal class CreateManyPriceForShapesCommandHandler : IRequestHandler<CreateManyPriceForShapesCommand, Result<List<DiamondPrice>>>
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondShapeRepository _diamondShapeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateManyPriceForShapesCommandHandler(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondShapeRepository diamondShapeRepository, IUnitOfWork unitOfWork)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondShapeRepository = diamondShapeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DiamondPrice>>> Handle(CreateManyPriceForShapesCommand request, CancellationToken cancellationToken)
        {
            //check if the shapesId provided is enought for the shapes in the db
            var parsedCriteriaId = DiamondCriteriaId.Parse(request.criteriaId);
            var getCriterias = _diamondCriteriaRepository.GetById(parsedCriteriaId);
            if(getCriterias == null)
                return Result.Fail<List<DiamondPrice>>(new NotFoundError("can't found the criteria to add price for these shapes"));
            var idsShapes = request.pricesForShape.Select(s => s.shapeId);
            var getShapes = await _diamondShapeRepository.GetAll();
            var shapesSelected = getShapes.Where(s => idsShapes.Contains(s.Id.Value));
            List<DiamondPrice> tobeAdded = new();
            foreach (var price in request.pricesForShape)
            {
                var parsedShapeId = DiamondShapeId.Parse(price.shapeId);
                var selectedShape = shapesSelected.First(p => p.Id == parsedShapeId);
                var newPrice = DiamondPrice.Create(parsedShapeId, parsedCriteriaId, price.price);
                tobeAdded.Add(newPrice);
            }
            await _unitOfWork.BeginTransactionAsync();
            await _diamondPriceRepository.CreateMany(tobeAdded);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            throw new NotImplementedException();
        }
    }
}
