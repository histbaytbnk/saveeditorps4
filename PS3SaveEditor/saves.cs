
// Type: PS3SaveEditor.saves


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("saves")]
  public class saves
  {
    [XmlElement("save")]
    public List<save> _saves { get; set; }
  }
}
