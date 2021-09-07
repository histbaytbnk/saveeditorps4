
// Type: Rss.RssPhotoAlbumCategoryPhoto


// Hacked by SystemAce

using System;

namespace Rss
{
  public sealed class RssPhotoAlbumCategoryPhoto : RssModuleItemCollection
  {
    public RssPhotoAlbumCategoryPhoto(DateTime photoDate, string photoDescription, Uri photoLink)
    {
      this.Add(photoDate, photoDescription, photoLink);
    }

    public RssPhotoAlbumCategoryPhoto(DateTime photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
    {
      this.Add(photoDate, photoDescription, photoLink, photoPeople);
    }

    public RssPhotoAlbumCategoryPhoto(string photoDate, string photoDescription, Uri photoLink)
    {
      this.Add(photoDate, photoDescription, photoLink);
    }

    public RssPhotoAlbumCategoryPhoto(string photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
    {
      this.Add(photoDate, photoDescription, photoLink, photoPeople);
    }

    private int Add(DateTime photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
    {
      this.Add(photoDate, photoDescription, photoLink);
      this.Add(new RssModuleItem("photoPeople", true, "", (RssModuleItemCollection) photoPeople));
      return -1;
    }

    private int Add(DateTime photoDate, string photoDescription, Uri photoLink)
    {
      this.Add(new RssModuleItem("photoDate", true, RssDefault.Check(photoDate.ToUniversalTime().ToString("r"))));
      this.Add(new RssModuleItem("photoDescription", false, RssDefault.Check(photoDescription)));
      this.Add(new RssModuleItem("photoLink", true, RssDefault.Check(photoLink).ToString()));
      return -1;
    }

    private int Add(string photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
    {
      this.Add(photoDate, photoDescription, photoLink);
      this.Add(new RssModuleItem("photoPeople", true, "", (RssModuleItemCollection) photoPeople));
      return -1;
    }

    private int Add(string photoDate, string photoDescription, Uri photoLink)
    {
      this.Add(new RssModuleItem("photoDate", true, RssDefault.Check(photoDate)));
      this.Add(new RssModuleItem("photoDescription", false, RssDefault.Check(photoDescription)));
      this.Add(new RssModuleItem("photoLink", true, RssDefault.Check(photoLink).ToString()));
      return -1;
    }
  }
}
