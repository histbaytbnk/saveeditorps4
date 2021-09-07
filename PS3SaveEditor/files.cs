
// Type: PS3SaveEditor.files


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("files")]
  public class files
  {
    [XmlElement("file")]
    public List<file> _files { get; set; }

    public files()
    {
      this._files = new List<file>();
    }
  }
}
