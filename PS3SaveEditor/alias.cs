
// Type: PS3SaveEditor.alias


// Hacked by SystemAce

namespace PS3SaveEditor
{
  public class alias
  {
    public string id;
    public string name;
    public int acts;
    public int region;

    public static alias Copy(alias alias)
    {
      return new alias()
      {
        id = alias.id,
        region = alias.region,
        name = alias.name
      };
    }
  }
}
