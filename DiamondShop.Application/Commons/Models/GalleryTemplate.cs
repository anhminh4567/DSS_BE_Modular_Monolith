using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Models
{
    public class GalleryTemplate
    {
        public string GalleryFolder { get; set; }
        public Media Thumbnail { get; set; }// thumnail, understandable
        public List<Media> BaseImages { get; set; } = new();// a set of images shared by many category, like different metal
                                                   // or different color but still display this same image
        public List<Media> Certificates { get; set; } = new();// certificate of the product, GIA, IGI, etc
        public Dictionary<string, List<Media>> Gallery { get; set; } = new();// categorize images by category, like metal/color/size, etc
    }

}
