
// Type: Rss.RssChannelCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssChannelCollection : CollectionBase
  {
    public RssChannel this[int index]
    {
      get
      {
        return (RssChannel) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public int Add(RssChannel channel)
    {
      return this.List.Add((object) channel);
    }

    public bool Contains(RssChannel rssChannel)
    {
      return this.List.Contains((object) rssChannel);
    }

    public void CopyTo(RssChannel[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssChannel rssChannel)
    {
      return this.List.IndexOf((object) rssChannel);
    }

    public void Insert(int index, RssChannel channel)
    {
      this.List.Insert(index, (object) channel);
    }

    public void Remove(RssChannel channel)
    {
      this.List.Remove((object) channel);
    }
  }
}
