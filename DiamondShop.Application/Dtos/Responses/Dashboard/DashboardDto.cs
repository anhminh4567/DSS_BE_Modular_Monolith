using DiamondShop.Application.Dtos.Responses.CustomizeRequests;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Transactions.Enum;

namespace DiamondShop.Application.Dtos.Responses.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalRevenue { get => TotalNormalOrderRevenue + TotalCustomizeOrderRevenue; }
        //Revenue
        public decimal TotalNormalOrderRevenue { get => NormalOrders.Sum(p => p.Transactions.Sum(k => k.TransactionType == TransactionType.Pay ? k.TransactionAmount : -1 * k.TransactionAmount)); }
        public decimal TotalCustomizeOrderRevenue { get => CustomizeOrders.Sum(p => p.Transactions.Sum(k => k.TransactionType == TransactionType.Pay ? k.TransactionAmount : -1 * k.TransactionAmount)); }
        //Order
        public int TotalNormalOrderCount { get => NormalOrders.Count(); }
        public int TotalCustomizeOrderCount { get => CustomizeOrders.Count(); }
        public Dictionary<string, int> NormalOrderGroupByStatus
        {
            get
            {
                return NormalOrders.GroupBy(p => p.Status).ToDictionary(p => p.Key.ToString(), p => p.Count());
            }
        }
        public Dictionary<string, int> CustomizeOrderGroupByStatus
        {
            get
            {
                return CustomizeOrders.GroupBy(p => p.Status).ToDictionary(p => p.Key.ToString(), p => p.Count());
            }
        }

        public List<OrderDashboardDto> NormalOrders { get => CombinedOrders.Where(p => p.CustomizeRequestId == null).ToList(); }
        public List<OrderDashboardDto> CustomizeOrders { get => CombinedOrders.Where(p => p.CustomizeRequestId != null).ToList(); }
        public List<OrderDashboardDto> CombinedOrders { get; set; }
        //Other
        public List<JewelryDto> TopSellingJewelry { get; set; }
        public List<DiamondDto> TopSellingDiamond { get; set; }
        public DashboardDto(List<OrderDashboardDto> orders)
        {
            CombinedOrders = orders;
        }
    }
}
