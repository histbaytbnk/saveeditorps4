
// Type: PS3SaveEditor.containers


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class containers
  {
    [XmlElement("container")]
    public List<container> _containers;

    public containers()
    {
      this._containers = new List<container>();
    }
  }
}
