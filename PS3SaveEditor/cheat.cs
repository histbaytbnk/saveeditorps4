
// Type: PS3SaveEditor.cheat


// Hacked by SystemAce

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("cheat")]
  public class cheat
  {
    public string id { get; set; }

    public string name { get; set; }

    public string note { get; set; }

    public bool Selected { get; set; }

    public string code { get; set; }

    public cheat()
    {
    }

    public cheat(string id, string description, string comment)
    {
      this.id = id;
      this.name = description;
      this.note = comment;
    }

    public string ToEditableString()
    {
      string str = "";
      string[] strArray = this.code.Split(' ');
      int index = 0;
      while (index < strArray.Length)
      {
        str = str + strArray[index] + " " + strArray[index + 1] + "\n";
        index += 2;
      }
      return str;
    }

    public string ToString(bool _protected = false)
    {
      string str = "";
      if (this.Selected)
      {
        if (_protected)
        {
          if (!string.IsNullOrEmpty(this.id))
            str += string.Format("<id>{0}</id>", (object) this.id);
        }
        else
        {
          if (this.code != null)
            return string.Format("<code>{0}</code>", (object) this.code);
          return string.Format("<id>{0}</id>", (object) this.id);
        }
      }
      return str;
    }

    internal static cheat Copy(cheat cheat)
    {
      return new cheat()
      {
        id = cheat.id,
        name = cheat.name,
        note = cheat.note,
        code = cheat.code
      };
    }

    internal static byte GetBitCodeBytes(int bitCode)
    {
      switch (bitCode)
      {
        case 0:
          return (byte) 1;
        case 1:
          return (byte) 2;
        case 2:
          return (byte) 4;
        default:
          return (byte) 4;
      }
    }

    internal static long GetMemLocation(string value, out int bitWriteCode)
    {
      long result;
      long.TryParse(value, NumberStyles.HexNumber, (IFormatProvider) null, out result);
      long num = result & 268435455L;
      bitWriteCode = (int) (result >> 28);
      return num;
    }
  }
}
