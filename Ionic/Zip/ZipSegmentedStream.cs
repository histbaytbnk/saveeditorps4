
// Type: Ionic.Zip.ZipSegmentedStream


// Hacked by SystemAce

using System;
using System.IO;

namespace Ionic.Zip
{
  internal class ZipSegmentedStream : Stream
  {
    private ZipSegmentedStream.RwMode rwMode;
    private bool _exceptionPending;
    private string _baseName;
    private string _baseDir;
    private string _currentName;
    private string _currentTempName;
    private uint _currentDiskNumber;
    private uint _maxDiskNumber;
    private int _maxSegmentSize;
    private Stream _innerStream;

    public bool ContiguousWrite { get; set; }

    public uint CurrentSegment
    {
      get
      {
        return this._currentDiskNumber;
      }
      private set
      {
        this._currentDiskNumber = value;
        this._currentName = (string) null;
      }
    }

    public string CurrentName
    {
      get
      {
        if (this._currentName == null)
          this._currentName = this._NameForSegment(this.CurrentSegment);
        return this._currentName;
      }
    }

    public string CurrentTempName
    {
      get
      {
        return this._currentTempName;
      }
    }

    public override bool CanRead
    {
      get
      {
        if (this.rwMode == ZipSegmentedStream.RwMode.ReadOnly && this._innerStream != null)
          return this._innerStream.CanRead;
        return false;
      }
    }

    public override bool CanSeek
    {
      get
      {
        if (this._innerStream != null)
          return this._innerStream.CanSeek;
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        if (this.rwMode == ZipSegmentedStream.RwMode.Write && this._innerStream != null)
          return this._innerStream.CanWrite;
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return this._innerStream.Length;
      }
    }

    public override long Position
    {
      get
      {
        return this._innerStream.Position;
      }
      set
      {
        this._innerStream.Position = value;
      }
    }

    private ZipSegmentedStream()
    {
      this._exceptionPending = false;
    }

    public static ZipSegmentedStream ForReading(string name, uint initialDiskNumber, uint maxDiskNumber)
    {
      ZipSegmentedStream zipSegmentedStream = new ZipSegmentedStream()
      {
        rwMode = ZipSegmentedStream.RwMode.ReadOnly,
        CurrentSegment = initialDiskNumber,
        _maxDiskNumber = maxDiskNumber,
        _baseName = name
      };
      zipSegmentedStream._SetReadStream();
      return zipSegmentedStream;
    }

    public static ZipSegmentedStream ForWriting(string name, int maxSegmentSize)
    {
      ZipSegmentedStream zipSegmentedStream = new ZipSegmentedStream()
      {
        rwMode = ZipSegmentedStream.RwMode.Write,
        CurrentSegment = 0U,
        _baseName = name,
        _maxSegmentSize = maxSegmentSize,
        _baseDir = Path.GetDirectoryName(name)
      };
      if (zipSegmentedStream._baseDir == "")
        zipSegmentedStream._baseDir = ".";
      zipSegmentedStream._SetWriteStream(0U);
      return zipSegmentedStream;
    }

    public static Stream ForUpdate(string name, uint diskNumber)
    {
      if (diskNumber >= 99U)
        throw new ArgumentOutOfRangeException("diskNumber");
      return (Stream) File.Open(string.Format("{0}.z{1:D2}", (object) Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name)), (object) (uint) ((int) diskNumber + 1)), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }

    private string _NameForSegment(uint diskNumber)
    {
      if (diskNumber >= 99U)
      {
        this._exceptionPending = true;
        throw new OverflowException("The number of zip segments would exceed 99.");
      }
      return string.Format("{0}.z{1:D2}", (object) Path.Combine(Path.GetDirectoryName(this._baseName), Path.GetFileNameWithoutExtension(this._baseName)), (object) (uint) ((int) diskNumber + 1));
    }

    public uint ComputeSegment(int length)
    {
      if (this._innerStream.Position + (long) length > (long) this._maxSegmentSize)
        return this.CurrentSegment + 1U;
      return this.CurrentSegment;
    }

    public override string ToString()
    {
      return string.Format("{0}[{1}][{2}], pos=0x{3:X})", (object) "ZipSegmentedStream", (object) this.CurrentName, (object) this.rwMode.ToString(), (object) this.Position);
    }

    private void _SetReadStream()
    {
      if (this._innerStream != null)
        this._innerStream.Dispose();
      if ((int) this.CurrentSegment + 1 == (int) this._maxDiskNumber)
        this._currentName = this._baseName;
      this._innerStream = (Stream) File.OpenRead(this.CurrentName);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.rwMode != ZipSegmentedStream.RwMode.ReadOnly)
      {
        this._exceptionPending = true;
        throw new InvalidOperationException("Stream Error: Cannot Read.");
      }
      int num1 = this._innerStream.Read(buffer, offset, count);
      int num2 = num1;
      while (num2 != count)
      {
        if (this._innerStream.Position != this._innerStream.Length)
        {
          this._exceptionPending = true;
          throw new ZipException(string.Format("Read error in file {0}", (object) this.CurrentName));
        }
        if ((int) this.CurrentSegment + 1 == (int) this._maxDiskNumber)
          return num1;
        ++this.CurrentSegment;
        this._SetReadStream();
        offset += num2;
        count -= num2;
        num2 = this._innerStream.Read(buffer, offset, count);
        num1 += num2;
      }
      return num1;
    }

    private void _SetWriteStream(uint increment)
    {
      if (this._innerStream != null)
      {
        this._innerStream.Dispose();
        if (File.Exists(this.CurrentName))
          File.Delete(this.CurrentName);
        File.Move(this._currentTempName, this.CurrentName);
      }
      if (increment > 0U)
        this.CurrentSegment += increment;
      SharedUtilities.CreateAndOpenUniqueTempFile(this._baseDir, out this._innerStream, out this._currentTempName);
      if ((int) this.CurrentSegment != 0)
        return;
      this._innerStream.Write(BitConverter.GetBytes(134695760), 0, 4);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.rwMode != ZipSegmentedStream.RwMode.Write)
      {
        this._exceptionPending = true;
        throw new InvalidOperationException("Stream Error: Cannot Write.");
      }
      if (this.ContiguousWrite)
      {
        if (this._innerStream.Position + (long) count > (long) this._maxSegmentSize)
          this._SetWriteStream(1U);
      }
      else
      {
        while (this._innerStream.Position + (long) count > (long) this._maxSegmentSize)
        {
          int count1 = this._maxSegmentSize - (int) this._innerStream.Position;
          this._innerStream.Write(buffer, offset, count1);
          this._SetWriteStream(1U);
          count -= count1;
          offset += count1;
        }
      }
      this._innerStream.Write(buffer, offset, count);
    }

    public long TruncateBackward(uint diskNumber, long offset)
    {
      if (diskNumber >= 99U)
        throw new ArgumentOutOfRangeException("diskNumber");
      if (this.rwMode != ZipSegmentedStream.RwMode.Write)
      {
        this._exceptionPending = true;
        throw new ZipException("bad state.");
      }
      if ((int) diskNumber == (int) this.CurrentSegment)
        return this._innerStream.Seek(offset, SeekOrigin.Begin);
      if (this._innerStream != null)
      {
        this._innerStream.Dispose();
        if (File.Exists(this._currentTempName))
          File.Delete(this._currentTempName);
      }
      for (uint diskNumber1 = this.CurrentSegment - 1U; diskNumber1 > diskNumber; --diskNumber1)
      {
        string path = this._NameForSegment(diskNumber1);
        if (File.Exists(path))
          File.Delete(path);
      }
      this.CurrentSegment = diskNumber;
      for (int index = 0; index < 3; ++index)
      {
        try
        {
          this._currentTempName = SharedUtilities.InternalGetTempFileName();
          File.Move(this.CurrentName, this._currentTempName);
          break;
        }
        catch (IOException ex)
        {
          if (index == 2)
            throw;
        }
      }
      this._innerStream = (Stream) new FileStream(this._currentTempName, FileMode.Open);
      return this._innerStream.Seek(offset, SeekOrigin.Begin);
    }

    public override void Flush()
    {
      this._innerStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this._innerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      if (this.rwMode != ZipSegmentedStream.RwMode.Write)
      {
        this._exceptionPending = true;
        throw new InvalidOperationException();
      }
      this._innerStream.SetLength(value);
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (this._innerStream == null)
          return;
        this._innerStream.Dispose();
        if (this.rwMode != ZipSegmentedStream.RwMode.Write)
          return;
        int num = this._exceptionPending ? 1 : 0;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    private enum RwMode
    {
      None,
      ReadOnly,
      Write,
    }
  }
}
