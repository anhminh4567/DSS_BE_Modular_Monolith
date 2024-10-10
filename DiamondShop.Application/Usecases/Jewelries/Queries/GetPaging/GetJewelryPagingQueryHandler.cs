using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.Jewelries;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetPaging
{
    public record GetJewelryPagingQuery(int pageSize = 20, int start = 0) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetJewelryPagingQueryHandler
    {
    }
}
