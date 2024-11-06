namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Color
    {
        K = 1, J = 2 , I = 3 , H = 4 , G = 5 , F = 6 , E = 7 , D = 8
    }
    public static class ColorHelper
    {
        public static string GetColorName(Color color)
        {
            return color switch
            {
                Color.K => "K",
                Color.J => "J",
                Color.I => "I",
                Color.H => "H",
                Color.G => "G",
                Color.F => "F",
                Color.E => "E",
                Color.D => "D",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
        public static List<Color> GetColorList() 
        {
            return Enum.GetValues(typeof(Color)).Cast<Color>().ToList();
        }
    }
}
