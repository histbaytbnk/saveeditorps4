
// Type: Ionic.Zip.ZipInputStream


// Hacked by SystemAce

using Ionic.Crc;
using System;
using System.IO;
using System.Text;

namespace Ionic.Zip
{
  public class ZipInputStream : Stream
  {
    private Stream _inputStream;
    private Encoding _provisionalAlternateEncoding;
    private ZipEntry _currentEntry;
    private bool _firstEntry;
    private bool _needSetup;
    private ZipContainer _container;
    private CrcCalculatorStream _crcStream;
    private long _LeftToRead;
    internal string _Password;
    private long _endOfEntry;
    private string _name;
    private bool _leaveUnderlyingStreamOpen;
    private bool _closed;
    private bool _findRequired;
    private bool _exceptionPending;

    public Encoding ProvisionalAlternateEncoding
    {
      get
      {
        return this._provisionalAlternateEncoding;
      }
      set
      {
        this._provisionalAlternateEncoding = value;
      }
    }

    public int CodecBufferSize { get; set; }

    public string Password
    {
      set
      {
        if (this._closed)
        {
          this._exceptionPending = true;
          throw new InvalidOperationException("The stream has been closed.");
        }
        this._Password = value;
      }
    }

    internal Stream ReadStream
    {
      get
      {
        return this._inputStream;
      }
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return this._inputStream.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return this._inputStream.Length;
      }
    }

    public override long Position
    {
      get
      {
        return this._inputStream.Position;
      }
      set
      {
        this.Seek(value, SeekOrigin.Begin);
      }
    }

    public ZipInputStream(Stream stream)
      : this(stream, false)
    {
    }

    public ZipInputStream(string fileName)
    {
      this._Init((Stream) File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), false, fileName);
    }

    public ZipInputStream(Stream stream, bool leaveOpen)
    {
      this._Init(stream, leaveOpen, (string) null);
    }

    private void _Init(Stream stream, bool leaveOpen, string name)
    {
      this._inputStream = stream;
      if (!this._inputStream.CanRead)
        throw new ZipException("The stream must be readable.");
      this._container = new ZipContainer((object) this);
      this._provisionalAlternateEncoding = Encoding.GetEncoding("IBM437");
      this._leaveUnderlyingStreamOpen = leaveOpen;
      this._findRequired = true;
      this._name = name ?? "(stream)";
    }
        

    private void SetupStream()
    {
      this._crcStream = this._currentEntry.InternalOpenReader(this._Password);
      this._LeftToRead = this._crcStream.Length;
      this._needSetup = false;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this._closed)
      {
        this._exceptionPending = true;
        throw new InvalidOperationException("The stream has been closed.");
      }
      if (this._needSetup)
        this.SetupStream();
      if (this._LeftToRead == 0L)
        return 0;
      int count1 = this._LeftToRead > (long) count ? count : (int) this._LeftToRead;
      int num = this._crcStream.Read(buffer, offset, count1);
      this._LeftToRead -= (long) num;
      if (this._LeftToRead == 0L)
      {
        this._currentEntry.VerifyCrcAfterExtract(this._crcStream.Crc);
        this._inputStream.Seek(this._endOfEntry, SeekOrigin.Begin);
      }
      return num;
    }

    public ZipEntry GetNextEntry()
    {
      if (this._findRequired)
      {
        if (SharedUtilities.FindSignature(this._inputStream, 67324752) == -1L)
          return (ZipEntry) null;
        this._inputStream.Seek(-4L, SeekOrigin.Current);
      }
      else if (this._firstEntry)
        this._inputStream.Seek(this._endOfEntry, SeekOrigin.Begin);
      this._currentEntry = ZipEntry.ReadEntry(this._container, !this._firstEntry);
      this._endOfEntry = this._inputStream.Position;
      this._firstEntry = true;
      this._needSetup = true;
      this._findRequired = false;
      return this._currentEntry;
    }

    protected override void Dispose(bool disposing)
    {
      if (this._closed)
        return;
      if (disposing)
      {
        if (this._exceptionPending)
          return;
        if (!this._leaveUnderlyingStreamOpen)
          this._inputStream.Dispose();
      }
      this._closed = true;
    }

    public override void Flush()
    {
      throw new NotSupportedException("Flush");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException("Write");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this._findRequired = true;
      return this._inputStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }
  }
}
