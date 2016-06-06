namespace Dawsonsoft.DotNet.DevFeed.Core.Models
{
    public class DevFeedPackageVersionInfo
    {
        public string staticSegment { get; set; }
        public int versionSuffix { get; set; }
        public string NextVersion => $"{staticSegment}{versionSuffix++}";

        public DevFeedPackageVersionInfo(string staticSegment, int versionSuffix)
        {
            this.staticSegment = staticSegment;
            this.versionSuffix = versionSuffix;
        }
    }
}