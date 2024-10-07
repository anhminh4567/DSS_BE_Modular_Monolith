using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests
{
    public class CustomizeRequest : Entity<CustomizeRequestId>
    {
        public AccountId AccountId { get; set; }
        public JewelryModelId JewelryModelId { get; set; }
        public SizeId SizeId { get; set; }
        public MetalId MetalId { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public string? Note { get; set; }   
        public CustomizeDiamondRequestStatus Status { get; set; } 
        public List<DiamondRequest> DiamondRequests { get; set; } = new();
        public List<SideDiamondRequest> SideDiamondRequests { get; set; } = new();
    }
}
