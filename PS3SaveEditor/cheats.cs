
// Type: PS3SaveEditor.cheats


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("cheats")]
  public class cheats
  {
    [XmlElement("cheat")]
    public List<cheat> _cheats { get; set; }

    [XmlElement("group")]
    public List<group> groups { get; set; }

    public int TotalCheats
    {
      get
      {
        int count = this._cheats.Count;
        foreach (group group in this.groups)
          count += group.TotalCheats;
        return count;
      }
    }

    public cheats()
    {
      this._cheats = new List<cheat>();
      this.groups = new List<group>();
    }
  }
}
