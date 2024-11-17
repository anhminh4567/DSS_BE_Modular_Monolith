namespace DiamondShop.Application.Dtos.Responses.Diamonds.DashboardDto
{
    public class ListBestSellingDiamondShapeDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TotalInStock { get; set; }
        public int TotalActive { get; set; }
        public int TotalInactive { get; set; }
        public List<DiamondBestSellingShapeDto> DiamondBestSellingShapes { get; set; } = new();
    }

}
