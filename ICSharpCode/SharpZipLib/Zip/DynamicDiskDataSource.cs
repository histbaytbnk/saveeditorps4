
// Type: ICSharpCode.SharpZipLib.Zip.DynamicDiskDataSource


// Hacked by SystemAce

using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class DynamicDiskDataSource : IDynamicDataSource
  {
    public Stream GetSource(ZipEntry entry, string name)
    {
      Stream stream = (Stream) null;
      if (name != null)
        stream = (Stream) File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
      return stream;
    }
  }
}
