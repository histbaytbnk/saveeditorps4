
// Type: Ionic.Zlib.ParallelDeflateOutputStream


// Hacked by SystemAce

using Ionic.Crc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ionic.Zlib
{
  public class ParallelDeflateOutputStream : Stream
  {
    private static readonly int IO_BUFFER_SIZE_DEFAULT = 65536;
    private static readonly int BufferPairsPerCore = 4;
    private int _bufferSize = ParallelDeflateOutputStream.IO_BUFFER_SIZE_DEFAULT;
    private object _outputLock = new object();
    private object _latestLock = new object();
    private object _eLock = new object();
    private ParallelDeflateOutputStream.TraceBits _DesiredTrace = ParallelDeflateOutputStream.TraceBits.EmitAll | ParallelDeflateOutputStream.TraceBits.EmitEnter | ParallelDeflateOutputStream.TraceBits.Session | ParallelDeflateOutputStream.TraceBits.Compress | ParallelDeflateOutputStream.TraceBits.WriteEnter | ParallelDeflateOutputStream.TraceBits.WriteTake;
    private List<WorkItem> _pool;
    private bool _leaveOpen;
    private bool emitting;
    private Stream _outStream;
    private int _maxBufferPairs;
    private AutoResetEvent _newlyCompressedBlob;
    private bool _isClosed;
    private bool _firstWriteDone;
    private int _currentlyFilling;
    private int _lastFilled;
    private int _lastWritten;
    private int _latestCompressed;
    private int _Crc32;
    private CRC32 _runningCrc;
    private Queue<int> _toWrite;
    private Queue<int> _toFill;
    private long _totalBytesProcessed;
    private CompressionLevel _compressLevel;
    private volatile Exception _pendingException;
    private bool _handlingException;

    public CompressionStrategy Strategy { get; private set; }

    public int MaxBufferPairs
    {
      get
      {
        return this._maxBufferPairs;
      }
      set
      {
        if (value < 4)
          throw new ArgumentException("MaxBufferPairs", "Value must be 4 or greater.");
        this._maxBufferPairs = value;
      }
    }

    public int BufferSize
    {
      get
      {
        return this._bufferSize;
      }
      set
      {
        if (value < 1024)
          throw new ArgumentOutOfRangeException("BufferSize", "BufferSize must be greater than 1024 bytes");
        this._bufferSize = value;
      }
    }

    public int Crc32
    {
      get
      {
        return this._Crc32;
      }
    }

    public long BytesProcessed
    {
      get
      {
        return this._totalBytesProcessed;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public override bool CanRead
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
        return this._outStream.CanWrite;
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
        return this._outStream.Position;
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public ParallelDeflateOutputStream(Stream stream)
      : this(stream, CompressionLevel.Default, CompressionStrategy.Default, false)
    {
    }

    public ParallelDeflateOutputStream(Stream stream, CompressionLevel level)
      : this(stream, level, CompressionStrategy.Default, false)
    {
    }

    public ParallelDeflateOutputStream(Stream stream, bool leaveOpen)
      : this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
    {
    }

    public ParallelDeflateOutputStream(Stream stream, CompressionLevel level, bool leaveOpen)
      : this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
    {
    }

    public ParallelDeflateOutputStream(Stream stream, CompressionLevel level, CompressionStrategy strategy, bool leaveOpen)
    {
      this._outStream = stream;
      this._compressLevel = level;
      this.Strategy = strategy;
      this._leaveOpen = leaveOpen;
      this.MaxBufferPairs = 16;
    }

    private void _InitializePoolOfWorkItems()
    {
      this._toWrite = new Queue<int>();
      this._toFill = new Queue<int>();
      this._pool = new List<WorkItem>();
      int num = Math.Min(ParallelDeflateOutputStream.BufferPairsPerCore * Environment.ProcessorCount, this._maxBufferPairs);
      for (int ix = 0; ix < num; ++ix)
      {
        this._pool.Add(new WorkItem(this._bufferSize, this._compressLevel, this.Strategy, ix));
        this._toFill.Enqueue(ix);
      }
      this._newlyCompressedBlob = new AutoResetEvent(false);
      this._runningCrc = new CRC32();
      this._currentlyFilling = -1;
      this._lastFilled = -1;
      this._lastWritten = -1;
      this._latestCompressed = -1;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      bool mustWait = false;
      if (this._isClosed)
        throw new InvalidOperationException();
      if (this._pendingException != null)
      {
        this._handlingException = true;
        Exception exception = this._pendingException;
        this._pendingException = (Exception) null;
        throw exception;
      }
      if (count == 0)
        return;
      if (!this._firstWriteDone)
      {
        this._InitializePoolOfWorkItems();
        this._firstWriteDone = true;
      }
      do
      {
        this.EmitPendingBuffers(false, mustWait);
        mustWait = false;
        int index;
        if (this._currentlyFilling >= 0)
          index = this._currentlyFilling;
        else if (this._toFill.Count == 0)
        {
          mustWait = true;
          goto label_19;
        }
        else
        {
          index = this._toFill.Dequeue();
          ++this._lastFilled;
        }
        WorkItem workItem = this._pool[index];
        int count1 = workItem.buffer.Length - workItem.inputBytesAvailable > count ? count : workItem.buffer.Length - workItem.inputBytesAvailable;
        workItem.ordinal = this._lastFilled;
        Buffer.BlockCopy((Array) buffer, offset, (Array) workItem.buffer, workItem.inputBytesAvailable, count1);
        count -= count1;
        offset += count1;
        workItem.inputBytesAvailable += count1;
        if (workItem.inputBytesAvailable == workItem.buffer.Length)
        {
          if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this._DeflateOne), (object) workItem))
            throw new Exception("Cannot enqueue workitem");
          this._currentlyFilling = -1;
        }
        else
          this._currentlyFilling = index;
label_19:;
      }
      while (count > 0);
    }

    private void _FlushFinish()
    {
      byte[] buffer = new byte[128];
      ZlibCodec zlibCodec = new ZlibCodec();
      zlibCodec.InitializeDeflate(this._compressLevel, false);
      zlibCodec.InputBuffer = (byte[]) null;
      zlibCodec.NextIn = 0;
      zlibCodec.AvailableBytesIn = 0;
      zlibCodec.OutputBuffer = buffer;
      zlibCodec.NextOut = 0;
      zlibCodec.AvailableBytesOut = buffer.Length;
      switch (zlibCodec.Deflate(FlushType.Finish))
      {
        case 1:
        case 0:
          if (buffer.Length - zlibCodec.AvailableBytesOut > 0)
            this._outStream.Write(buffer, 0, buffer.Length - zlibCodec.AvailableBytesOut);
          zlibCodec.EndDeflate();
          this._Crc32 = this._runningCrc.Crc32Result;
          break;
        default:
          throw new Exception("deflating: " + zlibCodec.Message);
      }
    }

    private void _Flush(bool lastInput)
    {
      if (this._isClosed)
        throw new InvalidOperationException();
      if (this.emitting)
        return;
      if (this._currentlyFilling >= 0)
      {
        this._DeflateOne((object) this._pool[this._currentlyFilling]);
        this._currentlyFilling = -1;
      }
      if (lastInput)
      {
        this.EmitPendingBuffers(true, false);
        this._FlushFinish();
      }
      else
        this.EmitPendingBuffers(false, false);
    }

    public override void Flush()
    {
      if (this._pendingException != null)
      {
        this._handlingException = true;
        Exception exception = this._pendingException;
        this._pendingException = (Exception) null;
        throw exception;
      }
      if (this._handlingException)
        return;
      this._Flush(false);
    }

    public override void Close()
    {
      if (this._pendingException != null)
      {
        this._handlingException = true;
        Exception exception = this._pendingException;
        this._pendingException = (Exception) null;
        throw exception;
      }
      if (this._handlingException || this._isClosed)
        return;
      this._Flush(true);
      if (!this._leaveOpen)
        this._outStream.Close();
      this._isClosed = true;
    }

    public new void Dispose()
    {
      this.Close();
      this._pool = (List<WorkItem>) null;
      this.Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

    public void Reset(Stream stream)
    {
      if (!this._firstWriteDone)
        return;
      this._toWrite.Clear();
      this._toFill.Clear();
      foreach (WorkItem workItem in this._pool)
      {
        this._toFill.Enqueue(workItem.index);
        workItem.ordinal = -1;
      }
      this._firstWriteDone = false;
      this._totalBytesProcessed = 0L;
      this._runningCrc = new CRC32();
      this._isClosed = false;
      this._currentlyFilling = -1;
      this._lastFilled = -1;
      this._lastWritten = -1;
      this._latestCompressed = -1;
      this._outStream = stream;
    }

    private void EmitPendingBuffers(bool doAll, bool mustWait)
    {
      if (this.emitting)
        return;
      this.emitting = true;
      if (doAll || mustWait)
        this._newlyCompressedBlob.WaitOne();
      do
      {
        int num = -1;
        int millisecondsTimeout = doAll ? 200 : (mustWait ? -1 : 0);
        int index;
        do
        {
          if (Monitor.TryEnter((object) this._toWrite, millisecondsTimeout))
          {
            index = -1;
            try
            {
              if (this._toWrite.Count > 0)
                index = this._toWrite.Dequeue();
            }
            finally
            {
              Monitor.Exit((object) this._toWrite);
            }
            if (index >= 0)
            {
              WorkItem workItem = this._pool[index];
              if (workItem.ordinal != this._lastWritten + 1)
              {
                lock (this._toWrite)
                  this._toWrite.Enqueue(index);
                if (num == index)
                {
                  this._newlyCompressedBlob.WaitOne();
                  num = -1;
                }
                else if (num == -1)
                  num = index;
              }
              else
              {
                num = -1;
                this._outStream.Write(workItem.compressed, 0, workItem.compressedBytesAvailable);
                this._runningCrc.Combine(workItem.crc, workItem.inputBytesAvailable);
                this._totalBytesProcessed += (long) workItem.inputBytesAvailable;
                workItem.inputBytesAvailable = 0;
                this._lastWritten = workItem.ordinal;
                this._toFill.Enqueue(workItem.index);
                if (millisecondsTimeout == -1)
                  millisecondsTimeout = 0;
              }
            }
          }
          else
            index = -1;
        }
        while (index >= 0);
      }
      while (doAll && this._lastWritten != this._latestCompressed);
      this.emitting = false;
    }

    private void _DeflateOne(object wi)
    {
      WorkItem workitem = (WorkItem) wi;
      try
      {
        int num = workitem.index;
        CRC32 crC32 = new CRC32();
        crC32.SlurpBlock(workitem.buffer, 0, workitem.inputBytesAvailable);
        this.DeflateOneSegment(workitem);
        workitem.crc = crC32.Crc32Result;
        lock (this._latestLock)
        {
          if (workitem.ordinal > this._latestCompressed)
            this._latestCompressed = workitem.ordinal;
        }
        lock (this._toWrite)
          this._toWrite.Enqueue(workitem.index);
        this._newlyCompressedBlob.Set();
      }
      catch (Exception ex)
      {
        lock (this._eLock)
        {
          if (this._pendingException == null)
            return;
          this._pendingException = ex;
        }
      }
    }

    private bool DeflateOneSegment(WorkItem workitem)
    {
      ZlibCodec zlibCodec = workitem.compressor;
      zlibCodec.ResetDeflate();
      zlibCodec.NextIn = 0;
      zlibCodec.AvailableBytesIn = workitem.inputBytesAvailable;
      zlibCodec.NextOut = 0;
      zlibCodec.AvailableBytesOut = workitem.compressed.Length;
      do
      {
        zlibCodec.Deflate(FlushType.None);
      }
      while (zlibCodec.AvailableBytesIn > 0 || zlibCodec.AvailableBytesOut == 0);
      zlibCodec.Deflate(FlushType.Sync);
      workitem.compressedBytesAvailable = (int) zlibCodec.TotalBytesOut;
      return true;
    }

    [Conditional("Trace")]
    private void TraceOutput(ParallelDeflateOutputStream.TraceBits bits, string format, params object[] varParams)
    {
      if ((bits & this._DesiredTrace) == ParallelDeflateOutputStream.TraceBits.None)
        return;
      lock (this._outputLock)
      {
        int local_0 = Thread.CurrentThread.GetHashCode();
        Console.ForegroundColor = (ConsoleColor) (local_0 % 8 + 8);
        Console.Write("{0:000} PDOS ", (object) local_0);
        Console.WriteLine(format, varParams);
        Console.ResetColor();
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    [Flags]
    private enum TraceBits : uint
    {
      None = 0U,
      NotUsed1 = 1U,
      EmitLock = 2U,
      EmitEnter = 4U,
      EmitBegin = 8U,
      EmitDone = 16U,
      EmitSkip = 32U,
      EmitAll = EmitSkip | EmitDone | EmitBegin | EmitLock,
      Flush = 64U,
      Lifecycle = 128U,
      Session = 256U,
      Synch = 512U,
      Instance = 1024U,
      Compress = 2048U,
      Write = 4096U,
      WriteEnter = 8192U,
      WriteTake = 16384U,
      All = 4294967295U,
    }
  }
}
