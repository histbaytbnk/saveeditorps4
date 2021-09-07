
// Type: PS3SaveEditor.save


// Hacked by SystemAce

namespace PS3SaveEditor
{
  public class save
  {
    public string id { get; set; }

    public string gamecode { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public string note { get; set; }

    public string folder { get; set; }

    public string region { get; set; }

    public long updated { get; set; }

    internal static save Copy(save save)
    {
      return new save()
      {
        folder = save.folder,
        region = save.region,
        updated = save.updated,
        description = save.description,
        gamecode = save.gamecode,
        note = save.note,
        title = save.title,
        id = save.id
      };
    }
  }
}
