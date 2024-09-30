namespace DiamondShop.Infrastructure.Services.Payments.Zalopays.Models
{
    public class ZalopayEmbeddedData
    {
        public string[] preferred_payment_method { get; set; } =  { };
        public string redirecturl { get; set; }
        //	JSON String	{"column_name": "value"}	
        //	Thêm thông tin hiển thị ở phần Quản lý giao dịch chi tiết trên Merchant site
        public string? columninfo { get; set; } 
    }
}
