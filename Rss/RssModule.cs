
// Type: Rss.RssModule


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public abstract class RssModule
  {
    private ArrayList _alBindTo = new ArrayList();
    private RssModuleItemCollection _rssChannelExtensions = new RssModuleItemCollection();
    private RssModuleItemCollectionCollection _rssItemExtensions = new RssModuleItemCollectionCollection();
    private string _sNamespacePrefix = "";
    private Uri _uriNamespaceURL = RssDefault.Uri;

    internal RssModuleItemCollection ChannelExtensions
    {
      get
      {
        return this._rssChannelExtensions;
      }
      set
      {
        this._rssChannelExtensions = value;
      }
    }

    internal RssModuleItemCollectionCollection ItemExtensions
    {
      get
      {
        return this._rssItemExtensions;
      }
      set
      {
        this._rssItemExtensions = value;
      }
    }

    public string NamespacePrefix
    {
      get
      {
        return this._sNamespacePrefix;
      }
      set
      {
        this._sNamespacePrefix = RssDefault.Check(value);
      }
    }

    public Uri NamespaceURL
    {
      get
      {
        return this._uriNamespaceURL;
      }
      set
      {
        this._uriNamespaceURL = RssDefault.Check(value);
      }
    }

    public void BindTo(int channelHashCode)
    {
      this._alBindTo.Add((object) channelHashCode);
    }

    public bool IsBoundTo(int channelHashCode)
    {
      return this._alBindTo.BinarySearch(0, this._alBindTo.Count, (object) channelHashCode, (IComparer) null) >= 0;
    }
  }
}
