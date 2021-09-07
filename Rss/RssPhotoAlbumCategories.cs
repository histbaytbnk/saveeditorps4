
// Type: Rss.RssPhotoAlbumCategories


// Hacked by SystemAce

namespace Rss
{
  public sealed class RssPhotoAlbumCategories : RssModuleItemCollectionCollection
  {
    public int Add(RssPhotoAlbumCategory category)
    {
      return this.Add((RssModuleItemCollection) category);
    }
  }
}
