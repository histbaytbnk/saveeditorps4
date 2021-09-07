
// Type: Rss.RssGuid


// Hacked by SystemAce

namespace Rss
{
  [Serializable]
  public class RssGuid : RssElement
  {
    private DBBool permaLink = DBBool.Null;
    private string name = "";

    public DBBool PermaLink
    {
      get
      {
        return this.permaLink;
      }
      set
      {
        this.permaLink = value;
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
  }
}
