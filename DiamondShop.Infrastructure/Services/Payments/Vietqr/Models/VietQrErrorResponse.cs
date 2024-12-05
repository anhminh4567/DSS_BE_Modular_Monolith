namespace DiamondShop.Infrastructure.Services.Payments.Vietqr.Models
{
    public record VietQrErrorResponse
    {
        public string Status { get; init; }
        public string Message { get; init; }
    }
}
