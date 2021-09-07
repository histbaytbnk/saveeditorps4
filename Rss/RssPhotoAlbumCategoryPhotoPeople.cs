
// Type: Rss.RssPhotoAlbumCategoryPhotoPeople


// Hacked by SystemAce

namespace Rss
{
  public sealed class RssPhotoAlbumCategoryPhotoPeople : RssModuleItemCollection
  {
    public RssPhotoAlbumCategoryPhotoPeople()
    {
    }

    public RssPhotoAlbumCategoryPhotoPeople(string value)
    {
      this.Add(value);
    }

    public int Add(string value)
    {
      return this.Add(new RssModuleItem("person", true, value));
    }
  }
}
