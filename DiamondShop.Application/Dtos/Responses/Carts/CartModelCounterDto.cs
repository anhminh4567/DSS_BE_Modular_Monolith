namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelCounterDto
    {
        public int TotalProduct { get; set; } = 0;
        public int TotalInvalidProduct { get; set; } = 0;
        public int TotalItem { get; set; } = 0;
        public int TotalInvalidItem { get; set; } = 0;
    }
}
