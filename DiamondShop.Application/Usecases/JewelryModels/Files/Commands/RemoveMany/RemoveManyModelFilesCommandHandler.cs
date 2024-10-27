using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveMany;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.RemoveMany
{
    public record RemoveManyModelFilesCommand(string jewelryModelId, string[] absolutePaths) : IRequest<Result>;
    internal class RemoveManyModelFilesCommandHandler : IRequestHandler<RemoveManyModelFilesCommand, Result>
    {
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;

        public RemoveManyModelFilesCommandHandler(IJewelryModelFileService jewelryModelFileService, IJewelryModelRepository jewelryModelRepository)
        {
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result> Handle(RemoveManyModelFilesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = JewelryModelId.Parse(request.jewelryModelId);
            var getDiamond = await _jewelryModelRepository.GetById(parsedId);
            if (getDiamond is null)
                return Result.Fail(new NotFoundError());
            
            string[] relativePath = request.absolutePaths.Select(x => _jewelryModelFileService.ToRelativePath(x)).ToArray();
            List<Task<Result>> runningDelete = new();
            foreach (var path in relativePath)
            {
                runningDelete.Add(_jewelryModelFileService.DeleteFileAsync(path, cancellationToken));
            }
            var result = await Task.WhenAll(runningDelete);
            if (result.Any(x => x.IsSuccess))
                return Result.Ok();
            return Result.Fail("unable to delete any of the given files");
        }
    }

}
