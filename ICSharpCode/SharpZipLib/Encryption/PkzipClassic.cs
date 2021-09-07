
// Type: ICSharpCode.SharpZipLib.Encryption.PkzipClassic


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Checksums;
using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
  public abstract class PkzipClassic : SymmetricAlgorithm
  {
    public static byte[] GenerateKeys(byte[] seed)
    {
      if (seed == null)
        throw new ArgumentNullException("seed");
      if (seed.Length == 0)
        throw new ArgumentException("Length is zero", "seed");
      uint[] numArray = new uint[3]
      {
        305419896U,
        591751049U,
        878082192U
      };
      for (int index = 0; index < seed.Length; ++index)
      {
        numArray[1] = numArray[1] + (uint) (byte) numArray[0];
        numArray[1] = (uint) ((int) numArray[1] * 134775813 + 1);
      }
      return new byte[12]
      {
        (byte) (numArray[0] & (uint) byte.MaxValue),
        (byte) (numArray[0] >> 8 & (uint) byte.MaxValue),
        (byte) (numArray[0] >> 16 & (uint) byte.MaxValue),
        (byte) (numArray[0] >> 24 & (uint) byte.MaxValue),
        (byte) (numArray[1] & (uint) byte.MaxValue),
        (byte) (numArray[1] >> 8 & (uint) byte.MaxValue),
        (byte) (numArray[1] >> 16 & (uint) byte.MaxValue),
        (byte) (numArray[1] >> 24 & (uint) byte.MaxValue),
        (byte) (numArray[2] & (uint) byte.MaxValue),
        (byte) (numArray[2] >> 8 & (uint) byte.MaxValue),
        (byte) (numArray[2] >> 16 & (uint) byte.MaxValue),
        (byte) (numArray[2] >> 24 & (uint) byte.MaxValue)
      };
    }
  }
}
