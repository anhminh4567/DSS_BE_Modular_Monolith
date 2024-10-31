using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IApplicationSettingService
    {
        void ReloadAllConfiguration();
        void InitConfiguration();
        object? Get(string key);
        object Set(string key, object value);
    }
}
