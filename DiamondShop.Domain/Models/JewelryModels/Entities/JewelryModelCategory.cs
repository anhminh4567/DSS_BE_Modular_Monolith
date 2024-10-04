using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{

    public class JewelryModelCategory : Entity<JewelryModelCategoryId>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public bool IsGeneral { get; set; }
        public JewelryModelCategoryId? ParentCategoryId { get; set; }
        public JewelryModelCategory? ParentCategory { get; set; }
        private JewelryModelCategory() { }
        public static JewelryModelCategory Create(string name, string description, string thumbnailPath, bool isGeneral, JewelryModelCategoryId? parentCategoryId, JewelryModelCategoryId givenId = null)
        {
            return new JewelryModelCategory()
            {
                Id = givenId is null ? JewelryModelCategoryId.Create() : givenId,
                Name = name,
                Description = description,
                ThumbnailPath = thumbnailPath,
                IsGeneral = isGeneral,
                ParentCategoryId = parentCategoryId,
            };
        }
    }
}
