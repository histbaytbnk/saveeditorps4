﻿
// Type: Be.Windows.Forms.EbcdicByteCharProvider


// Hacked by SystemAce

using System.Text;

namespace Be.Windows.Forms
{
  public class EbcdicByteCharProvider : IByteCharConverter
  {
    private Encoding _ebcdicEncoding = Encoding.GetEncoding(500);

    public char ToChar(byte b)
    {
      string @string = this._ebcdicEncoding.GetString(new byte[1]
      {
        b
      });
      if (@string.Length <= 0)
        return '.';
      return @string[0];
    }

    public byte ToByte(char c)
    {
      byte[] bytes = this._ebcdicEncoding.GetBytes(new char[1]
      {
        c
      });
      if (bytes.Length <= 0)
        return (byte) 0;
      return bytes[0];
    }

    public override string ToString()
    {
      return "EBCDIC (Code Page 500)";
    }
  }
}
