using BeatvisionRemake.Domain.Common;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds
{
    public class Diamond : Entity<DiamondId> , IAggregateRoot
    {  
        public JewelryId JewelryId { get;  set;}
        public DiamondShapeId DiamondShapeId { get; set;}
        public string Title { get; set;}
        public string Description { get; set;}  
    }
}
