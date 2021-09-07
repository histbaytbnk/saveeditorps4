
// Type: Rss.RssModuleCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssModuleCollection : CollectionBase
  {
    public RssModule this[int index]
    {
      get
      {
        return (RssModule) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public int Add(RssModule rssModule)
    {
      return this.List.Add((object) rssModule);
    }

    public bool Contains(RssModule rssModule)
    {
      return this.List.Contains((object) rssModule);
    }

    public void CopyTo(RssModule[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssModule rssModule)
    {
      return this.List.IndexOf((object) rssModule);
    }

    public void Insert(int index, RssModule rssModule)
    {
      this.List.Insert(index, (object) rssModule);
    }

    public void Remove(RssModule rssModule)
    {
      this.List.Remove((object) rssModule);
    }
  }
}
