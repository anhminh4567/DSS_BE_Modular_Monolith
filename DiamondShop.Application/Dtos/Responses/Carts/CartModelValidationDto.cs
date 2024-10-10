namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelValidationDto
    {
        public bool IsOrderValid { get; set; }
        public int[] InvalidItemIndex { get; set; }
    }
}
