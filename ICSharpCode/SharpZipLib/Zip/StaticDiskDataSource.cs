
// Type: ICSharpCode.SharpZipLib.Zip.StaticDiskDataSource


// Hacked by SystemAce

using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class StaticDiskDataSource : IStaticDataSource
  {
    private string fileName_;

    public StaticDiskDataSource(string fileName)
    {
      this.fileName_ = fileName;
    }

    public Stream GetSource()
    {
      return (Stream) File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
  }
}
