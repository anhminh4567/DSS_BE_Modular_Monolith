namespace DiamondShop.Infrastructure.Services.Locations.OApi.Models
{
    public class OApiDistrictResponses
    {
        public int Total { get; set; }
        public List<OApiDistrict> Data { get; set; }
        public string Code { get; set; }
    }
    public class OApiDistrict
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProvinceId { get; set; }
        public int Type { get; set; }
        public string TypeText { get; set; }
    }
}
