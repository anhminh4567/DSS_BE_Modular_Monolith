using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Blogs.Entities
{
    public class BlogTag
    {
        public string Value { get; set; }
        public BlogTag(string value)
        {
            Value = value;
        }  
    }
}
