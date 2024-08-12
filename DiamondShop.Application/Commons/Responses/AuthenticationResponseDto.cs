using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Responses
{
    public record AuthenticationResultDto(string accessToken, DateTime expiredAccess, string refreshToken, DateTime expiredRefresh);

}
