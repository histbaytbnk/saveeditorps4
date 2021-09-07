
// Type: Rss.RssModuleItemCollectionCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssModuleItemCollectionCollection : CollectionBase
  {
    public RssModuleItemCollection this[int index]
    {
      get
      {
        return (RssModuleItemCollection) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public int Add(RssModuleItemCollection rssModuleItemCollection)
    {
      return this.List.Add((object) rssModuleItemCollection);
    }

    public bool Contains(RssModuleItemCollection rssModuleItemCollection)
    {
      return this.List.Contains((object) rssModuleItemCollection);
    }

    public void CopyTo(RssModuleItemCollection[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssModuleItemCollection rssModuleItemCollection)
    {
      return this.List.IndexOf((object) rssModuleItemCollection);
    }

    public void Insert(int index, RssModuleItemCollection rssModuleItemCollection)
    {
      this.List.Insert(index, (object) rssModuleItemCollection);
    }

    public void Remove(RssModuleItemCollection rssModuleItemCollection)
    {
      this.List.Remove((object) rssModuleItemCollection);
    }
  }
}
