namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelValidation
    {
        public bool IsOrderValid { get; set; }
        public int[] InvalidItemIndex { get; set; } = new int[] { };
    }

}
