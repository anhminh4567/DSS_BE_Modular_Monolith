using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Queries
{
    public record GetAllImagesQuery(string diamondId): IRequest<GalleryTemplate>;
    internal class GetAllImagesQueryHandler : IRequestHandler<GetAllImagesQuery, GalleryTemplate>
    {
        private readonly IDiamondFileService _diamondFileService;
        private readonly ILogger<GetAllImagesQueryHandler> _logger;
        private readonly IDiamondRepository _diamondRepository;

        public GetAllImagesQueryHandler(IDiamondFileService diamondFileService, ILogger<GetAllImagesQueryHandler> logger, IDiamondRepository diamondRepository)
        {
            _diamondFileService = diamondFileService;
            _logger = logger;
            _diamondRepository = diamondRepository;
        }

        public async Task<GalleryTemplate> Handle(GetAllImagesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all images of diamond is excecuted");
            var parsedResult = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(parsedResult);    
            if(getDiamond is null)
            {
                throw new NullReferenceException("Diamond not found");
            }
            var getResults = await  _diamondFileService.GetFolders(getDiamond, cancellationToken);
            var gallery = _diamondFileService.MapPathsToCorrectGallery(getDiamond, getResults, cancellationToken);
            gallery.Certificates.Clear();
            if(getDiamond.CertificateFilePath != null)
                gallery.Certificates.Add(getDiamond.CertificateFilePath);
            
            return gallery;
            //throw new NotImplementedException();
        }
    }
}
