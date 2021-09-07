
// Type: Be.Windows.Forms.Util


// Hacked by SystemAce

using System.Diagnostics;

namespace Be.Windows.Forms
{
  internal static class Util
  {
    private static bool _designMode = Process.GetCurrentProcess().ProcessName.ToLower() == "devenv";

    public static bool DesignMode
    {
      get
      {
        return Util._designMode;
      }
    }
  }
}
