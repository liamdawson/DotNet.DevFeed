using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.IO;
using Microsoft.Extensions.Configuration;
using Dawsonsoft.DotNet.DevFeed.Server;
using Microsoft.Extensions.CommandLineUtils;
using System.Threading;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Dawsonsoft.DotNet.DevFeed.Core.Watchers;
using System.Threading.Tasks;
using System.Diagnostics;
using Dawsonsoft.DotNet.DevFeed.Core.Models;
using Dawsonsoft.DotNet.DevFeed.Core.Services;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Dawsonsoft.DotNet.DevFeed.Tools
{
    public class Program
    {
        private readonly ILoggerFactory _loggerFactory;

        public Program()
        {
            _loggerFactory = new LoggerFactory();

            var commandProvider = new CommandOutputProvider();
            _loggerFactory.AddProvider(commandProvider);
        }

        public static int Main(string[] args)
        {
            using (CancellationTokenSource ctrlCTokenSource = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, ev) =>
                {
                    ctrlCTokenSource.Cancel();
                    ev.Cancel = false;
                };

                return new Program().MainInternal(args, ctrlCTokenSource.Token);
            }
        }

        public int MainInternal(string[] args, CancellationToken cancellationToken)
        {
            var app = new CommandLineApplication();
            app.Name = "dotnet-dev-feed";
            app.FullName = "dotnet dev feed tool";

            app.HelpOption("-?|-h|--help");
            var projectOption = app.Argument("project-dir", "Project the feed targets", true);
            var portOption = app.Option("--port <PORT>", "Port for the dev feed server", CommandOptionType.SingleValue);
            var refreshOption = app.Option("--refresh <REFRESH_COMMAND>", "Command to run when feed is updated", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var logger = _loggerFactory.CreateLogger<Program>();

                var port = int.Parse(portOption.Value() ?? "5000");

                var projectDir = Path.GetFullPath(projectOption.Value ?? Directory.GetCurrentDirectory());
                var outputDir = Path.Combine(projectDir, "bin", "Debug");
                var projectWatcher = new ProjectOutputWatcher(outputDir);

                var projectName = Path.GetFileName(projectDir);

                var staticSegment = "devf-";
                var packageResolver = new PackageResolutionService(new IPackageResolutionServiceSettings { BasePackageLocation = new Uri(Path.Combine("file://", outputDir)) });

                var packageJson = JObject.Parse(File.ReadAllText(Path.Combine(projectDir, "project.json")));
                var versionString = packageJson["version"].Value<string>();

                var versionRegex = new Regex($"^{Regex.Escape(versionString.Replace("*", staticSegment))}(\\d+)$");
                var existingVersions = packageResolver.GetVersionsForPackage(projectName)?
                    .Select(version => versionRegex.Match(version).Groups)?
                    .Where(capturedGroups => capturedGroups.Count > 1)?
                    .Select(capturedGroups => int.Parse(capturedGroups[1].Value)) ?? new int[] { };

                var prevMax = 0;

                if(existingVersions.Any())
                {
                    prevMax = existingVersions.Max();
                }

                var versionInfo = new DevFeedPackageVersionInfo(staticSegment, prevMax + 1);
                
                logger.LogInformation($"Watching {outputDir} for changes...");

                projectWatcher.Changed += (sender, e) =>
                {
                    logger.LogInformation("Repacking project.");
                    Task.Factory.StartNew(() =>
                    {
                        Process.Start(new ProcessStartInfo("dotnet", $"pack --version-suffix {versionInfo.NextVersion}")
                        {
                            WorkingDirectory = projectDir
                        }).WaitForExit();
                        if(refreshOption.HasValue())
                        {
                            logger.LogInformation("Running refresh command.");
                            var res = Process.Start(new ProcessStartInfo("cmd.exe", $"/c {refreshOption.Value()}")
                            {
                                WorkingDirectory = projectDir
                            });
                        }
                    });
                };

                var configuration = new ConfigurationBuilder();

                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(projectDir)
                    .UseStartup<DevFeedServer>()
                    .UseWebRoot(Path.Combine(projectDir, "bin", "debug"))
                    .UseUrls(new [] { $"http://localhost:{port}"})
                    .Build();

                try
                {
                    host.Run(cancellationToken);
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Count != 1 || !(ex.InnerException is TaskCanceledException))
                    {
                        throw;
                    }
                }
                finally
                {
                    projectWatcher.Finish();
                }

                return 0;
            });

            return app.Execute(args);
        }
    }
}
