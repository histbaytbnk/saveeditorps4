
// Type: Rss.RssTextInput


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssTextInput : RssElement
  {
    private string title = "";
    private string description = "";
    private string name = "";
    private Uri link = RssDefault.Uri;

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
  }
}
