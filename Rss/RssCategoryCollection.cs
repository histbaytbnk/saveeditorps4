
// Type: Rss.RssCategoryCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssCategoryCollection : CollectionBase
  {
    public RssCategory this[int index]
    {
      get
      {
        return (RssCategory) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public int Add(RssCategory rssCategory)
    {
      return this.List.Add((object) rssCategory);
    }

    public bool Contains(RssCategory rssCategory)
    {
      return this.List.Contains((object) rssCategory);
    }

    public void CopyTo(RssCategory[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssCategory rssCategory)
    {
      return this.List.IndexOf((object) rssCategory);
    }

    public void Insert(int index, RssCategory rssCategory)
    {
      this.List.Insert(index, (object) rssCategory);
    }

    public void Remove(RssCategory rssCategory)
    {
      this.List.Remove((object) rssCategory);
    }
  }
}
