using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Promotions.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Promotions
{
	public class DiamondSpecDto
	{
		public DiamondOrigin Origin { get; set; }
		public string[] ShapesIDs { get; set; } 
		public float CaratFrom { get; set; }
		public float CaratTo { get; set; }
		public Clarity ClarityFrom { get; set; }
		public Clarity ClarityTo { get; set; }
		public Cut CutFrom { get; set; }
		public Cut CutTo { get; set; }
		public Color ColorFrom { get; set; }
		public Color ColorTo { get; set; }
	}
}
