
// Type: ICSharpCode.SharpZipLib.Zip.ZipNameTransform


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class ZipNameTransform : INameTransform
  {
    private string trimPrefix_;
    private static readonly char[] InvalidEntryChars;
    private static readonly char[] InvalidEntryCharsRelaxed;

    public string TrimPrefix
    {
      get
      {
        return this.trimPrefix_;
      }
      set
      {
        this.trimPrefix_ = value;
        if (this.trimPrefix_ == null)
          return;
        this.trimPrefix_ = this.trimPrefix_.ToLower();
      }
    }

    static ZipNameTransform()
    {
      char[] invalidPathChars = Path.GetInvalidPathChars();
      int length1 = invalidPathChars.Length + 2;
      ZipNameTransform.InvalidEntryCharsRelaxed = new char[length1];
      Array.Copy((Array) invalidPathChars, 0, (Array) ZipNameTransform.InvalidEntryCharsRelaxed, 0, invalidPathChars.Length);
      ZipNameTransform.InvalidEntryCharsRelaxed[length1 - 1] = '*';
      ZipNameTransform.InvalidEntryCharsRelaxed[length1 - 2] = '?';
      int length2 = invalidPathChars.Length + 4;
      ZipNameTransform.InvalidEntryChars = new char[length2];
      Array.Copy((Array) invalidPathChars, 0, (Array) ZipNameTransform.InvalidEntryChars, 0, invalidPathChars.Length);
      ZipNameTransform.InvalidEntryChars[length2 - 1] = ':';
      ZipNameTransform.InvalidEntryChars[length2 - 2] = '\\';
      ZipNameTransform.InvalidEntryChars[length2 - 3] = '*';
      ZipNameTransform.InvalidEntryChars[length2 - 4] = '?';
    }

    public ZipNameTransform()
    {
    }

    public ZipNameTransform(string trimPrefix)
    {
      this.TrimPrefix = trimPrefix;
    }

    public string TransformDirectory(string name)
    {
      name = this.TransformFile(name);
      if (name.Length <= 0)
        throw new ZipException("Cannot have an empty directory name");
      if (!name.EndsWith("/"))
        name += "/";
      return name;
    }

    public string TransformFile(string name)
    {
      if (name != null)
      {
        if (this.trimPrefix_ != null && name.ToLower().IndexOf(this.trimPrefix_) == 0)
          name = name.Substring(this.trimPrefix_.Length);
        name = name.Replace("\\", "/");
        name = WindowsPathUtils.DropPathRoot(name);
        while (name.Length > 0 && (int) name[0] == 47)
          name = name.Remove(0, 1);
        while (name.Length > 0 && (int) name[name.Length - 1] == 47)
          name = name.Remove(name.Length - 1, 1);
        for (int startIndex = name.IndexOf("//"); startIndex >= 0; startIndex = name.IndexOf("//"))
          name = name.Remove(startIndex, 1);
        name = ZipNameTransform.MakeValidName(name, '_');
      }
      else
        name = string.Empty;
      return name;
    }

    private static string MakeValidName(string name, char replacement)
    {
      int index = name.IndexOfAny(ZipNameTransform.InvalidEntryChars);
      if (index >= 0)
      {
        StringBuilder stringBuilder = new StringBuilder(name);
        for (; index >= 0; index = index < name.Length ? name.IndexOfAny(ZipNameTransform.InvalidEntryChars, index + 1) : -1)
          stringBuilder[index] = replacement;
        name = stringBuilder.ToString();
      }
      if (name.Length > (int) ushort.MaxValue)
        throw new PathTooLongException();
      return name;
    }

    public static bool IsValidName(string name, bool relaxed)
    {
      bool flag = name != null;
      if (flag)
        flag = !relaxed ? name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0 : name.IndexOfAny(ZipNameTransform.InvalidEntryCharsRelaxed) < 0;
      return flag;
    }

    public static bool IsValidName(string name)
    {
      return name != null && name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0;
    }
  }
}
