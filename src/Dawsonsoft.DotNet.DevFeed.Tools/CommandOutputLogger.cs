﻿using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Tools
{
    // Taken from https://raw.githubusercontent.com/aspnet/dotnet-watch/c248e539f3004bb073efe0c94dc417dd5848202d/src/Microsoft.DotNet.Watcher.Tools/CommandOutputLogger.cs

    /// <summary>
    /// Logger to print formatted command output.
    /// </summary>
    public class CommandOutputLogger : ILogger
    {
        private readonly CommandOutputProvider _provider;
        private readonly AnsiConsole _outConsole;
        private readonly string _loggerName;

        public CommandOutputLogger(CommandOutputProvider commandOutputProvider, string loggerName, bool useConsoleColor)
        {
            _provider = commandOutputProvider;
            _outConsole = AnsiConsole.GetOutput(useConsoleColor);
            _loggerName = loggerName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel < _provider.LogLevel)
            {
                return false;
            }

            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                _outConsole.WriteLine($"[{_loggerName}] {Caption(logLevel)}: {formatter(state, exception)}");
            }
        }

        private string Caption(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace: return "\x1b[35mtrace\x1b[39m";
                case LogLevel.Debug: return "\x1b[35mdebug\x1b[39m";
                case LogLevel.Information: return "\x1b[32minfo\x1b[39m";
                case LogLevel.Warning: return "\x1b[33mwarn\x1b[39m";
                case LogLevel.Error: return "\x1b[31mfail\x1b[39m";
                case LogLevel.Critical: return "\x1b[31mcritical\x1b[39m";
            }

            throw new Exception("Unknown LogLevel");
        }
    }
}
