
// Type: Rss.RssCategory


// Hacked by SystemAce

namespace Rss
{
  [Serializable]
  public class RssCategory : RssElement
  {
    private string name = "";
    private string domain = "";

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

    public string Domain
    {
      get
      {
        return this.domain;
      }
      set
      {
        this.domain = RssDefault.Check(value);
      }
    }
  }
}
