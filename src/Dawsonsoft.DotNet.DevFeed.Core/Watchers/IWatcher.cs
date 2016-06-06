using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dawsonsoft.DotNet.DevFeed.Core.Watchers
{
    public class ChangedEventArgs { }

    public delegate void ChangedEventHandler(object sender, ChangedEventArgs args);

    public interface IWatcher
    {
        event ChangedEventHandler Changed;
        void Finish();
    }
}
