
// Type: ICSharpCode.SharpZipLib.Encryption.PkzipClassicCryptoBase


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Checksums;
using System;

namespace ICSharpCode.SharpZipLib.Encryption
{
  internal class PkzipClassicCryptoBase
  {
    private uint[] keys;

    protected byte TransformByte()
    {
      uint num = (uint) ((int) this.keys[2] & (int) ushort.MaxValue | 2);
      return (byte) (num * (num ^ 1U) >> 8);
    }

    protected void SetKeys(byte[] keyData)
    {
      if (keyData == null)
        throw new ArgumentNullException("keyData");
      if (keyData.Length != 12)
        throw new InvalidOperationException("Key length is not valid");
      this.keys = new uint[3];
      this.keys[0] = (uint) ((int) keyData[3] << 24 | (int) keyData[2] << 16 | (int) keyData[1] << 8) | (uint) keyData[0];
      this.keys[1] = (uint) ((int) keyData[7] << 24 | (int) keyData[6] << 16 | (int) keyData[5] << 8) | (uint) keyData[4];
      this.keys[2] = (uint) ((int) keyData[11] << 24 | (int) keyData[10] << 16 | (int) keyData[9] << 8) | (uint) keyData[8];
    }

    protected void UpdateKeys(byte ch)
    {
      this.keys[1] = this.keys[1] + (uint) (byte) this.keys[0];
      this.keys[1] = (uint) ((int) this.keys[1] * 134775813 + 1);
    }

    protected void Reset()
    {
      this.keys[0] = 0U;
      this.keys[1] = 0U;
      this.keys[2] = 0U;
    }
  }
}
