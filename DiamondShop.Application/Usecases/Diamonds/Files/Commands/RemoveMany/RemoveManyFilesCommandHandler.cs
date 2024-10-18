using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveMany
{
    public record RemoveManyFilesCommand(string diamondId , string[] absolutePaths) : IRequest<Result>;
    internal class RemoveManyFilesCommandHandler : IRequestHandler<RemoveManyFilesCommand, Result>
    {
        private readonly IDiamondFileService _diamondFileService;
        private readonly IDiamondRepository _diamondRepository;

        public RemoveManyFilesCommandHandler(IDiamondFileService diamondFileService, IDiamondRepository diamondRepository)
        {
            _diamondFileService = diamondFileService;
            _diamondRepository = diamondRepository;
        }

        public async Task<Result> Handle(RemoveManyFilesCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(parsedId);
            if (getDiamond is null)
            {
                return Result.Fail(new NotFoundError());
            }
            string[] relativePath = request.absolutePaths.Select(x => _diamondFileService.ToRelativePath(x)).ToArray();
            List<Task<Result>> runningDelete = new(); 
            foreach(var path in relativePath)
            {
                runningDelete.Add (_diamondFileService.DeleteFileAsync(path, cancellationToken));
            }
            var result = await Task.WhenAll(runningDelete);
            if (result.Any(x => x.IsSuccess))
                return Result.Ok();
            return Result.Fail("unable to delete any of the given files");
        }
    }
}
