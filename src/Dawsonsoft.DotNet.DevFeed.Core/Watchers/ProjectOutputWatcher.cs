using System;
using System.IO;
using System.Threading;

namespace Dawsonsoft.DotNet.DevFeed.Core.Watchers
{
    public class ProjectOutputWatcher : IWatcher
    {
        private readonly FileSystemWatcher _watcher;
        private Timer _debouncer;
        private object locker = new object();

        public ProjectOutputWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path, "*.dll");
            _watcher.IncludeSubdirectories = true;

            _watcher.Changed += (_, __) => OnUpdate();
            _watcher.Created += (_, __) => OnUpdate();
            _watcher.Deleted += (_, __) => OnUpdate();
            _watcher.Renamed += (_, __) => OnUpdate();

            _watcher.EnableRaisingEvents = true;
        }

        internal void OnUpdate()
        {
            if(_debouncer != null)
            {
                lock(locker)
                {
                    if (_debouncer != null)
                    {
                        _debouncer.Dispose();
                    }
                }
            }
            _debouncer = new Timer(new TimerCallback(_ => Changed(this, new ChangedEventArgs())), 0, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
        }

        public void Finish()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public event ChangedEventHandler Changed;
    }
}
