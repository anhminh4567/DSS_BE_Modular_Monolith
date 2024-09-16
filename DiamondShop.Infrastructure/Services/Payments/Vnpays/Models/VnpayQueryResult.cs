namespace DiamondShop.Infrastructure.Services.Payments.Vnpays.Models
{
    public class VnpayQueryResult
    {
        public string? vnp_ResponseId { get; set; }
        public string? vnp_Command { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_Message { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_Amount { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_PayDate { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? vnp_TransactionType { get; set; }
        public string? vnp_TransactionStatus { get; set; }
        public string? vnp_CardNumber { get; set; }
        public string? vnp_Trace { get; set; }
        public string? vnp_CardHolder { get; set; }
        public string? vnp_Issuer { get; set; }
        public string? vnp_FeeAmount { get; set; }
    }
}
