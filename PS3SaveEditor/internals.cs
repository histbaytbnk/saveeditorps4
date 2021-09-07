
// Type: PS3SaveEditor.internals


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class internals
  {
    [XmlElement("file")]
    public List<file> files { get; set; }

    public internals()
    {
      this.files = new List<file>();
    }

    public static internals Copy(internals i)
    {
      internals internals = new internals();
      foreach (file gameFile in i.files)
        internals.files.Add(file.Copy(gameFile));
      return internals;
    }
  }
}
