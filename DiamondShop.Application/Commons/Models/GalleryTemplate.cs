using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Transactions;
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
    public class JewelryModelGalleryTemplate
    {
        public string GalleryFolder { get; set; }
        public Media Thumbnail { get; set; }
        public List<Media> BaseImages { get; set; } = new(); // a set of images shared by many category, like different metal
                                                             // or different color but still display this same image
        public List<Media> BaseMetals { get; set; } = new(); // a set of images shared by many category, like different metal
                                                             // or different color but still display this same image
        public List<Media> BaseSideDiamonds { get; set; } = new(); // a set of images shared by many category, like different metal
                                                                   // or different color but still display this same image
        public List<Media> BaseMainDiamonds { get; set; } = new();// a set of images shared by many category, like different metal
                                                                  // or different color but still display this same image
        public Dictionary<string, List<Media>> Gallery { get; set; } // categorize images by category, like metal/color/size, etc
    }
    public class OrderGalleryTemplate
    {
        public string GalleryFolder { get; set; }
        public List<Media> OrderDeliveryConfirmationImages { get; set; } = new();
        public Media? OrderDeliveryConfirmationVideo { get; set; }
        //transaction id is key, list media is value
        public Dictionary<string, List<Media>> OrderTransactionImages { get; set; } = new();
        //log id is key, list media is value
        public Dictionary<string, List<Media>> OrderLogImages { get; set; } = new();
        public void AddOrderTransactionImages(string tobeComparedPath, Media images)
        {
            int lastSlashIndex = tobeComparedPath.LastIndexOf('/');
            var key = tobeComparedPath.Substring(0, lastSlashIndex);
            if (OrderTransactionImages.TryGetValue(key, out var medias) == false)
                OrderTransactionImages.Add(key, new List<Media> { images });
            else
                OrderTransactionImages[key].Add(images);
        }
        public void AddOrderLogImages(string tobeComparedPath, Media images)
        {
            int lastSlashIndex = tobeComparedPath.LastIndexOf('/');
            var key = tobeComparedPath.Substring(0, lastSlashIndex);
            if (OrderLogImages.TryGetValue(key, out var medias) == false)
                OrderLogImages.Add(key, new List<Media> { images });
            else
                OrderLogImages[key].Add(images);
        }
        public void AddConfirmOrderImages(List<Media> images)
        {
            OrderDeliveryConfirmationImages.AddRange(images);
        }
        public void AddConfirmOrderVideo(Media images)
        {
            OrderDeliveryConfirmationVideo = images;
        }
    }

}
