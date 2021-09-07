
// Type: ICSharpCode.SharpZipLib.Core.PathFilter


// Hacked by SystemAce

using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
  public class PathFilter : IScanFilter
  {
    private NameFilter nameFilter_;

    public PathFilter(string filter)
    {
      this.nameFilter_ = new NameFilter(filter);
    }

    public virtual bool IsMatch(string name)
    {
      bool flag = false;
      if (name != null)
        flag = this.nameFilter_.IsMatch(name.Length > 0 ? Path.GetFullPath(name) : "");
      return flag;
    }
  }
}
