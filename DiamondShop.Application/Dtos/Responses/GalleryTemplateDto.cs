using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses
{
    public class GalleryTemplateDto
    {
        public string GalleryFolder { get; set; }
        public MediaDto Thumbnail { get; set; }
        public List<MediaDto> BaseImages { get; set; } // a set of images shared by many category, like different metal
                                                            // or different color but still display this same image
        public List<MediaDto> Certificates { get; set; } // certificate of the product, GIA, IGI, etc
        public Dictionary<string, MediaDto> Gallery { get; set; } // categorize images by category, like metal/color/size, etc
    }
}
