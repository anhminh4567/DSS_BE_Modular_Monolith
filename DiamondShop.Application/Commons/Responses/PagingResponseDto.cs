using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Responses
{
    public record PagingResponseDto<T> (int totalPage, int currentPage, List<T> Values);

}
