using FluentResults;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelValidation
    {
        public bool IsOrderValid { get => MainErrorMessage.Length == 0; }
        public bool IsShippingValid { get; set; }
        public bool IsPaymentMethodValid { get; set; } = true;
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
            if(shippingPrice.IsLocationActive == false)
            {
                MainErrorMessage = MainErrorMessage.Append("the location is not supported for delivery").ToArray();
                IsShippingValid = false;
            }
        }
        public void SetErrorMessage(string message)
        {
            MainErrorMessage = MainErrorMessage.Append(message).ToArray();
        }
        public void SetErrorMessage(IError predefinedError)
        {
            SetErrorMessage(predefinedError.Message);
        }

    }

}
