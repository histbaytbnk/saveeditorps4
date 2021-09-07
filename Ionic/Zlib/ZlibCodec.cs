﻿
// Type: Ionic.Zlib.ZlibCodec


// Hacked by SystemAce

using System;
using System.Runtime.InteropServices;

namespace Ionic.Zlib
{
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000D")]
  public sealed class ZlibCodec
  {
    public CompressionLevel CompressLevel = CompressionLevel.Default;
    public int WindowBits = 15;
    public byte[] InputBuffer;
    public int NextIn;
    public int AvailableBytesIn;
    public long TotalBytesIn;
    public byte[] OutputBuffer;
    public int NextOut;
    public int AvailableBytesOut;
    public long TotalBytesOut;
    public string Message;
    internal DeflateManager dstate;
    internal InflateManager istate;
    internal uint _Adler32;
    public CompressionStrategy Strategy;

    public int Adler32
    {
      get
      {
        return (int) this._Adler32;
      }
    }

    public ZlibCodec()
    {
    }

    public ZlibCodec(CompressionMode mode)
    {
      if (mode == CompressionMode.Compress)
      {
        if (this.InitializeDeflate() != 0)
          throw new ZlibException("Cannot initialize for deflate.");
      }
      else
      {
        if (mode != CompressionMode.Decompress)
          throw new ZlibException("Invalid ZlibStreamFlavor.");
        if (this.InitializeInflate() != 0)
          throw new ZlibException("Cannot initialize for inflate.");
      }
    }

    public int InitializeInflate()
    {
      return this.InitializeInflate(this.WindowBits);
    }

    public int InitializeInflate(bool expectRfc1950Header)
    {
      return this.InitializeInflate(this.WindowBits, expectRfc1950Header);
    }

    public int InitializeInflate(int windowBits)
    {
      this.WindowBits = windowBits;
      return this.InitializeInflate(windowBits, true);
    }

    public int InitializeInflate(int windowBits, bool expectRfc1950Header)
    {
      this.WindowBits = windowBits;
      if (this.dstate != null)
        throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
      this.istate = new InflateManager(expectRfc1950Header);
      return this.istate.Initialize(this, windowBits);
    }

    public int Inflate(FlushType flush)
    {
      if (this.istate == null)
        throw new ZlibException("No Inflate State!");
      return this.istate.Inflate(flush);
    }

    public int EndInflate()
    {
      if (this.istate == null)
        throw new ZlibException("No Inflate State!");
      int num = this.istate.End();
      this.istate = (InflateManager) null;
      return num;
    }

    public int SyncInflate()
    {
      if (this.istate == null)
        throw new ZlibException("No Inflate State!");
      return this.istate.Sync();
    }

    public int InitializeDeflate()
    {
      return this._InternalInitializeDeflate(true);
    }

    public int InitializeDeflate(CompressionLevel level)
    {
      this.CompressLevel = level;
      return this._InternalInitializeDeflate(true);
    }

    public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
    {
      this.CompressLevel = level;
      return this._InternalInitializeDeflate(wantRfc1950Header);
    }

    public int InitializeDeflate(CompressionLevel level, int bits)
    {
      this.CompressLevel = level;
      this.WindowBits = bits;
      return this._InternalInitializeDeflate(true);
    }

    public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
    {
      this.CompressLevel = level;
      this.WindowBits = bits;
      return this._InternalInitializeDeflate(wantRfc1950Header);
    }

    private int _InternalInitializeDeflate(bool wantRfc1950Header)
    {
      if (this.istate != null)
        throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
      this.dstate = new DeflateManager();
      this.dstate.WantRfc1950HeaderBytes = wantRfc1950Header;
      return this.dstate.Initialize(this, this.CompressLevel, this.WindowBits, this.Strategy);
    }

    public int Deflate(FlushType flush)
    {
      if (this.dstate == null)
        throw new ZlibException("No Deflate State!");
      return this.dstate.Deflate(flush);
    }

    public int EndDeflate()
    {
      if (this.dstate == null)
        throw new ZlibException("No Deflate State!");
      this.dstate = (DeflateManager) null;
      return 0;
    }

    public void ResetDeflate()
    {
      if (this.dstate == null)
        throw new ZlibException("No Deflate State!");
      this.dstate.Reset();
    }

    public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
    {
      if (this.dstate == null)
        throw new ZlibException("No Deflate State!");
      return this.dstate.SetParams(level, strategy);
    }

    public int SetDictionary(byte[] dictionary)
    {
      if (this.istate != null)
        return this.istate.SetDictionary(dictionary);
      if (this.dstate != null)
        return this.dstate.SetDictionary(dictionary);
      throw new ZlibException("No Inflate or Deflate state!");
    }

    internal void flush_pending()
    {
      int length = this.dstate.pendingCount;
      if (length > this.AvailableBytesOut)
        length = this.AvailableBytesOut;
      if (length == 0)
        return;
      if (this.dstate.pending.Length <= this.dstate.nextPending || this.OutputBuffer.Length <= this.NextOut || (this.dstate.pending.Length < this.dstate.nextPending + length || this.OutputBuffer.Length < this.NextOut + length))
        throw new ZlibException(string.Format("Invalid State. (pending.Length={0}, pendingCount={1})", (object) this.dstate.pending.Length, (object) this.dstate.pendingCount));
      Array.Copy((Array) this.dstate.pending, this.dstate.nextPending, (Array) this.OutputBuffer, this.NextOut, length);
      this.NextOut += length;
      this.dstate.nextPending += length;
      this.TotalBytesOut += (long) length;
      this.AvailableBytesOut -= length;
      this.dstate.pendingCount -= length;
      if (this.dstate.pendingCount != 0)
        return;
      this.dstate.nextPending = 0;
    }

    internal int read_buf(byte[] buf, int start, int size)
    {
      int num = this.AvailableBytesIn;
      if (num > size)
        num = size;
      if (num == 0)
        return 0;
      this.AvailableBytesIn -= num;
      if (this.dstate.WantRfc1950HeaderBytes)
        this._Adler32 = Adler.Adler32(this._Adler32, this.InputBuffer, this.NextIn, num);
      Array.Copy((Array) this.InputBuffer, this.NextIn, (Array) buf, start, num);
      this.NextIn += num;
      this.TotalBytesIn += (long) num;
      return num;
    }
  }
}
