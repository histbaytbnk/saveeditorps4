
// Type: Rss.RssDefault


// Hacked by SystemAce

using System;

namespace Rss
{
  public class RssDefault
  {
    public static readonly DateTime DateTime = DateTime.MinValue;
    public static readonly Uri Uri = (Uri) null;
    public const string String = "";
    public const int Int = -1;

    public static string Check(string input)
    {
      if (input != null)
        return input;
      return "";
    }

    public static int Check(int input)
    {
      if (input >= -1)
        return input;
      return -1;
    }

    public static Uri Check(Uri input)
    {
      if (!(input == (Uri) null))
        return input;
      return RssDefault.Uri;
    }
  }
}
