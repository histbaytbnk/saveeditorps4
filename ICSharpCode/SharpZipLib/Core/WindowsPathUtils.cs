
// Type: ICSharpCode.SharpZipLib.Core.WindowsPathUtils


// Hacked by SystemAce

namespace ICSharpCode.SharpZipLib.Core
{
  public abstract class WindowsPathUtils
  {
    internal WindowsPathUtils()
    {
    }

    public static string DropPathRoot(string path)
    {
      string str = path;
      if (path != null && path.Length > 0)
      {
        if ((int) path[0] == 92 || (int) path[0] == 47)
        {
          if (path.Length > 1 && ((int) path[1] == 92 || (int) path[1] == 47))
          {
            int index = 2;
            int num = 2;
            while (index <= path.Length && ((int) path[index] != 92 && (int) path[index] != 47 || --num > 0))
              ++index;
            int startIndex = index + 1;
            str = startIndex >= path.Length ? "" : path.Substring(startIndex);
          }
        }
        else if (path.Length > 1 && (int) path[1] == 58)
        {
          int count = 2;
          if (path.Length > 2 && ((int) path[2] == 92 || (int) path[2] == 47))
            count = 3;
          str = str.Remove(0, count);
        }
      }
      return str;
    }
  }
}
