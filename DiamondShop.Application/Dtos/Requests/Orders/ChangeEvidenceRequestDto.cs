using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record ChangeEvidenceRequestDto(string TransactionId, IFormFile Evidence);

}
