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
    public interface IDiamondExcelService
    {
        Task<List<DiamondCriteria>> ReadCriteriaFromExcel(IFormFile excelFile, CancellationToken cancellationToken = default);
        Task<List<PriceFromExcelResponse>> ReadPrices(IFormFile excelFile,CancellationToken cancellationToken = default);
    }
    public class PriceFromExcelResponse
    {
        DiamondCriteriaId CriteriaId { get; set; }
        DiamondShapeId ShapeId { get; set; }
        bool IsLabDiamond { get; set; }
        decimal Price { get; set; }
    }

}
