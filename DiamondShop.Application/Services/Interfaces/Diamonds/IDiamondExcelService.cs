using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Diamonds
{
    public interface IDiamondExcelService : IExcelService
    {
        //Task<List<DiamondCriteria>> ReadCriteriaFromExcel(IFormFile excelFile, CancellationToken cancellationToken = default);
        Task<List<MainPriceFromExcelResponse>> ReadPrices(string sheetName,CancellationToken cancellationToken = default);
    }
    public class MainPriceFromExcelResponse
    {
        public DiamondCriteriaId? CriteriaId { get; set; }
        public DiamondCriteria? Criteria { get; set; }
        public DiamondPrice? PriceFound { get; set; }
        public bool IsFancyShape { get; set; }
        public bool IsLabDiamond { get; set; }
        public decimal NewPrice { get; set; }
        public bool IsSideDiamond { get; set; } = false;
    }

}
