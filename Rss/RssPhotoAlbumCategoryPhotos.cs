
// Type: Rss.RssPhotoAlbumCategoryPhotos


// Hacked by SystemAce

namespace Rss
{
  public sealed class RssPhotoAlbumCategoryPhotos : RssModuleItemCollectionCollection
  {
    public int Add(RssPhotoAlbumCategoryPhoto photo)
    {
      return this.Add((RssModuleItemCollection) photo);
    }
  }
}
