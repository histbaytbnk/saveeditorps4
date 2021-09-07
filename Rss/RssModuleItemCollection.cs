
// Type: Rss.RssModuleItemCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class RssModuleItemCollection : CollectionBase
  {
    private ArrayList _alBindTo = new ArrayList();

    public RssModuleItem this[int index]
    {
      get
      {
        return (RssModuleItem) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public int Add(RssModuleItem rssModuleItem)
    {
      return this.List.Add((object) rssModuleItem);
    }

    public bool Contains(RssModuleItem rssModuleItem)
    {
      return this.List.Contains((object) rssModuleItem);
    }

    public void CopyTo(RssModuleItem[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(RssModuleItem rssModuleItem)
    {
      return this.List.IndexOf((object) rssModuleItem);
    }

    public void Insert(int index, RssModuleItem rssModuleItem)
    {
      this.List.Insert(index, (object) rssModuleItem);
    }

    public void Remove(RssModuleItem rssModuleItem)
    {
      this.List.Remove((object) rssModuleItem);
    }

    public void BindTo(int itemHashCode)
    {
      this._alBindTo.Add((object) itemHashCode);
    }

    public bool IsBoundTo(int itemHashCode)
    {
      return this._alBindTo.BinarySearch(0, this._alBindTo.Count, (object) itemHashCode, (IComparer) null) >= 0;
    }
  }
}
