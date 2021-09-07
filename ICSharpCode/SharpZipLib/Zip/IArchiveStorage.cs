
// Type: ICSharpCode.SharpZipLib.Zip.IArchiveStorage


// Hacked by SystemAce

using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public interface IArchiveStorage
  {
    FileUpdateMode UpdateMode { get; }

    Stream GetTemporaryOutput();

    Stream ConvertTemporaryToFinal();

    Stream MakeTemporaryCopy(Stream stream);

    Stream OpenForDirectUpdate(Stream stream);

    void Dispose();
  }
}
