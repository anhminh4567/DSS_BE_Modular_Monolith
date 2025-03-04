﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.Enums
{
    public enum Fluorescence
    {
        None = 1, Faint = 2, Medium = 3, Strong = 4
    }
    public static class FluorescenceHelper 
    {
        public static List<Fluorescence> GetFluorescencesList()
        {
            return Enum.GetValues(typeof(Fluorescence)).Cast<Fluorescence>().ToList();
        }
    }

}
