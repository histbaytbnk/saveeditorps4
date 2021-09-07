
// Type: ICSharpCode.SharpZipLib.Zip.WindowsNameTransform


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Core;
using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class WindowsNameTransform : INameTransform
  {
    private char _replacementChar = '_';
    private const int MaxPath = 260;
    private string _baseDirectory;
    private bool _trimIncomingPaths;
    private static readonly char[] InvalidEntryChars;

    public string BaseDirectory
    {
      get
      {
        return this._baseDirectory;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this._baseDirectory = Path.GetFullPath(value);
      }
    }

    public bool TrimIncomingPaths
    {
      get
      {
        return this._trimIncomingPaths;
      }
      set
      {
        this._trimIncomingPaths = value;
      }
    }

    public char Replacement
    {
      get
      {
        return this._replacementChar;
      }
      set
      {
        for (int index = 0; index < WindowsNameTransform.InvalidEntryChars.Length; ++index)
        {
          if ((int) WindowsNameTransform.InvalidEntryChars[index] == (int) value)
            throw new ArgumentException("invalid path character");
        }
        if ((int) value == 92 || (int) value == 47)
          throw new ArgumentException("invalid replacement character");
        this._replacementChar = value;
      }
    }

    static WindowsNameTransform()
    {
      char[] invalidPathChars = Path.GetInvalidPathChars();
      int length = invalidPathChars.Length + 3;
      WindowsNameTransform.InvalidEntryChars = new char[length];
      Array.Copy((Array) invalidPathChars, 0, (Array) WindowsNameTransform.InvalidEntryChars, 0, invalidPathChars.Length);
      WindowsNameTransform.InvalidEntryChars[length - 1] = '*';
      WindowsNameTransform.InvalidEntryChars[length - 2] = '?';
      WindowsNameTransform.InvalidEntryChars[length - 3] = ':';
    }

    public WindowsNameTransform(string baseDirectory)
    {
      if (baseDirectory == null)
        throw new ArgumentNullException("baseDirectory", "Directory name is invalid");
      this.BaseDirectory = baseDirectory;
    }

    public WindowsNameTransform()
    {
    }

    public string TransformDirectory(string name)
    {
      name = this.TransformFile(name);
      if (name.Length <= 0)
        throw new ZipException("Cannot have an empty directory name");
      while (name.EndsWith("\\"))
        name = name.Remove(name.Length - 1, 1);
      return name;
    }

    public string TransformFile(string name)
    {
      if (name != null)
      {
        name = WindowsNameTransform.MakeValidName(name, this._replacementChar);
        if (this._trimIncomingPaths)
          name = Path.GetFileName(name);
        if (this._baseDirectory != null)
          name = Path.Combine(this._baseDirectory, name);
      }
      else
        name = string.Empty;
      return name;
    }

    public static bool IsValidName(string name)
    {
      return name != null && name.Length <= 260 && string.Compare(name, WindowsNameTransform.MakeValidName(name, '_')) == 0;
    }

    public static string MakeValidName(string name, char replacement)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      name = WindowsPathUtils.DropPathRoot(name.Replace("/", "\\"));
      while (name.Length > 0 && (int) name[0] == 92)
        name = name.Remove(0, 1);
      while (name.Length > 0 && (int) name[name.Length - 1] == 92)
        name = name.Remove(name.Length - 1, 1);
      for (int startIndex = name.IndexOf("\\\\"); startIndex >= 0; startIndex = name.IndexOf("\\\\"))
        name = name.Remove(startIndex, 1);
      int index = name.IndexOfAny(WindowsNameTransform.InvalidEntryChars);
      if (index >= 0)
      {
        StringBuilder stringBuilder = new StringBuilder(name);
        for (; index >= 0; index = index < name.Length ? name.IndexOfAny(WindowsNameTransform.InvalidEntryChars, index + 1) : -1)
          stringBuilder[index] = replacement;
        name = stringBuilder.ToString();
      }
      if (name.Length > 260)
        throw new PathTooLongException();
      return name;
    }
  }
}
