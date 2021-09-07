
// Type: ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputBuffer


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
  public class InflaterInputBuffer
  {
    private int rawLength;
    private byte[] rawData;
    private int clearTextLength;
    private byte[] clearText;
    private byte[] internalClearText;
    private int available;
    private ICryptoTransform cryptoTransform;
    private Stream inputStream;

    public int RawLength
    {
      get
      {
        return this.rawLength;
      }
    }

    public byte[] RawData
    {
      get
      {
        return this.rawData;
      }
    }

    public int ClearTextLength
    {
      get
      {
        return this.clearTextLength;
      }
    }

    public byte[] ClearText
    {
      get
      {
        return this.clearText;
      }
    }

    public int Available
    {
      get
      {
        return this.available;
      }
      set
      {
        this.available = value;
      }
    }

    public ICryptoTransform CryptoTransform
    {
      set
      {
        this.cryptoTransform = value;
        if (this.cryptoTransform != null)
        {
          if (this.rawData == this.clearText)
          {
            if (this.internalClearText == null)
              this.internalClearText = new byte[this.rawData.Length];
            this.clearText = this.internalClearText;
          }
          this.clearTextLength = this.rawLength;
          if (this.available <= 0)
            return;
          this.cryptoTransform.TransformBlock(this.rawData, this.rawLength - this.available, this.available, this.clearText, this.rawLength - this.available);
        }
        else
        {
          this.clearText = this.rawData;
          this.clearTextLength = this.rawLength;
        }
      }
    }

    public InflaterInputBuffer(Stream stream)
      : this(stream, 4096)
    {
    }

    public InflaterInputBuffer(Stream stream, int bufferSize)
    {
      this.inputStream = stream;
      if (bufferSize < 1024)
        bufferSize = 1024;
      this.rawData = new byte[bufferSize];
      this.clearText = this.rawData;
    }

    public void SetInflaterInput(Inflater inflater)
    {
      if (this.available <= 0)
        return;
      inflater.SetInput(this.clearText, this.clearTextLength - this.available, this.available);
      this.available = 0;
    }

    public void Fill()
    {
      this.rawLength = 0;
      int length = this.rawData.Length;
      while (length > 0)
      {
        int num = this.inputStream.Read(this.rawData, this.rawLength, length);
        if (num > 0)
        {
          this.rawLength += num;
          length -= num;
        }
        else
          break;
      }
      this.clearTextLength = this.cryptoTransform == null ? this.rawLength : this.cryptoTransform.TransformBlock(this.rawData, 0, this.rawLength, this.clearText, 0);
      this.available = this.clearTextLength;
    }

    public int ReadRawBuffer(byte[] buffer)
    {
      return this.ReadRawBuffer(buffer, 0, buffer.Length);
    }

    public int ReadRawBuffer(byte[] outBuffer, int offset, int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");
      int destinationIndex = offset;
      int val1 = length;
      while (val1 > 0)
      {
        if (this.available <= 0)
        {
          this.Fill();
          if (this.available <= 0)
            return 0;
        }
        int length1 = Math.Min(val1, this.available);
        Array.Copy((Array) this.rawData, this.rawLength - this.available, (Array) outBuffer, destinationIndex, length1);
        destinationIndex += length1;
        val1 -= length1;
        this.available -= length1;
      }
      return length;
    }

    public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");
      int destinationIndex = offset;
      int val1 = length;
      while (val1 > 0)
      {
        if (this.available <= 0)
        {
          this.Fill();
          if (this.available <= 0)
            return 0;
        }
        int length1 = Math.Min(val1, this.available);
        Array.Copy((Array) this.clearText, this.clearTextLength - this.available, (Array) outBuffer, destinationIndex, length1);
        destinationIndex += length1;
        val1 -= length1;
        this.available -= length1;
      }
      return length;
    }

    public int ReadLeByte()
    {
      if (this.available <= 0)
      {
        this.Fill();
        if (this.available <= 0)
          throw new ZipException("EOF in header");
      }
      byte num = this.rawData[this.rawLength - this.available];
      --this.available;
      return (int) num;
    }

    public int ReadLeShort()
    {
      return this.ReadLeByte() | this.ReadLeByte() << 8;
    }

    public int ReadLeInt()
    {
      return this.ReadLeShort() | this.ReadLeShort() << 16;
    }

    public long ReadLeLong()
    {
      return (long) (uint) this.ReadLeInt() | (long) this.ReadLeInt() << 32;
    }
  }
}
