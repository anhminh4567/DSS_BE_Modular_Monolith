using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll
{
    public record GetAllJewelryModelQuery(int CurrentPage = 0, int PageSize = 20, string? Name = "", string? Category = null, bool? IsRhodiumFinished = null, bool? IsEngravable = null) : IRequest<PagingResponseDto<JewelryModel>>;
    internal class GetAllJewelryModelQueryHandler : IRequestHandler<GetAllJewelryModelQuery, PagingResponseDto<JewelryModel>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        public GetAllJewelryModelQueryHandler(IJewelryModelRepository jewelryModelRepository, IJewelryModelCategoryRepository categoryRepository)
        {
            _jewelryModelRepository = jewelryModelRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<PagingResponseDto<JewelryModel>> Handle(GetAllJewelryModelQuery request, CancellationToken token)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string? name, out string? categoryName, out bool? isRhodiumFinished, out bool? isEngravable);
            var query = _jewelryModelRepository.GetSellingModelQuery();
            query = _jewelryModelRepository.QueryInclude(query, p => p.SideDiamonds);
            query = _jewelryModelRepository.QueryInclude(query, p => p.MainDiamonds);
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = await _categoryRepository.ContainsName(categoryName);
                if (category == null)
                {
                    return BlankPaging();
                }
                query = _jewelryModelRepository.QueryFilter(query, p => p.CategoryId == category.Id);
            }
            if (isRhodiumFinished != null)
            {
                query = _jewelryModelRepository.QueryFilter(query, p => p.IsRhodiumFinish == isRhodiumFinished);
            }
            if (isEngravable != null)
            {
                query = _jewelryModelRepository.QueryFilter(query, p => p.IsEngravable == isEngravable);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = _jewelryModelRepository.QueryFilter(query, p => p.Name.ToUpper().Contains(name.ToUpper()));
            }
            int maxPage = (int)Math.Ceiling((decimal)query.Count() / pageSize);
            var list = query.Skip(currentPage * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<JewelryModel>(maxPage, currentPage+1, list);
        }
        private PagingResponseDto<JewelryModel> BlankPaging() => new PagingResponseDto<JewelryModel>(0, 0, []);
    }
}
