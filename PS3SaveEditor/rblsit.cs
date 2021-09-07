
// Type: PS3SaveEditor.rblsit


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("rblist", Namespace = "")]
  public class rblsit
  {
    [XmlElement("game")]
    public List<rbgame> _rbgames;
  }
}
