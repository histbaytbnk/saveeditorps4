
// Type: PS3SaveEditor.regions


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class regions
  {
    [XmlElement("region")]
    public List<region> _regions;
  }
}
