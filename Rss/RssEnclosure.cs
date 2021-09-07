
// Type: Rss.RssEnclosure


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssEnclosure : RssElement
  {
    private Uri uri = RssDefault.Uri;
    private int length = -1;
    private string type = "";

    public Uri Url
    {
      get
      {
        return this.uri;
      }
      set
      {
        this.uri = RssDefault.Check(value);
      }
    }

    public int Length
    {
      get
      {
        return this.length;
      }
      set
      {
        this.length = RssDefault.Check(value);
      }
    }

    public string Type
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = RssDefault.Check(value);
      }
    }
  }
}
