using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses
{
    public class MediaDto
    {
        //public string? MediaName { get; set; }
        public string MediaPath { get; set; }
        public string ContentType { get; set; }
    }
}
