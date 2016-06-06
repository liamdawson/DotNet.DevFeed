using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Dawsonsoft.DotNet.DevFeed.Core.Helpers;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Dawsonsoft.DotNet.DevFeed.Server
{
    public class DevFeedServer
    {
        private readonly IHostingEnvironment _hostingEnv;
        public DevFeedServer(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            services.AddPackageResolutionService(new Core.Services.IPackageResolutionServiceSettings
            {
                BasePackageLocation = new System.Uri(Path.Combine("file://", _hostingEnv.WebRootPath))
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Warning);

            app.UseMvc();

        }
    }
}
