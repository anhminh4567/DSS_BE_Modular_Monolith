namespace DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto
{
    public class ListBestSellingDiamondShapeDto
    {
        public string? From { get; set; }
        public string? To { get; set; }

        public int TotalInStock { get; set; }
        public int TotalActive { get; set; }
        public int TotalInactive { get; set; }
        public int TotalLocked { get; set; }
        public int TotalSold { get; set; }
        public List<DiamondBestSellingShapeDto> DiamondBestSellingShapes { get; set; } = new();
    }

}
