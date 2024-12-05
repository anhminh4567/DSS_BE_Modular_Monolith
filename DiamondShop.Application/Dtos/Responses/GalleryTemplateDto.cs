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
        public Dictionary<string, List<MediaDto>> Gallery { get; set; } // categorize images by category, like metal/color/size, etc
    }
    public class JewelryModelGalleryTemplateDto
    {
        public string GalleryFolder { get; set; }
        public MediaDto Thumbnail { get; set; }
        public List<MediaDto> BaseImages { get; set; } = new(); // a set of images shared by many category, like different metal
                                                       // or different color but still display this same image
        public List<MediaDto> BaseMetals { get; set; } = new(); // a set of images shared by many category, like different metal
                                                                // or different color but still display this same image
        public List<MediaDto> BaseSideDiamonds { get; set; } = new(); // a set of images shared by many category, like different metal
                                                                      // or different color but still display this same image
        public List<MediaDto> BaseMainDiamonds { get; set; } = new();// a set of images shared by many category, like different metal
                                                                     // or different color but still display this same image
        public Dictionary<string, List<MediaDto>> Gallery { get; set; } // categorize images by category, like metal/color/size, etc
    }
    public class OrderGalleryTemplateDto
    {
        public string GalleryFolder { get; set; }
        public List<MediaDto> OrderDeliveryConfirmationImages { get; set; } = new();
        public MediaDto? OrderDeliveryConfirmationVideo { get; set; }
        //transaction id is key, list media is value
        public Dictionary<string, List<MediaDto>> OrderTransactionImages { get; set; } = new();
        //log id is key, list media is value
        public Dictionary<string, List<MediaDto>> OrderLogImages { get; set; } = new();
    }
}
