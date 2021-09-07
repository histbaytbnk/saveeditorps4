
// Type: Ionic.Zip.ZipCrypto


// Hacked by SystemAce

using Ionic.Crc;
using System;
using System.IO;

namespace Ionic.Zip
{
  internal class ZipCrypto
  {
    private uint[] _Keys = new uint[3]
    {
      305419896U,
      591751049U,
      878082192U
    };
    private CRC32 crc32 = new CRC32();

    private byte MagicByte
    {
      get
      {
        ushort num = (ushort) ((uint) (ushort) (this._Keys[2] & (uint) ushort.MaxValue) | 2U);
        return (byte) ((int) num * ((int) num ^ 1) >> 8);
      }
    }

    private ZipCrypto()
    {
    }

    public static ZipCrypto ForWrite(string password)
    {
      ZipCrypto zipCrypto = new ZipCrypto();
      if (password == null)
        throw new BadPasswordException("This entry requires a password.");
      zipCrypto.InitCipher(password);
      return zipCrypto;
    }

    public static ZipCrypto ForRead(string password, ZipEntry e)
    {
      Stream s = e._archiveStream;
      e._WeakEncryptionHeader = new byte[12];
      byte[] numArray1 = e._WeakEncryptionHeader;
      ZipCrypto zipCrypto = new ZipCrypto();
      if (password == null)
        throw new BadPasswordException("This entry requires a password.");
      zipCrypto.InitCipher(password);
      ZipEntry.ReadWeakEncryptionHeader(s, numArray1);
      byte[] numArray2 = zipCrypto.DecryptMessage(numArray1, numArray1.Length);
      if ((int) numArray2[11] != (int) (byte) (e._Crc32 >> 24 & (int) byte.MaxValue))
      {
        if (((int) e._BitField & 8) != 8)
          throw new BadPasswordException("The password did not match.");
        if ((int) numArray2[11] != (int) (byte) (e._TimeBlob >> 8 & (int) byte.MaxValue))
          throw new BadPasswordException("The password did not match.");
      }
      return zipCrypto;
    }

    public byte[] DecryptMessage(byte[] cipherText, int length)
    {
      if (cipherText == null)
        throw new ArgumentNullException("cipherText");
      if (length > cipherText.Length)
        throw new ArgumentOutOfRangeException("length", "Bad length during Decryption: the length parameter must be smaller than or equal to the size of the destination array.");
      byte[] numArray = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        byte byteValue = (byte) ((uint) cipherText[index] ^ (uint) this.MagicByte);
        this.UpdateKeys(byteValue);
        numArray[index] = byteValue;
      }
      return numArray;
    }

    public byte[] EncryptMessage(byte[] plainText, int length)
    {
      if (plainText == null)
        throw new ArgumentNullException("plaintext");
      if (length > plainText.Length)
        throw new ArgumentOutOfRangeException("length", "Bad length during Encryption: The length parameter must be smaller than or equal to the size of the destination array.");
      byte[] numArray = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        byte byteValue = plainText[index];
        numArray[index] = (byte) ((uint) plainText[index] ^ (uint) this.MagicByte);
        this.UpdateKeys(byteValue);
      }
      return numArray;
    }

    public void InitCipher(string passphrase)
    {
      byte[] numArray = SharedUtilities.StringToByteArray(passphrase);
      for (int index = 0; index < passphrase.Length; ++index)
        this.UpdateKeys(numArray[index]);
    }

    private void UpdateKeys(byte byteValue)
    {
      this._Keys[1] = this._Keys[1] + (uint) (byte) this._Keys[0];
      this._Keys[1] = (uint) ((int) this._Keys[1] * 134775813 + 1);
    }
  }
}
