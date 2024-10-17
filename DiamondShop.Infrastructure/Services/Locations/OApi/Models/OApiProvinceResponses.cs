namespace DiamondShop.Infrastructure.Services.Locations.OApi.Models
{
    public class OApiProvinceResponses
    {
        public int Total { get; set; }
        public List<OApiProvince> Data { get; set; }
        public string Code { get; set; }
    }
    public class OApiProvince
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string TypeText { get; set; }
        public string Slug { get; set; }
    }
}
