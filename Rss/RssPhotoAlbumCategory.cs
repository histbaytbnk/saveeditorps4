
// Type: Rss.RssPhotoAlbumCategory


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public sealed class RssPhotoAlbumCategory : RssModuleItemCollection
  {
    public RssPhotoAlbumCategory(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
    {
      this.Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhoto);
    }

    public RssPhotoAlbumCategory(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
    {
      this.Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhoto);
    }

    public RssPhotoAlbumCategory(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
    {
      this.Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhotos);
    }

    public RssPhotoAlbumCategory(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
    {
      this.Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhotos);
    }

    private int Add(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
    {
      RssModuleItemCollection subElements = new RssModuleItemCollection();
      subElements.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom.ToUniversalTime().ToString("r"))));
      subElements.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo.ToUniversalTime().ToString("r"))));
      this.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
      this.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
      this.Add(new RssModuleItem("categoryDateRange", true, "", subElements));
      this.Add(new RssModuleItem("categoryPhoto", true, "", (RssModuleItemCollection) categoryPhoto));
      return -1;
    }

    private int Add(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
    {
      RssModuleItemCollection subElements = new RssModuleItemCollection();
      subElements.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom)));
      subElements.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo)));
      this.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
      this.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
      this.Add(new RssModuleItem("categoryDateRange", true, "", subElements));
      this.Add(new RssModuleItem("categoryPhoto", true, "", (RssModuleItemCollection) categoryPhoto));
      return -1;
    }

    private int Add(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
    {
      RssModuleItemCollection subElements1 = new RssModuleItemCollection();
      subElements1.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom.ToUniversalTime().ToString("r"))));
      subElements1.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo.ToUniversalTime().ToString("r"))));
      this.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
      this.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
      this.Add(new RssModuleItem("categoryDateRange", true, "", subElements1));
      foreach (RssModuleItemCollection subElements2 in (CollectionBase) categoryPhotos)
        this.Add(new RssModuleItem("categoryPhoto", true, "", subElements2));
      return -1;
    }

    private int Add(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
    {
      RssModuleItemCollection subElements1 = new RssModuleItemCollection();
      subElements1.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom)));
      subElements1.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo)));
      this.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
      this.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
      this.Add(new RssModuleItem("categoryDateRange", true, "", subElements1));
      foreach (RssModuleItemCollection subElements2 in (CollectionBase) categoryPhotos)
        this.Add(new RssModuleItem("categoryPhoto", true, "", subElements2));
      return -1;
    }
  }
}
