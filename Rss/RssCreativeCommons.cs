
// Type: Rss.RssCreativeCommons


// Hacked by SystemAce

using System;

namespace Rss
{
  public sealed class RssCreativeCommons : RssModule
  {
    public RssCreativeCommons(Uri license, bool isChannelSubElement)
    {
      this.NamespacePrefix = "creativeCommons";
      this.NamespaceURL = new Uri("http://backend.userland.com/creativeCommonsRssModule");
      if (isChannelSubElement)
        this.ChannelExtensions.Add(new RssModuleItem("license", true, RssDefault.Check(license.ToString())));
      else
        this.ItemExtensions.Add(new RssModuleItemCollection()
        {
          new RssModuleItem("license", true, RssDefault.Check(license.ToString()))
        });
    }
  }
}
