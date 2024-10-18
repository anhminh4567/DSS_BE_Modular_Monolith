using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModelCategories.Queries.GetAll
{
    public record GetAllCategoryQuery() : IRequest<List<JewelryModelCategory>>;
    internal class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, List<JewelryModelCategory>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        public GetAllCategoryQueryHandler(
            IJewelryModelCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<JewelryModelCategory>> Handle(GetAllCategoryQuery request, CancellationToken token)
        {
            var query = _categoryRepository.GetQuery();
            query = _categoryRepository.QueryInclude(query, r => r.ParentCategory);
            return query.ToList();
        }
    }
}
