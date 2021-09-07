
// Type: Rss.RssPhotoAlbum


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public sealed class RssPhotoAlbum : RssModule
  {
    public Uri Link
    {
      get
      {
        if (!(RssDefault.Check(this.ChannelExtensions[0].Text) == ""))
          return new Uri(this.ChannelExtensions[0].Text);
        return (Uri) null;
      }
      set
      {
        this.ChannelExtensions[0].Text = RssDefault.Check(value) == RssDefault.Uri ? "" : value.ToString();
      }
    }

    public RssPhotoAlbum(Uri link, RssPhotoAlbumCategory photoAlbumCategory)
    {
      this.NamespacePrefix = "photoAlbum";
      this.NamespaceURL = new Uri("http://xml.innothinx.com/photoAlbum");
      this.ChannelExtensions.Add(new RssModuleItem("link", true, RssDefault.Check(link).ToString()));
      this.ItemExtensions.Add((RssModuleItemCollection) photoAlbumCategory);
    }

    public RssPhotoAlbum(Uri link, RssPhotoAlbumCategories photoAlbumCategories)
    {
      this.NamespacePrefix = "photoAlbum";
      this.NamespaceURL = new Uri("http://xml.innothinx.com/photoAlbum");
      this.ChannelExtensions.Add(new RssModuleItem("link", true, RssDefault.Check(link).ToString()));
      foreach (RssModuleItemCollection rssModuleItemCollection in (CollectionBase) photoAlbumCategories)
        this.ItemExtensions.Add(rssModuleItemCollection);
    }
  }
}
