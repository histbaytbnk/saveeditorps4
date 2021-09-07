
// Type: PS3SaveEditor.rbcontainers


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class rbcontainers
  {
    [XmlElement("container")]
    public List<string> container { get; set; }
  }
}
