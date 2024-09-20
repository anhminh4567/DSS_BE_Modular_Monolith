using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.News.ValueObjects
{
    public record NewsId(string Value)
    {
        public static NewsId Parse(string id)
        {
            return new NewsId(id) { Value = id };
        }
        public static NewsId Create()
        {
            return new NewsId(Guid.NewGuid().ToString());
        }
    }
}
