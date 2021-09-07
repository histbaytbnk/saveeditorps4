
// Type: Rss.RssFeedCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssFeedCollection : CollectionBase
  {
    public RssFeed this[int index]
    {
      get
      {
        return (RssFeed) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public RssFeed this[string url]
    {
      get
      {
        for (int index = 0; index < this.List.Count; ++index)
        {
          if (((RssFeed) this.List[index]).Url == url)
            return this[index];
        }
        return (RssFeed) null;
      }
    }

    public int Add(RssFeed feed)
    {
      return this.List.Add((object) feed);
    }

    public bool Contains(RssFeed rssFeed)
    {
      return this.List.Contains((object) rssFeed);
    }

    public void CopyTo(RssFeed[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssFeed rssFeed)
    {
      return this.List.IndexOf((object) rssFeed);
    }

    public void Insert(int index, RssFeed feed)
    {
      this.List.Insert(index, (object) feed);
    }

    public void Remove(RssFeed feed)
    {
      this.List.Remove((object) feed);
    }
  }
}
