
// Type: Rss.RssImage


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssImage : RssElement
  {
    private string title = "";
    private string description = "";
    private Uri uri = RssDefault.Uri;
    private Uri link = RssDefault.Uri;
    private int width = -1;
    private int height = -1;

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

    public string Title
    {
      get
      {
        return this.title;
      }
      set
      {
        this.title = RssDefault.Check(value);
      }
    }

    public Uri Link
    {
      get
      {
        return this.link;
      }
      set
      {
        this.link = RssDefault.Check(value);
      }
    }

    public string Description
    {
      get
      {
        return this.description;
      }
      set
      {
        this.description = RssDefault.Check(value);
      }
    }

    public int Width
    {
      get
      {
        return this.width;
      }
      set
      {
        this.width = RssDefault.Check(value);
      }
    }

    public int Height
    {
      get
      {
        return this.height;
      }
      set
      {
        this.height = RssDefault.Check(value);
      }
    }
  }
}
