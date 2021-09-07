
// Type: Be.Windows.Forms.DefaultByteCharConverter


// Hacked by SystemAce

namespace Be.Windows.Forms
{
  public class DefaultByteCharConverter : IByteCharConverter
  {
    public char ToChar(byte b)
    {
      if ((int) b <= 31 || (int) b > 126 && (int) b < 160)
        return '.';
      return (char) b;
    }

    public byte ToByte(char c)
    {
      return (byte) c;
    }

    public override string ToString()
    {
      return "Default";
    }
  }
}
