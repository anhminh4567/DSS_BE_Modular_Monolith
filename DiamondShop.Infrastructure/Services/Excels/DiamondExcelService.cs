using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Excels
{
    internal class DiamondExcelService : ExcelSyncfunctionService, IDiamondExcelService
    {
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;

        public DiamondExcelService(IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository) : base()
        {
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }

        public async Task<List<MainPriceFromExcelResponse>> ReadPrices(string sheetName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
