
// Type: Rss.RssChannel


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssChannel : RssElement
  {
    private string title = "";
    private Uri link = RssDefault.Uri;
    private string description = "";
    private string language = "";
    private string copyright = "";
    private string managingEditor = "";
    private string webMaster = "";
    private DateTime pubDate = RssDefault.DateTime;
    private DateTime lastBuildDate = RssDefault.DateTime;
    private RssCategoryCollection categories = new RssCategoryCollection();
    private string generator = "";
    private string docs = "";
    private int timeToLive = -1;
    private bool[] skipHours = new bool[24];
    private bool[] skipDays = new bool[7];
    private string rating = "";
    private RssItemCollection items = new RssItemCollection();
    private RssCloud cloud;
    private RssImage image;
    private RssTextInput textInput;

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

    public string Language
    {
      get
      {
        return this.language;
      }
      set
      {
        this.language = RssDefault.Check(value);
      }
    }

    public RssImage Image
    {
      get
      {
        return this.image;
      }
      set
      {
        this.image = value;
      }
    }

    public string Copyright
    {
      get
      {
        return this.copyright;
      }
      set
      {
        this.copyright = RssDefault.Check(value);
      }
    }

    public string ManagingEditor
    {
      get
      {
        return this.managingEditor;
      }
      set
      {
        this.managingEditor = RssDefault.Check(value);
      }
    }

    public string WebMaster
    {
      get
      {
        return this.webMaster;
      }
      set
      {
        this.webMaster = RssDefault.Check(value);
      }
    }

    public string Rating
    {
      get
      {
        return this.rating;
      }
      set
      {
        this.rating = RssDefault.Check(value);
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

    public DateTime LastBuildDate
    {
      get
      {
        return this.lastBuildDate;
      }
      set
      {
        this.lastBuildDate = value;
      }
    }

    public RssCategoryCollection Categories
    {
      get
      {
        return this.categories;
      }
    }

    public string Generator
    {
      get
      {
        return this.generator;
      }
      set
      {
        this.generator = RssDefault.Check(value);
      }
    }

    public string Docs
    {
      get
      {
        return this.docs;
      }
      set
      {
        this.docs = RssDefault.Check(value);
      }
    }

    public RssTextInput TextInput
    {
      get
      {
        return this.textInput;
      }
      set
      {
        this.textInput = value;
      }
    }

    public bool[] SkipDays
    {
      get
      {
        return this.skipDays;
      }
      set
      {
        this.skipDays = value;
      }
    }

    public bool[] SkipHours
    {
      get
      {
        return this.skipHours;
      }
      set
      {
        this.skipHours = value;
      }
    }

    public RssCloud Cloud
    {
      get
      {
        return this.cloud;
      }
      set
      {
        this.cloud = value;
      }
    }

    public int TimeToLive
    {
      get
      {
        return this.timeToLive;
      }
      set
      {
        this.timeToLive = RssDefault.Check(value);
      }
    }

    public RssItemCollection Items
    {
      get
      {
        return this.items;
      }
    }

    public override string ToString()
    {
      if (this.title != null)
        return this.title;
      if (this.description != null)
        return this.description;
      return "RssChannel";
    }
  }
}
