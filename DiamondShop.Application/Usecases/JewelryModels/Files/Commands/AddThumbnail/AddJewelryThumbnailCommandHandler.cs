using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.AddThumbnail
{
    public record AddJewelryThumbnailCommand(string jewelryModelId, IFormFile FormFile) : IRequest<Result<string>>;
    internal class AddJewelryThumbnailCommandHandler : IRequestHandler<AddJewelryThumbnailCommand, Result<string>>
    {
        public Task<Result<string>> Handle(AddJewelryThumbnailCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
