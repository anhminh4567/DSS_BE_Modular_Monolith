using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Application.Usecases.Diamonds.Files.Queries;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Queries
{
    public record GetAllModelImagesQuery(string jewelryModelId) : IRequest<GalleryTemplate>;
    internal class GetAllModelImagesQueryHandler : IRequestHandler<GetAllModelImagesQuery, GalleryTemplate>
    {
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ILogger<GetAllModelImagesQueryHandler> _logger;
        public GetAllModelImagesQueryHandler(IJewelryModelFileService jewelryModelFileService, ILogger<GetAllModelImagesQueryHandler> logger, IJewelryModelRepository jewelryModelRepository)
        {
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = jewelryModelRepository;
            _logger = logger;
        }

        public async Task<GalleryTemplate> Handle(GetAllModelImagesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all images of diamond is excecuted");
            var parsedResult = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(parsedResult);
            if (getJewelry is null)
                throw new NullReferenceException("Diamond not found");
            
            var getResults = await _jewelryModelFileService.GetFolders(getJewelry, cancellationToken);
            var gallery = _jewelryModelFileService.MapPathsToCorrectGallery(getJewelry, getResults, cancellationToken);
            return gallery;
        }
    }
   
}
