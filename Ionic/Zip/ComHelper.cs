﻿
// Type: Ionic.Zip.ComHelper


// Hacked by SystemAce

using System.Runtime.InteropServices;

namespace Ionic.Zip
{
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000F")]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  [ComVisible(true)]
  public class ComHelper
  {
    public bool IsZipFile(string filename)
    {
      return ZipFile.IsZipFile(filename);
    }

    public bool IsZipFileWithExtract(string filename)
    {
      return ZipFile.IsZipFile(filename, true);
    }

    public bool CheckZip(string filename)
    {
      return ZipFile.CheckZip(filename);
    }

    public bool CheckZipPassword(string filename, string password)
    {
      return ZipFile.CheckZipPassword(filename, password);
    }

    public void FixZipDirectory(string filename)
    {
      ZipFile.FixZipDirectory(filename);
    }

    public string GetZipLibraryVersion()
    {
      return ZipFile.LibraryVersion.ToString();
    }
  }
}
