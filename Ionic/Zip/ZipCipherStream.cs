
// Type: Ionic.Zip.ZipCipherStream


// Hacked by SystemAce

using System;
using System.IO;

namespace Ionic.Zip
{
  internal class ZipCipherStream : Stream
  {
    private ZipCrypto _cipher;
    private Stream _s;
    private CryptoMode _mode;

    public override bool CanRead
    {
      get
      {
        return this._mode == CryptoMode.Decrypt;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return this._mode == CryptoMode.Encrypt;
      }
    }

    public override long Length
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public ZipCipherStream(Stream s, ZipCrypto cipher, CryptoMode mode)
    {
      this._cipher = cipher;
      this._s = s;
      this._mode = mode;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this._mode == CryptoMode.Encrypt)
        throw new NotSupportedException("This stream does not encrypt via Read()");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      byte[] numArray1 = new byte[count];
      int length = this._s.Read(numArray1, 0, count);
      byte[] numArray2 = this._cipher.DecryptMessage(numArray1, length);
      for (int index = 0; index < length; ++index)
        buffer[offset + index] = numArray2[index];
      return length;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this._mode == CryptoMode.Decrypt)
        throw new NotSupportedException("This stream does not Decrypt via Write()");
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      if (count == 0)
        return;
      byte[] plainText;
      if (offset != 0)
      {
        plainText = new byte[count];
        for (int index = 0; index < count; ++index)
          plainText[index] = buffer[offset + index];
      }
      else
        plainText = buffer;
      byte[] buffer1 = this._cipher.EncryptMessage(plainText, count);
      this._s.Write(buffer1, 0, buffer1.Length);
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }
  }
}
