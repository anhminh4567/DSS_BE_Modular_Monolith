using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetDetail
{
    public record GetJewelryModelDetailQuery(string ModelId) : IRequest<Result<JewelryModel>>;
    internal class GetJewelryModelDetailQueryHandler : IRequestHandler<GetJewelryModelDetailQuery, Result<JewelryModel>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IJewelryModelService _jewelryModelService;

        public GetJewelryModelDetailQueryHandler(IJewelryModelRepository jewelryModelRepository, IJewelryModelService jewelryModelService)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _jewelryModelService = jewelryModelService;
        }

        public async Task<Result<JewelryModel>> Handle(GetJewelryModelDetailQuery request, CancellationToken token)
        {
            request.Deconstruct(out string modelId);
            var jewelryModel = await _jewelryModelRepository.GetById(JewelryModelId.Parse(modelId));
            if (jewelryModel == null)
                return Result.Fail("This jewelry model doesn't exist");
            return jewelryModel;
        }
    }
}
