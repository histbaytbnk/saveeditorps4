
// Type: ICSharpCode.SharpZipLib.Zip.ITaggedData


// Hacked by SystemAce

namespace ICSharpCode.SharpZipLib.Zip
{
  public interface ITaggedData
  {
    short TagID { get; }

    void SetData(byte[] data, int offset, int count);

    byte[] GetData();
  }
}
