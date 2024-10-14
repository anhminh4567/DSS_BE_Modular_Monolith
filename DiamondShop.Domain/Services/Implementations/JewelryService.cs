using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class JewelryService : IJewelryService
    {
        public JewelryService() { }
        public void AddPrice(List<Jewelry> jewelries, List<SizeMetal> sizeMetals)
        {
            jewelries.ForEach(p =>
            {
                var entry = sizeMetals.FirstOrDefault(k => k.MetalId == p.MetalId && k.SizeId == p.SizeId && k.ModelId == p.ModelId);
                p.Price = (decimal)entry.Weight * entry.Metal.Price;
            });
        }
        public Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository)
        {
            var query = sizeMetalRepository.GetQuery();
            query = sizeMetalRepository.QueryInclude(query, p => p.Metal);
            query = sizeMetalRepository.QueryFilter(query, p => p.ModelId == jewelry.ModelId && p.SizeId == jewelry.SizeId && p.MetalId == jewelry.MetalId);
            var sizeMetal = query.FirstOrDefault();
            if (sizeMetal != null)
            {
                jewelry.Price = (decimal)sizeMetal.Weight * sizeMetal.Metal.Price;
            }
            return jewelry;
        }

        public Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal)
        {
            jewelry.Price = (decimal)sizeMetal.Weight * sizeMetal.Metal.Price;
            return jewelry;
        }
    }
}
