
// Type: ICSharpCode.SharpZipLib.Zip.BaseArchiveStorage


// Hacked by SystemAce

using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public abstract class BaseArchiveStorage : IArchiveStorage
  {
    private FileUpdateMode updateMode_;

    public FileUpdateMode UpdateMode
    {
      get
      {
        return this.updateMode_;
      }
    }

    protected BaseArchiveStorage(FileUpdateMode updateMode)
    {
      this.updateMode_ = updateMode;
    }

    public abstract Stream GetTemporaryOutput();

    public abstract Stream ConvertTemporaryToFinal();

    public abstract Stream MakeTemporaryCopy(Stream stream);

    public abstract Stream OpenForDirectUpdate(Stream stream);

    public abstract void Dispose();
  }
}
