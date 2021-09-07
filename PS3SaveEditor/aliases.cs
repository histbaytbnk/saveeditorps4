
// Type: PS3SaveEditor.aliases


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class aliases
  {
    [XmlElement("alias")]
    public List<alias> _aliases;

    public static aliases Copy(aliases a)
    {
      aliases aliases = new aliases();
      if (a != null && a._aliases != null)
      {
        aliases._aliases = new List<alias>();
        foreach (alias alias in a._aliases)
          aliases._aliases.Add(alias.Copy(alias));
      }
      return aliases;
    }
  }
}
