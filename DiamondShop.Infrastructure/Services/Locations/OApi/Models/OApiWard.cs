namespace DiamondShop.Infrastructure.Services.Locations.OApi.Models
{
    public class OApiWardResponse
    {
        public int Total { get; set; }
        public List<OApiWard> Data { get; set; }
        public string Code { get; set; }
    }
    public class OApiWard
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DistrictId { get; set; }
        public int Type { get; set; }
        public string TypeText { get; set; }
    }
}
