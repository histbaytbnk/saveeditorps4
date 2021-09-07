
// Type: ICSharpCode.SharpZipLib.Core.INameTransform


// Hacked by SystemAce

namespace ICSharpCode.SharpZipLib.Core
{
  public interface INameTransform
  {
    string TransformFile(string name);

    string TransformDirectory(string name);
  }
}
