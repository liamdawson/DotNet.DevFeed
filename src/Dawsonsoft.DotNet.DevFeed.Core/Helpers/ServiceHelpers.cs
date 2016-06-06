using Dawsonsoft.DotNet.DevFeed.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Core.Helpers
{
    public static class ServiceHelpers
    {

        public static void AddPackageResolutionService(this IServiceCollection serviceCollection, IPackageResolutionServiceSettings settings)
        {
            serviceCollection.AddSingleton(settings);
            serviceCollection.AddScoped<IPackageResolutionService, PackageResolutionService>();
        }

    }
}
