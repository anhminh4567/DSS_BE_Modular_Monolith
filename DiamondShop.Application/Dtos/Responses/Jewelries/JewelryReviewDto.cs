using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Application.Dtos.Responses.Accounts;

namespace DiamondShop.Application.Dtos.Responses.Jewelries
{
    public class JewelryReviewDto
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public AccountDto Account { get; set; }
        public string Content { get; set; }
        public int StarRating { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsHidden { get; set; }
        public List<Media> Medias { get; set; } = new();
    }
}
