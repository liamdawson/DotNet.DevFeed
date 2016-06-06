using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Core.Services
{
    public interface IPackageResolutionService
    {

        string[] GetVersionsForPackage(string packageName);
        Stream FileStream(string packageName, string packageVersion);
        string FilePath(string packageName, string packageVersion);
    }
}
