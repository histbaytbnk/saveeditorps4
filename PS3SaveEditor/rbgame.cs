
// Type: PS3SaveEditor.rbgame


// Hacked by SystemAce

using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class rbgame
  {
    public string gamecode { get; set; }

    [XmlElement("containers")]
    public rbcontainers containers { get; set; }
  }
}
