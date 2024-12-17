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
            if (shippingPrice.IsLocationActive == false)
            {
                MainErrorMessage = MainErrorMessage.Append("địa chỉ ship không hỗ trợ, xin lỗi vì sự bất tiện").ToArray();
                IsShippingValid = false;
            }
            if (shippingPrice.IsValid == false)
            {
                MainErrorMessage = MainErrorMessage.Append("địa chỉ ship không hợp lệ").ToArray();
                IsShippingValid = false;
            }

            if (InvalidItemIndex.Length > 0 || UnavailableItemIndex.Length > 0)
                MainErrorMessage = MainErrorMessage.Append("Some items are invalid").ToArray();
            
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
