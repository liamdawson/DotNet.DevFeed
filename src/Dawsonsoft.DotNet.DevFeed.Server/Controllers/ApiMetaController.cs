using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Server.Controllers
{
    [Route("/v3/")]
    public class ApiMetaController : Controller
    {
        [Route("index.json")]
        public IActionResult Index()
        {
            /*var flatContainerUrl = Url.RouteUrl(new Microsoft.AspNetCore.Mvc.Routing.UrlRouteContext {
                Host = HttpContext.Request.Host.Value,
                Protocol = HttpContext.Request.Protocol.ToString(),
                RouteName = "flatcontainer-root",
                Values = null
            });*/
            var flatContainerUrl = $"{HttpContext.Request.Scheme.ToString()}://{HttpContext.Request.Host.Value}/v3-flatcontainer/"; // Url.Link("flatcontainer-root", null);
            return Ok("{  \"version\": \"3.0.0-beta.1\",\r\n  \"resources\": [\r\n    {\r\n      \"@id\": \"" + flatContainerUrl + "\",\r\n      \"@type\": \"PackageBaseAddress/3.0.0\",\r\n      \"comment\": \"Base URL of Azure storage where NuGet package registration info for DNX is stored, in the format https://api.nuget.org/v3-flatcontainer/{id-lower}/{version-lower}.{version-lower}.nupkg\"\r\n    }\r\n  ],\r\n  \"@context\": {\r\n    \"@vocab\": \"http://schema.nuget.org/services#\",\r\n    \"comment\": \"http://www.w3.org/2000/01/rdf-schema#comment\"\r\n  }\r\n}");
        }
    }
}
