
// Type: Rss.RssItem


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssItem : RssElement
  {
    private string title = "";
    private Uri link = RssDefault.Uri;
    private string description = "";
    private string author = "";
    private RssCategoryCollection categories = new RssCategoryCollection();
    private string comments = "";
    private DateTime pubDate = RssDefault.DateTime;
    private RssEnclosure enclosure;
    private RssGuid guid;
    private RssSource source;

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

    public string Author
    {
      get
      {
        return this.author;
      }
      set
      {
        this.author = RssDefault.Check(value);
      }
    }

    public RssCategoryCollection Categories
    {
      get
      {
        return this.categories;
      }
    }

    public string Comments
    {
      get
      {
        return this.comments;
      }
      set
      {
        this.comments = RssDefault.Check(value);
      }
    }

    public RssSource Source
    {
      get
      {
        return this.source;
      }
      set
      {
        this.source = value;
      }
    }

    public RssEnclosure Enclosure
    {
      get
      {
        return this.enclosure;
      }
      set
      {
        this.enclosure = value;
      }
    }

    public RssGuid Guid
    {
      get
      {
        return this.guid;
      }
      set
      {
        this.guid = value;
      }
    }

    public DateTime PubDate
    {
      get
      {
        return this.pubDate;
      }
      set
      {
        this.pubDate = value;
      }
    }

    public override string ToString()
    {
      if (this.title != null)
        return this.title;
      if (this.description != null)
        return this.description;
      return "RssItem";
    }
  }
}
