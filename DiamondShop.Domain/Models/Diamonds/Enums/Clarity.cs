namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Clarity
    {
        S12 = 1, 
        S11 = 2,
        VS2 = 3,
        VS1 = 4 ,
        VVS2 = 5,
        VVS1 = 6 ,
        IF = 7,
        FL = 8
    }
    public static class ClarityHelper
    {
        public static string GetClarityName(Clarity clarity)
        {
            return clarity switch
            {
                Clarity.S12 => "S12",
                Clarity.S11 => "S11",
                Clarity.VS2 => "VS2",
                Clarity.VS1 => "VS1",
                Clarity.VVS2 => "VVS2",
                Clarity.VVS1 => "VVS1",
                Clarity.IF => "IF",
                Clarity.FL => "FL",
                _ => throw new ArgumentOutOfRangeException(nameof(clarity), clarity, null)
            };
        }
        public static List<Clarity> GetClarityList()
        {
            return Enum.GetValues(typeof(Clarity)).Cast<Clarity>().ToList();
        }
    }
}
