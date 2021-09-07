
// Type: Rss.RssModuleItem


// Hacked by SystemAce

namespace Rss
{
  [Serializable]
  public class RssModuleItem : RssElement
  {
    private string _sElementName = "";
    private string _sElementText = "";
    private RssModuleItemCollection _rssSubElements = new RssModuleItemCollection();
    private bool _bRequired;

    public string Name
    {
      get
      {
        return this._sElementName;
      }
      set
      {
        this._sElementName = RssDefault.Check(value);
      }
    }

    public string Text
    {
      get
      {
        return this._sElementText;
      }
      set
      {
        this._sElementText = RssDefault.Check(value);
      }
    }

    public RssModuleItemCollection SubElements
    {
      get
      {
        return this._rssSubElements;
      }
      set
      {
        this._rssSubElements = value;
      }
    }

    public bool IsRequired
    {
      get
      {
        return this._bRequired;
      }
    }

    public RssModuleItem()
    {
    }

    public RssModuleItem(string name)
    {
      this._sElementName = RssDefault.Check(name);
    }

    public RssModuleItem(string name, bool required)
      : this(name)
    {
      this._bRequired = required;
    }

    public RssModuleItem(string name, string text)
      : this(name)
    {
      this._sElementText = RssDefault.Check(text);
    }

    public RssModuleItem(string name, bool required, string text)
      : this(name, required)
    {
      this._sElementText = RssDefault.Check(text);
    }

    public RssModuleItem(string name, string text, RssModuleItemCollection subElements)
      : this(name, text)
    {
      this._rssSubElements = subElements;
    }

    public RssModuleItem(string name, bool required, string text, RssModuleItemCollection subElements)
      : this(name, required, text)
    {
      this._rssSubElements = subElements;
    }

    public override string ToString()
    {
      if (this._sElementName != null)
        return this._sElementName;
      if (this._sElementText != null)
        return this._sElementText;
      return "RssModuleItem";
    }
  }
}
