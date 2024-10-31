using DiamondShop.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class StartupServices : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Application start, begin loading necessary data");
            Console.ResetColor();
            using var scope = _serviceProvider.CreateScope();
            var settingsCache = scope.ServiceProvider.GetRequiredService<IApplicationSettingService>();
            settingsCache.InitConfiguration();
            return;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Application stop");
            Console.ResetColor();
            return;
        }
    }
}
