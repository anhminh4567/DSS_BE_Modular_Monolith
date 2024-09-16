using BeatvisionRemake.Domain.Common;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondShapes
{
    public class DiamondShape : Entity<DiamondId> , IAggregateRoot
    {
        public string ShapeName { get; private set; }
        public string? IconPath { get; private set; }
        public static DiamondShape Create(string shapeName, string? relativeIconPath)
        {
            return new DiamondShape() 
            {
                Id = DiamondId.Create(),
                ShapeName = shapeName,
                IconPath = relativeIconPath
            };
        }
        public void Update(string? shapeName, string? relativeIconPath)
        {
            ShapeName = shapeName == null ? ShapeName : shapeName;
            IconPath = relativeIconPath == null ? IconPath : relativeIconPath;
        }
    }
}
