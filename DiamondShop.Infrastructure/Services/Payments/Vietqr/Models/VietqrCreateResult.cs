namespace DiamondShop.Infrastructure.Services.Payments.Vietqr.Models
{
    public class VietqrCreateResult
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string bankAccount { get; set; }
        public string userBankName { get; set; }
        public string mmount { get; set; }
        public string content { get; set; }
        public string qrCode { get; set; }
        public string imgId { get; set; }
        public int Existing { get; set; }
        public string transactionId { get; set; }
        public string transactionRefId { get; set; }
        public string qrLink { get; set; }
        public string terminalCode { get; set; }
        public string subTerminalCode { get; set; }
        public string serviceCode { get; set; }
        public string orderId { get; set; }
        public object additionalData { get; set; }
    }
    public class VietqrCreateResponseError
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
