using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetAll
{
    public record GetAllJewelryQuery(int page = 0, int take = 20, string? ModelName = "", string? SerialCode = "", string? MetalId = "", string? SizeId = "", bool? HasSideDiamond = null, ProductStatus? Status = null) : IRequest<PagingResponseDto<Jewelry>>;
    internal class GetAllJewelryQueryHandler : IRequestHandler<GetAllJewelryQuery, PagingResponseDto<Jewelry>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        public GetAllJewelryQueryHandler(
            IJewelryRepository jewelryRepository
            )
        {
            _jewelryRepository = jewelryRepository;
        }

        public async Task<PagingResponseDto<Jewelry>> Handle(GetAllJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int page, out int take, out string? modelName, out string? serialCode, out string? metalId, out string? sizeId, out bool? hasSideDiamond, out ProductStatus? status);
            var query = _jewelryRepository.GetQuery();
            query = _jewelryRepository.QueryInclude(query, p => p.Model);
            query = _jewelryRepository.QueryInclude(query, p => p.Metal);
            query = _jewelryRepository.QueryInclude(query, p => p.Size);
            query = _jewelryRepository.QueryInclude(query, p => p.Diamonds);
            if (!string.IsNullOrEmpty(modelName))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.Model.Name.ToUpper().Contains(modelName.ToUpper()));
            }
            if (!string.IsNullOrEmpty(serialCode))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.SerialCode.Contains(serialCode));
            }
            if (!string.IsNullOrEmpty(metalId))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.MetalId == MetalId.Parse(metalId));
            }
            if (!string.IsNullOrEmpty(sizeId))
            {
                query = _jewelryRepository.QueryFilter(query, p => p.SizeId == SizeId.Parse(sizeId));
            }
            if(hasSideDiamond != null)
            {
                query = _jewelryRepository.QueryFilter(query, p => p.SideDiamond != null == hasSideDiamond);
            }
            if (status != null)
            {
                query = _jewelryRepository.QueryFilter(query, p => p.Status == status);
            }
            else
                query = _jewelryRepository.QueryOrderBy(query, p => p.OrderBy(p => p.Status));
            query = _jewelryRepository.QuerySplit(query);
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / take);
            var list = query.Skip(page * take).Take(take).ToList();
            return new PagingResponseDto<Jewelry>(maxPage, page+1, list);
        }
    }
}
