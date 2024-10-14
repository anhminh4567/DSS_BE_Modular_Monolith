using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.GetAllAttributes
{
    public record GetAllDiamondAttributesQuery() : IRequest<Dictionary<string,Dictionary<string, int>>>;
    internal class GetAllDiamondAttributesQueryHandler : IRequestHandler<GetAllDiamondAttributesQuery, Dictionary<string, Dictionary<string, int>>>
    {
        public async Task<Dictionary<string, Dictionary<string, int>>> Handle(GetAllDiamondAttributesQuery request, CancellationToken cancellationToken)
        {
            var response = new Dictionary<string, Dictionary<string, int>>();
            var clairtyResponse = new Dictionary<string, int>();
            var enums = new List<Type>
            {
                typeof(Clarity),
                typeof(Color),
                typeof(Cut),
                typeof(Polish),
                typeof(Symmetry),
                typeof(Girdle),
                typeof(Culet),
                typeof(Fluorescence)
            };

            foreach (var enumType in enums)
            {
                var enumDict = Enum.GetValues(enumType)
                                   .Cast<Enum>()
                                   .ToDictionary(e => e.ToString(), e => Convert.ToInt32(e));
                response.Add(enumType.Name, enumDict);
            }

            return response;
        }
    }
    //public Clarity Clarity { get; set; }
    //public Color Color { get; set; }
    //public Cut? Cut { get; set; }
    //public Polish Polish { get; set; }
    //public Symmetry Symmetry { get; set; }
    //public Girdle Girdle { get; set; }
    //public Culet Culet { get; set; }
    //public Fluorescence Fluorescence { get; set; }
}
