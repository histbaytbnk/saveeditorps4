
// Type: Rss.RssBlogChannel


// Hacked by SystemAce

using System;

namespace Rss
{
  public sealed class RssBlogChannel : RssModule
  {
    public RssBlogChannel(Uri blogRoll, Uri mySubscriptions, Uri blink, Uri changes)
    {
      this.NamespacePrefix = "blogChannel";
      this.NamespaceURL = new Uri("http://backend.userland.com/blogChannelModule");
      this.ChannelExtensions.Add(new RssModuleItem("blogRoll", true, RssDefault.Check(blogRoll.ToString())));
      this.ChannelExtensions.Add(new RssModuleItem("mySubscriptions", true, RssDefault.Check(mySubscriptions.ToString())));
      this.ChannelExtensions.Add(new RssModuleItem("blink", true, RssDefault.Check(blink.ToString())));
      this.ChannelExtensions.Add(new RssModuleItem("changes", true, RssDefault.Check(changes.ToString())));
    }
  }
}
