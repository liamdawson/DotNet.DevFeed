using Dawsonsoft.DotNet.DevFeed.Core.Services;
using Dawsonsoft.DotNet.DevFeed.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Server.Controllers
{
    [Route("/v3-flatcontainer/{packageName}/")]
    public class FlatContainer : Controller
    {

        private readonly IPackageResolutionService _packageResolutionService;

        public FlatContainer(IPackageResolutionService packageResolutionService)
        {
            _packageResolutionService = packageResolutionService;
        }

        [HttpGet("")]
        public IActionResult Root()
        {
            return NotFound();
        }

        [HttpGet("index.json")]
        [Produces("application/json")]
        public IActionResult Index(string packageName)
        {
            var packageVersions = _packageResolutionService.GetVersionsForPackage(packageName) ?? new string[] { };
            if(packageVersions.Any())
            {
                return new OkObjectResult(
                    new FlatContainerPackageVersionInfo
                    {
                        Versions = packageVersions
                    });
            }
            return NotFound();
        }

        [HttpGet("{packageVersion}/{unused}.nupkg")]
        public IActionResult Download(string packageName, string packageVersion, string unused)
        {
            return new FileStreamResult(_packageResolutionService.FileStream(packageName, packageVersion), "application/octet-stream");
        }
    }
}
