namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelValidationDto
    {
        public bool IsOrderValid { get; set; }
        public bool IsShippingValid { get; set; }
        public int[] InvalidItemIndex { get; set; }
        public int[] UnavailableItemIndex { get; set; } 
        public string[] MainErrorMessage { get; set; }
    }
}
