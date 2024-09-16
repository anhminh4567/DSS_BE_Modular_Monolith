namespace DiamondShop.Infrastructure.Services.Payments.Vnpays.Models
{
    public class VnpayReturnResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public long Vnp_TxnRef { get; set; }
        public long TransactionNo { get; set; }
        public string? TransactionStatus { get; set; }
        public string? StatusCode { get; set; }
        public long Amount { get; set; }
        public string? BankCode { get; set; }
    }
}
