using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Blogs.ValueObjects
{
    public record BlogId(string Value)
    {
        public static BlogId Parse(string id)
        {
            return new BlogId(id) { Value = id };
        }
        public static BlogId Create()
        {
            return new BlogId(Guid.NewGuid().ToString());
        }
    }
}
