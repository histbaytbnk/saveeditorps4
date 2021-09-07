
// Type: ICSharpCode.SharpZipLib.Zip.ITaggedDataFactory


// Hacked by SystemAce

namespace ICSharpCode.SharpZipLib.Zip
{
  internal interface ITaggedDataFactory
  {
    ITaggedData Create(short tag, byte[] data, int offset, int count);
  }
}
