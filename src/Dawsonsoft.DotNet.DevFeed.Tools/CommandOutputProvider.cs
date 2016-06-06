using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Dawsonsoft.DotNet.DevFeed.Tools
{
    // Taken from https://raw.githubusercontent.com/aspnet/dotnet-watch/060a1c8f5ae4d92e6accaa07b9662b9ad55e7e8d/src/Microsoft.DotNet.Watcher.Tools/CommandOutputProvider.cs
    
    public class CommandOutputProvider : ILoggerProvider
    {
        private readonly bool _isWindows;

        public CommandOutputProvider()
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public ILogger CreateLogger(string name)
        {
            return new CommandOutputLogger(this, name, useConsoleColor: _isWindows);
        }

        public void Dispose()
        {
        }

        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
