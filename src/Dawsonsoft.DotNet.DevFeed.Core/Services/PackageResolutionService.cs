using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace Dawsonsoft.DotNet.DevFeed.Core.Services
{
    public class PackageResolutionService : IPackageResolutionService
    {
        private readonly IPackageResolutionServiceSettings _settings;
        private string PackageBaseDir => _settings.BasePackageLocation.AbsolutePath;

        public PackageResolutionService(IPackageResolutionServiceSettings settings)
        {
            _settings = settings;
        }

        internal string[] GetMatchingPackagePaths(string packageName)
        {
            var pathVersionRegex = new Regex($"{Regex.Escape(packageName)}\\.\\d+.*", RegexOptions.IgnoreCase);
            return Directory.GetFiles(PackageBaseDir, $"*.nupkg", SearchOption.TopDirectoryOnly)
                .Where(filepath => !filepath.EndsWith("symbols.nupkg"))
                .Where(filepath => pathVersionRegex.IsMatch(Path.GetFileNameWithoutExtension(filepath)))
                .ToArray();
        }

        public string[] GetVersionsForPackage(string packageName)
        {
            var matchingPackages = GetMatchingPackagePaths(packageName);
            if(!matchingPackages.Any())
            {
                return null;
            }

            return matchingPackages
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .Select(filename => filename.Substring(packageName.Length + 1, filename.Length - (packageName.Length + 1)))
                .ToArray();
        }

        public Stream FileStream(string packageName, string packageVersion)
        {
            return new FileStream(FilePath(packageName, packageVersion), FileMode.Open, FileAccess.Read);
        }

        public string FilePath(string packageName, string packageVersion)
        {
            return Directory.GetFiles(PackageBaseDir)
                .Where(filepath => Path.GetFileName(filepath).Equals($"{packageName}.{packageVersion}.nupkg", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }
    }
}
