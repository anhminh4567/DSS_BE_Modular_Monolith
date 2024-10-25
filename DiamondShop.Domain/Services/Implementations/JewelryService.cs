using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
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
        public bool SetupUnmapped(List<Jewelry> jewelries, List<SizeMetal> sizeMetals)
        {
            foreach (var jewelry in jewelries)
            {
                if (jewelry.Metal == null) return false;
                StringBuilder sb = new StringBuilder();
                sb.Append($"{jewelry.Metal.Name[0]}-{jewelry.Model.Name}");
                if (jewelry.SideDiamonds != null)
                {
                    string[] arr = new string[jewelry.SideDiamonds.Count];
                    jewelry.SideDiamonds.ForEach(d => arr.Append($"Avg.{d.Carat}"));
                    sb.Append(String.Join("|", arr));
                }
                jewelry.Name = sb.ToString();
                var sizeMetal = sizeMetals.FirstOrDefault(k => jewelry.ModelId == k.ModelId && jewelry.SizeId == k.SizeId && jewelry.MetalId == k.MetalId);
                if (sizeMetal?.Metal == null)
                    return false;
                jewelry.Price = sizeMetal.Price != null ? sizeMetal.Price : GetPrice(sizeMetal.Weight, jewelry.Metal.Price);
            }
            return true;
        }
        public Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository)
        {
            var query = sizeMetalRepository.GetQuery();
            query = sizeMetalRepository.QueryInclude(query, p => p.Metal);
            query = sizeMetalRepository.QueryFilter(query, p => p.ModelId == jewelry.ModelId && p.SizeId == jewelry.SizeId && p.MetalId == jewelry.MetalId);
            var sizeMetal = query.FirstOrDefault();
            if (sizeMetal != null)
            {
                jewelry.Price = GetPrice(sizeMetal.Weight, sizeMetal.Metal.Price);
            }
            return jewelry;
        }

        public Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal)
        {
            if (sizeMetal.Metal == null) return null;
            jewelry.Price = GetPrice(sizeMetal.Weight, sizeMetal.Metal.Price);
            return jewelry;
        }
        private decimal GetPrice(float Weight, decimal Price) => (decimal)Weight * Price;
    }
}
