namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelValidation
    {
        public bool IsOrderValid { get => MainErrorMessage.Length == 0; }
        public bool IsShippingValid { get; set; }
        public int[] InvalidItemIndex { get; set; } = new int[] { };
        public int[] UnavailableItemIndex { get; set; } = new int[] { };
        public string[] MainErrorMessage { get; set; } = new string[] { };
        public void SetErrorMessageInTheEnd(ShippingPrice shippingPrice)
        {
            if (InvalidItemIndex.Length > 0 || UnavailableItemIndex.Length > 0)
                MainErrorMessage = MainErrorMessage.Append("Some items are invalid").ToArray();
            if(shippingPrice.IsValid == false)
            {
                MainErrorMessage = MainErrorMessage.Append("Shipping address is invalid").ToArray();
                IsShippingValid = false;
            }
        }
    }

}
