
// Type: ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged


// Hacked by SystemAce

using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
  public sealed class PkzipClassicManaged : PkzipClassic
  {
    private byte[] key_;

    public override int BlockSize
    {
      get
      {
        return 8;
      }
      set
      {
        if (value != 8)
          throw new CryptographicException("Block size is invalid");
      }
    }

    public override KeySizes[] LegalKeySizes
    {
      get
      {
        return new KeySizes[1]
        {
          new KeySizes(96, 96, 0)
        };
      }
    }

    public override KeySizes[] LegalBlockSizes
    {
      get
      {
        return new KeySizes[1]
        {
          new KeySizes(8, 8, 0)
        };
      }
    }

    public override byte[] Key
    {
      get
      {
        if (this.key_ == null)
          this.GenerateKey();
        return (byte[]) this.key_.Clone();
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        if (value.Length != 12)
          throw new CryptographicException("Key size is illegal");
        this.key_ = (byte[]) value.Clone();
      }
    }

    public override void GenerateIV()
    {
    }

    public override void GenerateKey()
    {
      this.key_ = new byte[12];
      new Random().NextBytes(this.key_);
    }

    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
    {
      this.key_ = rgbKey;
      return (ICryptoTransform) new PkzipClassicEncryptCryptoTransform(this.Key);
    }

    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
    {
      this.key_ = rgbKey;
      return (ICryptoTransform) new PkzipClassicDecryptCryptoTransform(this.Key);
    }
  }
}
