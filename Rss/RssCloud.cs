
// Type: Rss.RssCloud


// Hacked by SystemAce

namespace Rss
{
  [Serializable]
  public class RssCloud : RssElement
  {
    private string domain = "";
    private string path = "";
    private string registerProcedure = "";
    private int port = -1;
    private RssCloudProtocol protocol;

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

    public int Port
    {
      get
      {
        return this.port;
      }
      set
      {
        this.port = RssDefault.Check(value);
      }
    }

    public string Path
    {
      get
      {
        return this.path;
      }
      set
      {
        this.path = RssDefault.Check(value);
      }
    }

    public string RegisterProcedure
    {
      get
      {
        return this.registerProcedure;
      }
      set
      {
        this.registerProcedure = RssDefault.Check(value);
      }
    }

    public RssCloudProtocol Protocol
    {
      get
      {
        return this.protocol;
      }
      set
      {
        this.protocol = value;
      }
    }
  }
}
