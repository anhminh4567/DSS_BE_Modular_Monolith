namespace DiamondShop.Infrastructure.Services.Payments.Vietqr.Models
{

    public class VietqrTransactionCallback
    {
        public string transactionid { get; set; }
        public long transactiontime { get; set; }
        public string referencenumber { get; set; }
        public decimal amount { get; set; }
        public string content { get; set; }
        public string bankaccount { get; set; }
        public string orderId { get; set; }
        public string sign { get; set; }
        public string terminalCode { get; set; }
        public string urlLink { get; set; }
        public string serviceCode { get; set; }
        public string subTerminalCode { get; set; }
    }
    // Lớp model cho success response
    public class VietqrSuccessResponse
    {
        public bool Error { get; set; }
        public string ErrorReason { get; set; }
        public string ToastMessage { get; set; }
        public VietqrTransactionResponseObject Object { get; set; }
    }

    // Lớp model cho lỗi response
    public class VietqrErrorResponse
    {
        public bool Error { get; set; }
        public string ErrorReason { get; set; }
        public string ToastMessage { get; set; }
        public object Object { get; set; }
    }

    // Lớp model cho object trả về trong success response
    public class VietqrTransactionResponseObject
    {
        public string reftransactionid { get; set; }
    }
}
