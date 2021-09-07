
// Type: Rss.RssSource


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssSource : RssElement
  {
    private string name = "";
    private Uri uri = RssDefault.Uri;

    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = RssDefault.Check(value);
      }
    }

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
  }
}
