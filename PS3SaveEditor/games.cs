
// Type: PS3SaveEditor.games


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("games", Namespace = "")]
  public class games
  {
    [XmlElement("regions")]
    public regions regions;
    [XmlElement("game")]
    public List<game> _games;
    [XmlElement("saves")]
    public saves _saves;

    [XmlElement("rblist")]
    public rblsit rblist { get; set; }
  }
}
