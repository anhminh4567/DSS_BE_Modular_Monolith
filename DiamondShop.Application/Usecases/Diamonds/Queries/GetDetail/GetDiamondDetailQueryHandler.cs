using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail
{
    public record GetDiamondDetail(string diamondId ) : IRequest<Result<Diamond>>;
    internal class GetDiamondDetailQueryHandler : IRequestHandler<GetDiamondDetail, Result<Diamond>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;

        public GetDiamondDetailQueryHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
        }

        public Task<Result<Diamond>> Handle(GetDiamondDetail request, CancellationToken cancellationToken)
        {
            return 
        }
    }
}
