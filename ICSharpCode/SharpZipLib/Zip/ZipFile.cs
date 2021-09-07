
// Type: ICSharpCode.SharpZipLib.Zip.ZipFile


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Encryption;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class ZipFile : IEnumerable, IDisposable
  {
    private UseZip64 useZip64_ = UseZip64.Dynamic;
    private int bufferSize_ = 4096;
    private IEntryFactory updateEntryFactory_ = (IEntryFactory) new ZipEntryFactory();
    private const int DefaultBufferSize = 4096;
    public ZipFile.KeysRequiredEventHandler KeysRequired;
    private bool isDisposed_;
    private string name_;
    private string comment_;
    private string rawPassword_;
    private Stream baseStream_;
    private bool isStreamOwner;
    private long offsetOfFirstEntry;
    private ZipEntry[] entries_;
    private byte[] key;
    private bool isNewArchive_;
    private ArrayList updates_;
    private long updateCount_;
    private Hashtable updateIndex_;
    private IArchiveStorage archiveStorage_;
    private IDynamicDataSource updateDataSource_;
    private bool contentsEdited_;
    private byte[] copyBuffer_;
    private ZipFile.ZipString newComment_;
    private bool commentEdited_;

    private byte[] Key
    {
      get
      {
        return this.key;
      }
      set
      {
        this.key = value;
      }
    }

    public string Password
    {
      set
      {
        if (value == null || value.Length == 0)
        {
          this.key = (byte[]) null;
        }
        else
        {
          this.rawPassword_ = value;
          this.key = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(value));
        }
      }
    }

    private bool HaveKeys
    {
      get
      {
        return this.key != null;
      }
    }

    public bool IsStreamOwner
    {
      get
      {
        return this.isStreamOwner;
      }
      set
      {
        this.isStreamOwner = value;
      }
    }

    public bool IsEmbeddedArchive
    {
      get
      {
        return this.offsetOfFirstEntry > 0L;
      }
    }

    public bool IsNewArchive
    {
      get
      {
        return this.isNewArchive_;
      }
    }

    public string ZipFileComment
    {
      get
      {
        return this.comment_;
      }
    }

    public string Name
    {
      get
      {
        return this.name_;
      }
    }

    [Obsolete("Use the Count property instead")]
    public int Size
    {
      get
      {
        return this.entries_.Length;
      }
    }

    public long Count
    {
      get
      {
        return (long) this.entries_.Length;
      }
    }

    [IndexerName("EntryByIndex")]
    public ZipEntry this[int index]
    {
      get
      {
        return (ZipEntry) this.entries_[index].Clone();
      }
    }

    public INameTransform NameTransform
    {
      get
      {
        return this.updateEntryFactory_.NameTransform;
      }
      set
      {
        this.updateEntryFactory_.NameTransform = value;
      }
    }

    public IEntryFactory EntryFactory
    {
      get
      {
        return this.updateEntryFactory_;
      }
      set
      {
        if (value == null)
          this.updateEntryFactory_ = (IEntryFactory) new ZipEntryFactory();
        else
          this.updateEntryFactory_ = value;
      }
    }

    public int BufferSize
    {
      get
      {
        return this.bufferSize_;
      }
      set
      {
        if (value < 1024)
          throw new ArgumentOutOfRangeException("value", "cannot be below 1024");
        if (this.bufferSize_ == value)
          return;
        this.bufferSize_ = value;
        this.copyBuffer_ = (byte[]) null;
      }
    }

    public bool IsUpdating
    {
      get
      {
        return this.updates_ != null;
      }
    }

    public UseZip64 UseZip64
    {
      get
      {
        return this.useZip64_;
      }
      set
      {
        this.useZip64_ = value;
      }
    }

    public ZipFile(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      this.name_ = name;
      this.baseStream_ = (Stream) File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
      this.isStreamOwner = true;
      try
      {
        this.ReadEntries();
      }
      catch
      {
        this.DisposeInternal(true);
        throw;
      }
    }

    public ZipFile(FileStream file)
    {
      if (file == null)
        throw new ArgumentNullException("file");
      if (!file.CanSeek)
        throw new ArgumentException("Stream is not seekable", "file");
      this.baseStream_ = (Stream) file;
      this.name_ = file.Name;
      this.isStreamOwner = true;
      try
      {
        this.ReadEntries();
      }
      catch
      {
        this.DisposeInternal(true);
        throw;
      }
    }

    public ZipFile(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanSeek)
        throw new ArgumentException("Stream is not seekable", "stream");
      this.baseStream_ = stream;
      this.isStreamOwner = true;
      if (this.baseStream_.Length > 0L)
      {
        try
        {
          this.ReadEntries();
        }
        catch
        {
          this.DisposeInternal(true);
          throw;
        }
      }
      else
      {
        this.entries_ = new ZipEntry[0];
        this.isNewArchive_ = true;
      }
    }

    internal ZipFile()
    {
      this.entries_ = new ZipEntry[0];
      this.isNewArchive_ = true;
    }

    ~ZipFile()
    {
      this.Dispose(false);
    }

    private void OnKeysRequired(string fileName)
    {
      if (this.KeysRequired == null)
        return;
      KeysRequiredEventArgs e = new KeysRequiredEventArgs(fileName, this.key);
      this.KeysRequired((object) this, e);
      this.key = e.Key;
    }

    public void Close()
    {
      this.DisposeInternal(true);
      GC.SuppressFinalize((object) this);
    }

    public static ZipFile Create(string fileName)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      FileStream fileStream = File.Create(fileName);
      return new ZipFile()
      {
        name_ = fileName,
        baseStream_ = (Stream) fileStream,
        isStreamOwner = true
      };
    }

    public static ZipFile Create(Stream outStream)
    {
      if (outStream == null)
        throw new ArgumentNullException("outStream");
      if (!outStream.CanWrite)
        throw new ArgumentException("Stream is not writeable", "outStream");
      if (!outStream.CanSeek)
        throw new ArgumentException("Stream is not seekable", "outStream");
      return new ZipFile()
      {
        baseStream_ = outStream
      };
    }

    public IEnumerator GetEnumerator()
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      return (IEnumerator) new ZipFile.ZipEntryEnumerator(this.entries_);
    }

    public int FindEntry(string name, bool ignoreCase)
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      for (int index = 0; index < this.entries_.Length; ++index)
      {
        if (string.Compare(name, this.entries_[index].Name, ignoreCase, CultureInfo.InvariantCulture) == 0)
          return index;
      }
      return -1;
    }

    public ZipEntry GetEntry(string name)
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      int entry = this.FindEntry(name, true);
      if (entry < 0)
        return (ZipEntry) null;
      return (ZipEntry) this.entries_[entry].Clone();
    }

    public Stream GetInputStream(ZipEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException("entry");
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      long entryIndex = entry.ZipFileIndex;
      if (entryIndex < 0L || entryIndex >= (long) this.entries_.Length || this.entries_[entryIndex].Name != entry.Name)
      {
        entryIndex = (long) this.FindEntry(entry.Name, true);
        if (entryIndex < 0L)
          throw new ZipException("Entry cannot be found");
      }
      return this.GetInputStream(entryIndex);
    }

    public Stream GetInputStream(long entryIndex)
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      long start = this.LocateEntry(this.entries_[entryIndex]);
      CompressionMethod compressionMethod = this.entries_[entryIndex].CompressionMethod;
      Stream stream = (Stream) new ZipFile.PartialInputStream(this, start, this.entries_[entryIndex].CompressedSize);
      if (this.entries_[entryIndex].IsCrypted)
      {
        stream = this.CreateAndInitDecryptionStream(stream, this.entries_[entryIndex]);
        if (stream == null)
          throw new ZipException("Unable to decrypt this entry");
      }
      switch (compressionMethod)
      {
        case CompressionMethod.Stored:
          return stream;
        case CompressionMethod.Deflated:
          stream = (Stream) new InflaterInputStream(stream, new Inflater(true));
          goto case CompressionMethod.Stored;
        default:
          throw new ZipException("Unsupported compression method " + (object) compressionMethod);
      }
    }

    public bool TestArchive(bool testData)
    {
      return this.TestArchive(testData, TestStrategy.FindFirstError, (ZipTestResultHandler) null);
    }

    public bool TestArchive(bool testData, TestStrategy strategy, ZipTestResultHandler resultHandler)
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      TestStatus status = new TestStatus(this);
      if (resultHandler != null)
        resultHandler(status, (string) null);
      ZipFile.HeaderTest tests = testData ? ZipFile.HeaderTest.Extract | ZipFile.HeaderTest.Header : ZipFile.HeaderTest.Header;
      bool flag = true;
      try
      {
        for (int index = 0; flag && (long) index < this.Count; ++index)
        {
          if (resultHandler != null)
          {
            status.SetEntry(this[index]);
            status.SetOperation(TestOperation.EntryHeader);
            resultHandler(status, (string) null);
          }
          try
          {
            this.TestLocalHeader(this[index], tests);
          }
          catch (ZipException ex)
          {
            status.AddError();
            if (resultHandler != null)
              resultHandler(status, string.Format("Exception during test - '{0}'", (object) ex.Message));
            if (strategy == TestStrategy.FindFirstError)
              flag = false;
          }
          if (flag && testData && this[index].IsFile)
          {
            if (resultHandler != null)
            {
              status.SetOperation(TestOperation.EntryData);
              resultHandler(status, (string) null);
            }
            Crc32 crc32 = new Crc32();
            using (Stream inputStream = this.GetInputStream(this[index]))
            {
              byte[] buffer = new byte[4096];
              long num = 0L;
              int count;
              while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
              {
                crc32.Update(buffer, 0, count);
                if (resultHandler != null)
                {
                  num += (long) count;
                  status.SetBytesTested(num);
                  resultHandler(status, (string) null);
                }
              }
            }
            if (this[index].Crc != crc32.Value)
            {
              status.AddError();
              if (resultHandler != null)
                resultHandler(status, "CRC mismatch");
              if (strategy == TestStrategy.FindFirstError)
                flag = false;
            }
            if ((this[index].Flags & 8) != 0)
            {
              ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_);
              DescriptorData data = new DescriptorData();
              zipHelperStream.ReadDataDescriptor(this[index].LocalHeaderRequiresZip64, data);
              if (this[index].Crc != data.Crc)
                status.AddError();
              if (this[index].CompressedSize != data.CompressedSize)
                status.AddError();
              if (this[index].Size != data.Size)
                status.AddError();
            }
          }
          if (resultHandler != null)
          {
            status.SetOperation(TestOperation.EntryComplete);
            resultHandler(status, (string) null);
          }
        }
        if (resultHandler != null)
        {
          status.SetOperation(TestOperation.MiscellaneousTests);
          resultHandler(status, (string) null);
        }
      }
      catch (Exception ex)
      {
        status.AddError();
        if (resultHandler != null)
          resultHandler(status, string.Format("Exception during test - '{0}'", (object) ex.Message));
      }
      if (resultHandler != null)
      {
        status.SetOperation(TestOperation.Complete);
        status.SetEntry((ZipEntry) null);
        resultHandler(status, (string) null);
      }
      return status.ErrorCount == 0;
    }

    private long TestLocalHeader(ZipEntry entry, ZipFile.HeaderTest tests)
    {
      lock (this.baseStream_)
      {
        bool local_0 = (tests & ZipFile.HeaderTest.Header) != (ZipFile.HeaderTest) 0;
        bool local_1 = (tests & ZipFile.HeaderTest.Extract) != (ZipFile.HeaderTest) 0;
        this.baseStream_.Seek(this.offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);
        if ((int) this.ReadLEUint() != 67324752)
          throw new ZipException(string.Format("Wrong local header signature @{0:X}", (object) (this.offsetOfFirstEntry + entry.Offset)));
        short local_2 = (short) this.ReadLEUshort();
        short local_3 = (short) this.ReadLEUshort();
        short local_4 = (short) this.ReadLEUshort();
        short local_5 = (short) this.ReadLEUshort();
        short local_6 = (short) this.ReadLEUshort();
        uint local_7 = this.ReadLEUint();
        long local_8 = (long) this.ReadLEUint();
        long local_9 = (long) this.ReadLEUint();
        int local_10 = (int) this.ReadLEUshort();
        int local_11 = (int) this.ReadLEUshort();
        byte[] local_12 = new byte[local_10];
        StreamUtils.ReadFully(this.baseStream_, local_12);
        byte[] local_13 = new byte[local_11];
        StreamUtils.ReadFully(this.baseStream_, local_13);
        ZipExtraData local_14 = new ZipExtraData(local_13);
        if (local_14.Find(1))
        {
          local_9 = local_14.ReadLong();
          local_8 = local_14.ReadLong();
          if (((int) local_3 & 8) != 0)
          {
            if (local_9 != -1L && local_9 != entry.Size)
              throw new ZipException("Size invalid for descriptor");
            if (local_8 != -1L && local_8 != entry.CompressedSize)
              throw new ZipException("Compressed size invalid for descriptor");
          }
        }
        else if ((int) local_2 >= 45 && ((int) (uint) local_9 == -1 || (int) (uint) local_8 == -1))
          throw new ZipException("Required Zip64 extended information missing");
        if (local_1 && entry.IsFile)
        {
          if (!entry.IsCompressionMethodSupported())
            throw new ZipException("Compression method not supported");
          if ((int) local_2 > 51 || (int) local_2 > 20 && (int) local_2 < 45)
            throw new ZipException(string.Format("Version required to extract this entry not supported ({0})", (object) local_2));
          if (((int) local_3 & 12384) != 0)
            throw new ZipException("The library does not support the zip version required to extract this entry");
        }
        if (local_0)
        {
          if ((int) local_2 <= 63 && (int) local_2 != 10 && ((int) local_2 != 11 && (int) local_2 != 20) && ((int) local_2 != 21 && (int) local_2 != 25 && ((int) local_2 != 27 && (int) local_2 != 45)) && ((int) local_2 != 46 && (int) local_2 != 50 && ((int) local_2 != 51 && (int) local_2 != 52) && ((int) local_2 != 61 && (int) local_2 != 62 && (int) local_2 != 63)))
            throw new ZipException(string.Format("Version required to extract this entry is invalid ({0})", (object) local_2));
          if (((int) local_3 & 49168) != 0)
            throw new ZipException("Reserved bit flags cannot be set.");
          if (((int) local_3 & 1) != 0 && (int) local_2 < 20)
            throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", (object) local_2));
          if (((int) local_3 & 64) != 0)
          {
            if (((int) local_3 & 1) == 0)
              throw new ZipException("Strong encryption flag set but encryption flag is not set");
            if ((int) local_2 < 50)
              throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", (object) local_2));
          }
          if (((int) local_3 & 32) != 0 && (int) local_2 < 27)
            throw new ZipException(string.Format("Patched data requires higher version than ({0})", (object) local_2));
          if ((int) local_3 != entry.Flags)
            throw new ZipException("Central header/local header flags mismatch");
          if (entry.CompressionMethod != (CompressionMethod) local_4)
            throw new ZipException("Central header/local header compression method mismatch");
          if (entry.Version != (int) local_2)
            throw new ZipException("Extract version mismatch");
          if (((int) local_3 & 64) != 0 && (int) local_2 < 62)
            throw new ZipException("Strong encryption flag set but version not high enough");
          if (((int) local_3 & 8192) != 0 && ((int) local_5 != 0 || (int) local_6 != 0))
            throw new ZipException("Header masked set but date/time values non-zero");
          if (((int) local_3 & 8) == 0 && (int) local_7 != (int) (uint) entry.Crc)
            throw new ZipException("Central header/local header crc mismatch");
          if (local_9 == 0L && local_8 == 0L && (int) local_7 != 0)
            throw new ZipException("Invalid CRC for empty entry");
          if (entry.Name.Length > local_10)
            throw new ZipException("File name length mismatch");
          string local_15 = ZipConstants.ConvertToStringExt((int) local_3, local_12);
          if (local_15 != entry.Name)
            throw new ZipException("Central header and local header file name mismatch");
          if (entry.IsDirectory)
          {
            if (local_9 > 0L)
              throw new ZipException("Directory cannot have size");
            if (entry.IsCrypted)
            {
              if (local_8 > 14L)
                throw new ZipException("Directory compressed size invalid");
            }
            else if (local_8 > 2L)
              throw new ZipException("Directory compressed size invalid");
          }
          if (!ZipNameTransform.IsValidName(local_15, true))
            throw new ZipException("Name is invalid");
        }
        if (((int) local_3 & 8) == 0 || local_9 > 0L || local_8 > 0L)
        {
          if (local_9 != entry.Size)
            throw new ZipException(string.Format("Size mismatch between central header({0}) and local header({1})", (object) entry.Size, (object) local_9));
          if (local_8 != entry.CompressedSize && local_8 != (long) uint.MaxValue && local_8 != -1L)
            throw new ZipException(string.Format("Compressed size mismatch between central header({0}) and local header({1})", (object) entry.CompressedSize, (object) local_8));
        }
        int local_16 = local_10 + local_11;
        return this.offsetOfFirstEntry + entry.Offset + 30L + (long) local_16;
      }
    }

    public void BeginUpdate(IArchiveStorage archiveStorage, IDynamicDataSource dataSource)
    {
      if (archiveStorage == null)
        throw new ArgumentNullException("archiveStorage");
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      if (this.IsEmbeddedArchive)
        throw new ZipException("Cannot update embedded/SFX archives");
      this.archiveStorage_ = archiveStorage;
      this.updateDataSource_ = dataSource;
      this.updateIndex_ = new Hashtable();
      this.updates_ = new ArrayList(this.entries_.Length);
      foreach (ZipEntry entry in this.entries_)
      {
        int num = this.updates_.Add((object) new ZipFile.ZipUpdate(entry));
        this.updateIndex_.Add((object) entry.Name, (object) num);
      }
      this.updates_.Sort((IComparer) new ZipFile.UpdateComparer());
      int num1 = 0;
      foreach (ZipFile.ZipUpdate zipUpdate in this.updates_)
      {
        if (num1 != this.updates_.Count - 1)
        {
          zipUpdate.OffsetBasedSize = ((ZipFile.ZipUpdate) this.updates_[num1 + 1]).Entry.Offset - zipUpdate.Entry.Offset;
          ++num1;
        }
        else
          break;
      }
      this.updateCount_ = (long) this.updates_.Count;
      this.contentsEdited_ = false;
      this.commentEdited_ = false;
      this.newComment_ = (ZipFile.ZipString) null;
    }

    public void BeginUpdate(IArchiveStorage archiveStorage)
    {
      this.BeginUpdate(archiveStorage, (IDynamicDataSource) new DynamicDiskDataSource());
    }

    public void BeginUpdate()
    {
      if (this.Name == null)
        this.BeginUpdate((IArchiveStorage) new MemoryArchiveStorage(), (IDynamicDataSource) new DynamicDiskDataSource());
      else
        this.BeginUpdate((IArchiveStorage) new DiskArchiveStorage(this), (IDynamicDataSource) new DynamicDiskDataSource());
    }

    public void CommitUpdate()
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      this.CheckUpdating();
      try
      {
        this.updateIndex_.Clear();
        this.updateIndex_ = (Hashtable) null;
        if (this.contentsEdited_)
          this.RunUpdates();
        else if (this.commentEdited_)
        {
          this.UpdateCommentOnly();
        }
        else
        {
          if (this.entries_.Length != 0)
            return;
          byte[] comment = this.newComment_ != null ? this.newComment_.RawComment : ZipConstants.ConvertToArray(this.comment_);
          using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_))
            zipHelperStream.WriteEndOfCentralDirectory(0L, 0L, 0L, comment);
        }
      }
      finally
      {
        this.PostUpdateCleanup();
      }
    }

    public void AbortUpdate()
    {
      this.PostUpdateCleanup();
    }

    public void SetComment(string comment)
    {
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      this.CheckUpdating();
      this.newComment_ = new ZipFile.ZipString(comment);
      if (this.newComment_.RawLength > (int) ushort.MaxValue)
      {
        this.newComment_ = (ZipFile.ZipString) null;
        throw new ZipException("Comment length exceeds maximum - 65535");
      }
      this.commentEdited_ = true;
    }

    private void AddUpdate(ZipFile.ZipUpdate update)
    {
      this.contentsEdited_ = true;
      int existingUpdate = this.FindExistingUpdate(update.Entry.Name);
      if (existingUpdate >= 0)
      {
        if (this.updates_[existingUpdate] == null)
          ++this.updateCount_;
        this.updates_[existingUpdate] = (object) update;
      }
      else
      {
        int num = this.updates_.Add((object) update);
        ++this.updateCount_;
        this.updateIndex_.Add((object) update.Entry.Name, (object) num);
      }
    }

    public void Add(string fileName, CompressionMethod compressionMethod, bool useUnicodeText)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      if (this.isDisposed_)
        throw new ObjectDisposedException("ZipFile");
      if (!ZipEntry.IsCompressionMethodSupported(compressionMethod))
        throw new ArgumentOutOfRangeException("compressionMethod");
      this.CheckUpdating();
      this.contentsEdited_ = true;
      ZipEntry entry = this.EntryFactory.MakeFileEntry(fileName);
      entry.IsUnicodeText = useUnicodeText;
      entry.CompressionMethod = compressionMethod;
      this.AddUpdate(new ZipFile.ZipUpdate(fileName, entry));
    }

    public void Add(string fileName, CompressionMethod compressionMethod)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      if (!ZipEntry.IsCompressionMethodSupported(compressionMethod))
        throw new ArgumentOutOfRangeException("compressionMethod");
      this.CheckUpdating();
      this.contentsEdited_ = true;
      ZipEntry entry = this.EntryFactory.MakeFileEntry(fileName);
      entry.CompressionMethod = compressionMethod;
      this.AddUpdate(new ZipFile.ZipUpdate(fileName, entry));
    }

    public void Add(string fileName)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      this.CheckUpdating();
      this.AddUpdate(new ZipFile.ZipUpdate(fileName, this.EntryFactory.MakeFileEntry(fileName)));
    }

    public void Add(string fileName, string entryName)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      if (entryName == null)
        throw new ArgumentNullException("entryName");
      this.CheckUpdating();
      this.AddUpdate(new ZipFile.ZipUpdate(fileName, this.EntryFactory.MakeFileEntry(entryName)));
    }

    public void Add(IStaticDataSource dataSource, string entryName)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      if (entryName == null)
        throw new ArgumentNullException("entryName");
      this.CheckUpdating();
      this.AddUpdate(new ZipFile.ZipUpdate(dataSource, this.EntryFactory.MakeFileEntry(entryName, false)));
    }

    public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      if (entryName == null)
        throw new ArgumentNullException("entryName");
      this.CheckUpdating();
      ZipEntry entry = this.EntryFactory.MakeFileEntry(entryName, false);
      entry.CompressionMethod = compressionMethod;
      this.AddUpdate(new ZipFile.ZipUpdate(dataSource, entry));
    }

    public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod, bool useUnicodeText)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      if (entryName == null)
        throw new ArgumentNullException("entryName");
      this.CheckUpdating();
      ZipEntry entry = this.EntryFactory.MakeFileEntry(entryName, false);
      entry.IsUnicodeText = useUnicodeText;
      entry.CompressionMethod = compressionMethod;
      this.AddUpdate(new ZipFile.ZipUpdate(dataSource, entry));
    }

    public void Add(ZipEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException("entry");
      this.CheckUpdating();
      if (entry.Size != 0L || entry.CompressedSize != 0L)
        throw new ZipException("Entry cannot have any data");
      this.AddUpdate(new ZipFile.ZipUpdate(ZipFile.UpdateCommand.Add, entry));
    }

    public void AddDirectory(string directoryName)
    {
      if (directoryName == null)
        throw new ArgumentNullException("directoryName");
      this.CheckUpdating();
      this.AddUpdate(new ZipFile.ZipUpdate(ZipFile.UpdateCommand.Add, this.EntryFactory.MakeDirectoryEntry(directoryName)));
    }

    public bool Delete(string fileName)
    {
      if (fileName == null)
        throw new ArgumentNullException("fileName");
      this.CheckUpdating();
      int existingUpdate = this.FindExistingUpdate(fileName);
      if (existingUpdate < 0 || this.updates_[existingUpdate] == null)
        throw new ZipException("Cannot find entry to delete");
      bool flag = true;
      this.contentsEdited_ = true;
      this.updates_[existingUpdate] = (object) null;
      --this.updateCount_;
      return flag;
    }

    public void Delete(ZipEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException("entry");
      this.CheckUpdating();
      int existingUpdate = this.FindExistingUpdate(entry);
      if (existingUpdate < 0)
        throw new ZipException("Cannot find entry to delete");
      this.contentsEdited_ = true;
      this.updates_[existingUpdate] = (object) null;
      --this.updateCount_;
    }

    private void WriteLEShort(int value)
    {
      this.baseStream_.WriteByte((byte) (value & (int) byte.MaxValue));
      this.baseStream_.WriteByte((byte) (value >> 8 & (int) byte.MaxValue));
    }

    private void WriteLEUshort(ushort value)
    {
      this.baseStream_.WriteByte((byte) ((uint) value & (uint) byte.MaxValue));
      this.baseStream_.WriteByte((byte) ((uint) value >> 8));
    }

    private void WriteLEInt(int value)
    {
      this.WriteLEShort(value & (int) ushort.MaxValue);
      this.WriteLEShort(value >> 16);
    }

    private void WriteLEUint(uint value)
    {
      this.WriteLEUshort((ushort) (value & (uint) ushort.MaxValue));
      this.WriteLEUshort((ushort) (value >> 16));
    }

    private void WriteLeLong(long value)
    {
      this.WriteLEInt((int) (value & (long) uint.MaxValue));
      this.WriteLEInt((int) (value >> 32));
    }

    private void WriteLEUlong(ulong value)
    {
      this.WriteLEUint((uint) (value & (ulong) uint.MaxValue));
      this.WriteLEUint((uint) (value >> 32));
    }

    private void WriteLocalEntryHeader(ZipFile.ZipUpdate update)
    {
      ZipEntry outEntry = update.OutEntry;
      outEntry.Offset = this.baseStream_.Position;
      if (update.Command != ZipFile.UpdateCommand.Copy)
      {
        if (outEntry.CompressionMethod == CompressionMethod.Deflated)
        {
          if (outEntry.Size == 0L)
          {
            outEntry.CompressedSize = outEntry.Size;
            outEntry.Crc = 0L;
            outEntry.CompressionMethod = CompressionMethod.Stored;
          }
        }
        else if (outEntry.CompressionMethod == CompressionMethod.Stored)
          outEntry.Flags &= -9;
        if (this.HaveKeys)
        {
          outEntry.IsCrypted = true;
          if (outEntry.Crc < 0L)
            outEntry.Flags |= 8;
        }
        else
          outEntry.IsCrypted = false;
        switch (this.useZip64_)
        {
          case UseZip64.On:
            outEntry.ForceZip64();
            break;
          case UseZip64.Dynamic:
            if (outEntry.Size < 0L)
            {
              outEntry.ForceZip64();
              break;
            }
            break;
        }
      }
      this.WriteLEInt(67324752);
      this.WriteLEShort(outEntry.Version);
      this.WriteLEShort(outEntry.Flags);
      this.WriteLEShort((int) (byte) outEntry.CompressionMethod);
      this.WriteLEInt((int) outEntry.DosTime);
      if (!outEntry.HasCrc)
      {
        update.CrcPatchOffset = this.baseStream_.Position;
        this.WriteLEInt(0);
      }
      else
        this.WriteLEInt((int) outEntry.Crc);
      if (outEntry.LocalHeaderRequiresZip64)
      {
        this.WriteLEInt(-1);
        this.WriteLEInt(-1);
      }
      else
      {
        if (outEntry.CompressedSize < 0L || outEntry.Size < 0L)
          update.SizePatchOffset = this.baseStream_.Position;
        this.WriteLEInt((int) outEntry.CompressedSize);
        this.WriteLEInt((int) outEntry.Size);
      }
      byte[] buffer = ZipConstants.ConvertToArray(outEntry.Flags, outEntry.Name);
      if (buffer.Length > (int) ushort.MaxValue)
        throw new ZipException("Entry name too long.");
      ZipExtraData zipExtraData = new ZipExtraData(outEntry.ExtraData);
      if (outEntry.LocalHeaderRequiresZip64)
      {
        zipExtraData.StartNewEntry();
        zipExtraData.AddLeLong(outEntry.Size);
        zipExtraData.AddLeLong(outEntry.CompressedSize);
        zipExtraData.AddNewEntry(1);
      }
      else
        zipExtraData.Delete(1);
      outEntry.ExtraData = zipExtraData.GetEntryData();
      this.WriteLEShort(buffer.Length);
      this.WriteLEShort(outEntry.ExtraData.Length);
      if (buffer.Length > 0)
        this.baseStream_.Write(buffer, 0, buffer.Length);
      if (outEntry.LocalHeaderRequiresZip64)
      {
        if (!zipExtraData.Find(1))
          throw new ZipException("Internal error cannot find extra data");
        update.SizePatchOffset = this.baseStream_.Position + (long) zipExtraData.CurrentReadIndex;
      }
      if (outEntry.ExtraData.Length <= 0)
        return;
      this.baseStream_.Write(outEntry.ExtraData, 0, outEntry.ExtraData.Length);
    }

    private int WriteCentralDirectoryHeader(ZipEntry entry)
    {
      if (entry.CompressedSize < 0L)
        throw new ZipException("Attempt to write central directory entry with unknown csize");
      if (entry.Size < 0L)
        throw new ZipException("Attempt to write central directory entry with unknown size");
      if (entry.Crc < 0L)
        throw new ZipException("Attempt to write central directory entry with unknown crc");
      this.WriteLEInt(33639248);
      this.WriteLEShort(51);
      this.WriteLEShort(entry.Version);
      this.WriteLEShort(entry.Flags);
      this.WriteLEShort((int) (byte) entry.CompressionMethod);
      this.WriteLEInt((int) entry.DosTime);
      this.WriteLEInt((int) entry.Crc);
      if (entry.IsZip64Forced() || entry.CompressedSize >= (long) uint.MaxValue)
        this.WriteLEInt(-1);
      else
        this.WriteLEInt((int) (entry.CompressedSize & (long) uint.MaxValue));
      if (entry.IsZip64Forced() || entry.Size >= (long) uint.MaxValue)
        this.WriteLEInt(-1);
      else
        this.WriteLEInt((int) entry.Size);
      byte[] buffer1 = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
      if (buffer1.Length > (int) ushort.MaxValue)
        throw new ZipException("Entry name is too long.");
      this.WriteLEShort(buffer1.Length);
      ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
      if (entry.CentralHeaderRequiresZip64)
      {
        zipExtraData.StartNewEntry();
        if (entry.Size >= (long) uint.MaxValue || this.useZip64_ == UseZip64.On)
          zipExtraData.AddLeLong(entry.Size);
        if (entry.CompressedSize >= (long) uint.MaxValue || this.useZip64_ == UseZip64.On)
          zipExtraData.AddLeLong(entry.CompressedSize);
        if (entry.Offset >= (long) uint.MaxValue)
          zipExtraData.AddLeLong(entry.Offset);
        zipExtraData.AddNewEntry(1);
      }
      else
        zipExtraData.Delete(1);
      byte[] entryData = zipExtraData.GetEntryData();
      this.WriteLEShort(entryData.Length);
      this.WriteLEShort(entry.Comment != null ? entry.Comment.Length : 0);
      this.WriteLEShort(0);
      this.WriteLEShort(0);
      if (entry.ExternalFileAttributes != -1)
        this.WriteLEInt(entry.ExternalFileAttributes);
      else if (entry.IsDirectory)
        this.WriteLEUint(16U);
      else
        this.WriteLEUint(0U);
      if (entry.Offset >= (long) uint.MaxValue)
        this.WriteLEUint(uint.MaxValue);
      else
        this.WriteLEUint((uint) (int) entry.Offset);
      if (buffer1.Length > 0)
        this.baseStream_.Write(buffer1, 0, buffer1.Length);
      if (entryData.Length > 0)
        this.baseStream_.Write(entryData, 0, entryData.Length);
      byte[] buffer2 = entry.Comment != null ? Encoding.ASCII.GetBytes(entry.Comment) : new byte[0];
      if (buffer2.Length > 0)
        this.baseStream_.Write(buffer2, 0, buffer2.Length);
      return 46 + buffer1.Length + entryData.Length + buffer2.Length;
    }

    private void PostUpdateCleanup()
    {
      this.updateDataSource_ = (IDynamicDataSource) null;
      this.updates_ = (ArrayList) null;
      this.updateIndex_ = (Hashtable) null;
      if (this.archiveStorage_ == null)
        return;
      this.archiveStorage_.Dispose();
      this.archiveStorage_ = (IArchiveStorage) null;
    }

    private string GetTransformedFileName(string name)
    {
      INameTransform nameTransform = this.NameTransform;
      if (nameTransform == null)
        return name;
      return nameTransform.TransformFile(name);
    }

    private string GetTransformedDirectoryName(string name)
    {
      INameTransform nameTransform = this.NameTransform;
      if (nameTransform == null)
        return name;
      return nameTransform.TransformDirectory(name);
    }

    private byte[] GetBuffer()
    {
      if (this.copyBuffer_ == null)
        this.copyBuffer_ = new byte[this.bufferSize_];
      return this.copyBuffer_;
    }

    private void CopyDescriptorBytes(ZipFile.ZipUpdate update, Stream dest, Stream source)
    {
      int descriptorSize = this.GetDescriptorSize(update);
      if (descriptorSize <= 0)
        return;
      byte[] buffer = this.GetBuffer();
      while (descriptorSize > 0)
      {
        int count1 = Math.Min(buffer.Length, descriptorSize);
        int count2 = source.Read(buffer, 0, count1);
        if (count2 <= 0)
          throw new ZipException("Unxpected end of stream");
        dest.Write(buffer, 0, count2);
        descriptorSize -= count2;
      }
    }

    private void CopyBytes(ZipFile.ZipUpdate update, Stream destination, Stream source, long bytesToCopy, bool updateCrc)
    {
      if (destination == source)
        throw new InvalidOperationException("Destination and source are the same");
      Crc32 crc32 = new Crc32();
      byte[] buffer = this.GetBuffer();
      long num1 = bytesToCopy;
      long num2 = 0L;
      int count1;
      do
      {
        int count2 = buffer.Length;
        if (bytesToCopy < (long) count2)
          count2 = (int) bytesToCopy;
        count1 = source.Read(buffer, 0, count2);
        if (count1 > 0)
        {
          if (updateCrc)
            crc32.Update(buffer, 0, count1);
          destination.Write(buffer, 0, count1);
          bytesToCopy -= (long) count1;
          num2 += (long) count1;
        }
      }
      while (count1 > 0 && bytesToCopy > 0L);
      if (num2 != num1)
        throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", (object) num1, (object) num2));
      if (!updateCrc)
        return;
      update.OutEntry.Crc = crc32.Value;
    }

    private int GetDescriptorSize(ZipFile.ZipUpdate update)
    {
      int num = 0;
      if ((update.Entry.Flags & 8) != 0)
      {
        num = 12;
        if (update.Entry.LocalHeaderRequiresZip64)
          num = 20;
      }
      return num;
    }

    private void CopyDescriptorBytesDirect(ZipFile.ZipUpdate update, Stream stream, ref long destinationPosition, long sourcePosition)
    {
      int descriptorSize = this.GetDescriptorSize(update);
      while (descriptorSize > 0)
      {
        int count1 = descriptorSize;
        byte[] buffer = this.GetBuffer();
        stream.Position = sourcePosition;
        int count2 = stream.Read(buffer, 0, count1);
        if (count2 <= 0)
          throw new ZipException("Unxpected end of stream");
        stream.Position = destinationPosition;
        stream.Write(buffer, 0, count2);
        descriptorSize -= count2;
        destinationPosition += (long) count2;
        sourcePosition += (long) count2;
      }
    }

    private void CopyEntryDataDirect(ZipFile.ZipUpdate update, Stream stream, bool updateCrc, ref long destinationPosition, ref long sourcePosition)
    {
      long compressedSize = update.Entry.CompressedSize;
      Crc32 crc32 = new Crc32();
      byte[] buffer = this.GetBuffer();
      long num1 = compressedSize;
      long num2 = 0L;
      int count1;
      do
      {
        int count2 = buffer.Length;
        if (compressedSize < (long) count2)
          count2 = (int) compressedSize;
        stream.Position = sourcePosition;
        count1 = stream.Read(buffer, 0, count2);
        if (count1 > 0)
        {
          if (updateCrc)
            crc32.Update(buffer, 0, count1);
          stream.Position = destinationPosition;
          stream.Write(buffer, 0, count1);
          destinationPosition += (long) count1;
          sourcePosition += (long) count1;
          compressedSize -= (long) count1;
          num2 += (long) count1;
        }
      }
      while (count1 > 0 && compressedSize > 0L);
      if (num2 != num1)
        throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", (object) num1, (object) num2));
      if (!updateCrc)
        return;
      update.OutEntry.Crc = crc32.Value;
    }

    private int FindExistingUpdate(ZipEntry entry)
    {
      int num = -1;
      string transformedFileName = this.GetTransformedFileName(entry.Name);
      if (this.updateIndex_.ContainsKey((object) transformedFileName))
        num = (int) this.updateIndex_[(object) transformedFileName];
      return num;
    }

    private int FindExistingUpdate(string fileName)
    {
      int num = -1;
      string transformedFileName = this.GetTransformedFileName(fileName);
      if (this.updateIndex_.ContainsKey((object) transformedFileName))
        num = (int) this.updateIndex_[(object) transformedFileName];
      return num;
    }

    private Stream GetOutputStream(ZipEntry entry)
    {
      Stream stream = this.baseStream_;
      if (entry.IsCrypted)
        stream = this.CreateAndInitEncryptionStream(stream, entry);
      switch (entry.CompressionMethod)
      {
        case CompressionMethod.Stored:
          return (Stream) new ZipFile.UncompressedStream(stream);
        case CompressionMethod.Deflated:
          return (Stream) new DeflaterOutputStream(stream, new Deflater(9, true))
          {
            IsStreamOwner = false
          };
        default:
          throw new ZipException("Unknown compression method " + (object) entry.CompressionMethod);
      }
    }

    private void AddEntry(ZipFile workFile, ZipFile.ZipUpdate update)
    {
      Stream source = (Stream) null;
      if (update.Entry.IsFile)
        source = update.GetSource() ?? this.updateDataSource_.GetSource(update.Entry, update.Filename);
      if (source != null)
      {
        using (source)
        {
          long length = source.Length;
          if (update.OutEntry.Size < 0L)
            update.OutEntry.Size = length;
          else if (update.OutEntry.Size != length)
            throw new ZipException("Entry size/stream size mismatch");
          workFile.WriteLocalEntryHeader(update);
          long position1 = workFile.baseStream_.Position;
          using (Stream outputStream = workFile.GetOutputStream(update.OutEntry))
            this.CopyBytes(update, outputStream, source, length, true);
          long position2 = workFile.baseStream_.Position;
          update.OutEntry.CompressedSize = position2 - position1;
          if ((update.OutEntry.Flags & 8) != 8)
            return;
          new ZipHelperStream(workFile.baseStream_).WriteDataDescriptor(update.OutEntry);
        }
      }
      else
      {
        workFile.WriteLocalEntryHeader(update);
        update.OutEntry.CompressedSize = 0L;
      }
    }

    private void ModifyEntry(ZipFile workFile, ZipFile.ZipUpdate update)
    {
      workFile.WriteLocalEntryHeader(update);
      long position1 = workFile.baseStream_.Position;
      if (update.Entry.IsFile && update.Filename != null)
      {
        using (Stream outputStream = workFile.GetOutputStream(update.OutEntry))
        {
          using (Stream inputStream = this.GetInputStream(update.Entry))
            this.CopyBytes(update, outputStream, inputStream, inputStream.Length, true);
        }
      }
      long position2 = workFile.baseStream_.Position;
      update.Entry.CompressedSize = position2 - position1;
    }

    private void CopyEntryDirect(ZipFile workFile, ZipFile.ZipUpdate update, ref long destinationPosition)
    {
      bool flag = false;
      if (update.Entry.Offset == destinationPosition)
        flag = true;
      if (!flag)
      {
        this.baseStream_.Position = destinationPosition;
        workFile.WriteLocalEntryHeader(update);
        destinationPosition = this.baseStream_.Position;
      }
      long offset = update.Entry.Offset + 26L;
      this.baseStream_.Seek(offset, SeekOrigin.Begin);
      long sourcePosition = this.baseStream_.Position + (long) this.ReadLEUshort() + (long) this.ReadLEUshort();
      if (flag)
      {
        if (update.OffsetBasedSize != -1L)
          destinationPosition += update.OffsetBasedSize;
        else
          destinationPosition += sourcePosition - offset + 26L + update.Entry.CompressedSize + (long) this.GetDescriptorSize(update);
      }
      else
      {
        if (update.Entry.CompressedSize > 0L)
          this.CopyEntryDataDirect(update, this.baseStream_, false, ref destinationPosition, ref sourcePosition);
        this.CopyDescriptorBytesDirect(update, this.baseStream_, ref destinationPosition, sourcePosition);
      }
    }

    private void CopyEntry(ZipFile workFile, ZipFile.ZipUpdate update)
    {
      workFile.WriteLocalEntryHeader(update);
      if (update.Entry.CompressedSize > 0L)
      {
        this.baseStream_.Seek(update.Entry.Offset + 26L, SeekOrigin.Begin);
        this.baseStream_.Seek((long) ((uint) this.ReadLEUshort() + (uint) this.ReadLEUshort()), SeekOrigin.Current);
        this.CopyBytes(update, workFile.baseStream_, this.baseStream_, update.Entry.CompressedSize, false);
      }
      this.CopyDescriptorBytes(update, workFile.baseStream_, this.baseStream_);
    }

    private void Reopen(Stream source)
    {
      if (source == null)
        throw new ZipException("Failed to reopen archive - no source");
      this.isNewArchive_ = false;
      this.baseStream_ = source;
      this.ReadEntries();
    }

    private void Reopen()
    {
      if (this.Name == null)
        throw new InvalidOperationException("Name is not known cannot Reopen");
      this.Reopen((Stream) File.Open(this.Name, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    private void UpdateCommentOnly()
    {
      long length = this.baseStream_.Length;
      ZipHelperStream zipHelperStream;
      if (this.archiveStorage_.UpdateMode == FileUpdateMode.Safe)
      {
        zipHelperStream = new ZipHelperStream(this.archiveStorage_.MakeTemporaryCopy(this.baseStream_));
        zipHelperStream.IsStreamOwner = true;
        this.baseStream_.Close();
        this.baseStream_ = (Stream) null;
      }
      else if (this.archiveStorage_.UpdateMode == FileUpdateMode.Direct)
      {
        this.baseStream_ = this.archiveStorage_.OpenForDirectUpdate(this.baseStream_);
        zipHelperStream = new ZipHelperStream(this.baseStream_);
      }
      else
      {
        this.baseStream_.Close();
        this.baseStream_ = (Stream) null;
        zipHelperStream = new ZipHelperStream(this.Name);
      }
      using (zipHelperStream)
      {
        if (zipHelperStream.LocateBlockWithSignature(101010256, length, 22, (int) ushort.MaxValue) < 0L)
          throw new ZipException("Cannot find central directory");
        zipHelperStream.Position += 16L;
        byte[] rawComment = this.newComment_.RawComment;
        zipHelperStream.WriteLEShort(rawComment.Length);
        zipHelperStream.Write(rawComment, 0, rawComment.Length);
        zipHelperStream.SetLength(zipHelperStream.Position);
      }
      if (this.archiveStorage_.UpdateMode == FileUpdateMode.Safe)
        this.Reopen(this.archiveStorage_.ConvertTemporaryToFinal());
      else
        this.ReadEntries();
    }

    private void RunUpdates()
    {
      long sizeEntries = 0L;
      bool flag = false;
      long destinationPosition = 0L;
      ZipFile workFile;
      if (this.IsNewArchive)
      {
        workFile = this;
        workFile.baseStream_.Position = 0L;
        flag = true;
      }
      else if (this.archiveStorage_.UpdateMode == FileUpdateMode.Direct)
      {
        workFile = this;
        workFile.baseStream_.Position = 0L;
        flag = true;
        this.updates_.Sort((IComparer) new ZipFile.UpdateComparer());
      }
      else
      {
        workFile = ZipFile.Create(this.archiveStorage_.GetTemporaryOutput());
        workFile.UseZip64 = this.UseZip64;
        if (this.key != null)
          workFile.key = (byte[]) this.key.Clone();
      }
      long position1;
      try
      {
        foreach (ZipFile.ZipUpdate update in this.updates_)
        {
          if (update != null)
          {
            switch (update.Command)
            {
              case ZipFile.UpdateCommand.Copy:
                if (flag)
                {
                  this.CopyEntryDirect(workFile, update, ref destinationPosition);
                  continue;
                }
                this.CopyEntry(workFile, update);
                continue;
              case ZipFile.UpdateCommand.Modify:
                this.ModifyEntry(workFile, update);
                continue;
              case ZipFile.UpdateCommand.Add:
                if (!this.IsNewArchive && flag)
                  workFile.baseStream_.Position = destinationPosition;
                this.AddEntry(workFile, update);
                if (flag)
                {
                  destinationPosition = workFile.baseStream_.Position;
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        if (!this.IsNewArchive && flag)
          workFile.baseStream_.Position = destinationPosition;
        long position2 = workFile.baseStream_.Position;
        foreach (ZipFile.ZipUpdate zipUpdate in this.updates_)
        {
          if (zipUpdate != null)
            sizeEntries += (long) workFile.WriteCentralDirectoryHeader(zipUpdate.OutEntry);
        }
        byte[] comment = this.newComment_ != null ? this.newComment_.RawComment : ZipConstants.ConvertToArray(this.comment_);
        using (ZipHelperStream zipHelperStream = new ZipHelperStream(workFile.baseStream_))
          zipHelperStream.WriteEndOfCentralDirectory(this.updateCount_, sizeEntries, position2, comment);
        position1 = workFile.baseStream_.Position;
        foreach (ZipFile.ZipUpdate zipUpdate in this.updates_)
        {
          if (zipUpdate != null)
          {
            if (zipUpdate.CrcPatchOffset > 0L && zipUpdate.OutEntry.CompressedSize > 0L)
            {
              workFile.baseStream_.Position = zipUpdate.CrcPatchOffset;
              workFile.WriteLEInt((int) zipUpdate.OutEntry.Crc);
            }
            if (zipUpdate.SizePatchOffset > 0L)
            {
              workFile.baseStream_.Position = zipUpdate.SizePatchOffset;
              if (zipUpdate.OutEntry.LocalHeaderRequiresZip64)
              {
                workFile.WriteLeLong(zipUpdate.OutEntry.Size);
                workFile.WriteLeLong(zipUpdate.OutEntry.CompressedSize);
              }
              else
              {
                workFile.WriteLEInt((int) zipUpdate.OutEntry.CompressedSize);
                workFile.WriteLEInt((int) zipUpdate.OutEntry.Size);
              }
            }
          }
        }
      }
      catch
      {
        workFile.Close();
        if (!flag && workFile.Name != null)
          File.Delete(workFile.Name);
        throw;
      }
      if (flag)
      {
        workFile.baseStream_.SetLength(position1);
        workFile.baseStream_.Flush();
        this.isNewArchive_ = false;
        this.ReadEntries();
      }
      else
      {
        this.baseStream_.Close();
        this.Reopen(this.archiveStorage_.ConvertTemporaryToFinal());
      }
    }

    private void CheckUpdating()
    {
      if (this.updates_ == null)
        throw new InvalidOperationException("BeginUpdate has not been called");
    }

    void IDisposable.Dispose()
    {
      this.Close();
    }

    private void DisposeInternal(bool disposing)
    {
      if (this.isDisposed_)
        return;
      this.isDisposed_ = true;
      this.entries_ = new ZipEntry[0];
      if (this.IsStreamOwner && this.baseStream_ != null)
      {
        lock (this.baseStream_)
          this.baseStream_.Close();
      }
      this.PostUpdateCleanup();
    }

    protected virtual void Dispose(bool disposing)
    {
      this.DisposeInternal(disposing);
    }

    private ushort ReadLEUshort()
    {
      int num1 = this.baseStream_.ReadByte();
      if (num1 < 0)
        throw new EndOfStreamException("End of stream");
      int num2 = this.baseStream_.ReadByte();
      if (num2 < 0)
        throw new EndOfStreamException("End of stream");
      return (ushort) ((uint) (ushort) num1 | (uint) (ushort) (num2 << 8));
    }

    private uint ReadLEUint()
    {
      return (uint) this.ReadLEUshort() | (uint) this.ReadLEUshort() << 16;
    }

    private ulong ReadLEUlong()
    {
      return (ulong) this.ReadLEUint() | (ulong) this.ReadLEUint() << 32;
    }

    private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
    {
      using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_))
        return zipHelperStream.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
    }

    private void ReadEntries()
    {
      if (!this.baseStream_.CanSeek)
        throw new ZipException("ZipFile stream must be seekable");
      long endLocation = this.LocateBlockWithSignature(101010256, this.baseStream_.Length, 22, (int) ushort.MaxValue);
      if (endLocation < 0L)
        throw new ZipException("Cannot find central directory");
      else
        this.comment_ = string.Empty;
      bool flag = false;
      {
        flag = true;
        if (this.LocateBlockWithSignature(117853008, endLocation, 0, 4096) < 0L)
          throw new ZipException("Cannot find Zip64 locator");
        int num7 = (int) this.ReadLEUint();
        ulong num8 = this.ReadLEUlong();
        int num9 = (int) this.ReadLEUint();
        this.baseStream_.Position = (long) num8;
        if ((long) this.ReadLEUint() != 101075792L)
          throw new ZipException(string.Format("Invalid Zip64 Central directory signature at {0:X}", (object) num8));
        long num10 = (long) this.ReadLEUlong();
        int num11 = (int) this.ReadLEUshort();
        int num12 = (int) this.ReadLEUshort();
        int num13 = (int) this.ReadLEUint();
        int num14 = (int) this.ReadLEUint();
        this.ReadLEUlong();
      }
      {
        this.offsetOfFirstEntry = endLocation;
        if (this.offsetOfFirstEntry <= 0L)
          throw new ZipException("Invalid embedded zip archive");
      }
      {
        if ((int) this.ReadLEUint() != 33639248)
          throw new ZipException("Wrong Central Directory signature");
        int madeByInfo = (int) this.ReadLEUshort();
        int versionRequiredToExtract = (int) this.ReadLEUshort();
        int flags = (int) this.ReadLEUshort();
        int num7 = (int) this.ReadLEUshort();
        uint num8 = this.ReadLEUint();
        uint num9 = this.ReadLEUint();
        long num10 = (long) this.ReadLEUint();
        long num11 = (long) this.ReadLEUint();
        int num12 = (int) this.ReadLEUshort();
        int length2 = (int) this.ReadLEUshort();
        int num13 = (int) this.ReadLEUshort();
        int num14 = (int) this.ReadLEUshort();
        int num15 = (int) this.ReadLEUshort();
        uint num16 = this.ReadLEUint();
        long num17 = (long) this.ReadLEUint();
        byte[] numArray = new byte[Math.Max(num12, num13)];
        StreamUtils.ReadFully(this.baseStream_, numArray, 0, num12);
        ZipEntry zipEntry = new ZipEntry(ZipConstants.ConvertToStringExt(flags, numArray, num12), versionRequiredToExtract, madeByInfo, (CompressionMethod) num7);
        zipEntry.Crc = (long) num9 & (long) uint.MaxValue;
        zipEntry.Size = num11 & (long) uint.MaxValue;
        zipEntry.CompressedSize = num10 & (long) uint.MaxValue;
        zipEntry.Flags = flags;
        zipEntry.DosTime = (long) num8;
        zipEntry.Offset = num17;
        zipEntry.ExternalFileAttributes = (int) num16;
        zipEntry.CryptoCheckValue = (flags & 8) != 0 ? (byte) (num8 >> 8 & (uint) byte.MaxValue) : (byte) (num9 >> 24);
        if (length2 > 0)
        {
          byte[] buffer = new byte[length2];
          StreamUtils.ReadFully(this.baseStream_, buffer);
          zipEntry.ExtraData = buffer;
        }
        zipEntry.ProcessExtraData(false);
        if (num13 > 0)
        {
          StreamUtils.ReadFully(this.baseStream_, numArray, 0, num13);
          zipEntry.Comment = ZipConstants.ConvertToStringExt(flags, numArray, num13);
        }
      }
    }

    private long LocateEntry(ZipEntry entry)
    {
      return this.TestLocalHeader(entry, ZipFile.HeaderTest.Extract);
    }

    private Stream CreateAndInitDecryptionStream(Stream baseStream, ZipEntry entry)
    {
      CryptoStream classicCryptoStream;
      if (entry.Version < 50 || (entry.Flags & 64) == 0)
      {
        PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
        this.OnKeysRequired(entry.Name);
        if (!this.HaveKeys)
          throw new ZipException("No password available for encrypted stream");
        classicCryptoStream = new CryptoStream(baseStream, pkzipClassicManaged.CreateDecryptor(this.key, (byte[]) null), CryptoStreamMode.Read);
        ZipFile.CheckClassicPassword(classicCryptoStream, entry);
      }
      else
      {
        if (entry.Version != 51)
          throw new ZipException("Decryption method not supported");
        this.OnKeysRequired(entry.Name);
        if (!this.HaveKeys)
          throw new ZipException("No password available for AES encrypted stream");
        int aesSaltLen = entry.AESSaltLen;
        byte[] numArray = new byte[aesSaltLen];
        int num = baseStream.Read(numArray, 0, aesSaltLen);
        if (num != aesSaltLen)
          throw new ZipException(string.Concat(new object[4]
          {
            (object) "AES Salt expected ",
            (object) aesSaltLen,
            (object) " got ",
            (object) num
          }));
        byte[] buffer = new byte[2];
        baseStream.Read(buffer, 0, 2);
        int blockSize = entry.AESKeySize / 8;
        ZipAESTransform transform = new ZipAESTransform(this.rawPassword_, numArray, blockSize, false);
        byte[] pwdVerifier = transform.PwdVerifier;
        if ((int) pwdVerifier[0] != (int) buffer[0] || (int) pwdVerifier[1] != (int) buffer[1])
          throw new Exception("Invalid password for AES");
        classicCryptoStream = (CryptoStream) new ZipAESStream(baseStream, transform, CryptoStreamMode.Read);
      }
      return (Stream) classicCryptoStream;
    }

    private Stream CreateAndInitEncryptionStream(Stream baseStream, ZipEntry entry)
    {
      CryptoStream cryptoStream = (CryptoStream) null;
      if (entry.Version < 50 || (entry.Flags & 64) == 0)
      {
        PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
        this.OnKeysRequired(entry.Name);
        if (!this.HaveKeys)
          throw new ZipException("No password available for encrypted stream");
        cryptoStream = new CryptoStream((Stream) new ZipFile.UncompressedStream(baseStream), pkzipClassicManaged.CreateEncryptor(this.key, (byte[]) null), CryptoStreamMode.Write);
        if (entry.Crc < 0L || (entry.Flags & 8) != 0)
          ZipFile.WriteEncryptionHeader((Stream) cryptoStream, entry.DosTime << 16);
        else
          ZipFile.WriteEncryptionHeader((Stream) cryptoStream, entry.Crc);
      }
      return (Stream) cryptoStream;
    }

    private static void CheckClassicPassword(CryptoStream classicCryptoStream, ZipEntry entry)
    {
      byte[] buffer = new byte[12];
      StreamUtils.ReadFully((Stream) classicCryptoStream, buffer);
      if ((int) buffer[11] != (int) entry.CryptoCheckValue)
        throw new ZipException("Invalid password");
    }

    private static void WriteEncryptionHeader(Stream stream, long crcValue)
    {
      byte[] buffer = new byte[12];
      new Random().NextBytes(buffer);
      buffer[11] = (byte) (crcValue >> 24);
      stream.Write(buffer, 0, buffer.Length);
    }

    public delegate void KeysRequiredEventHandler(object sender, KeysRequiredEventArgs e);

    [Flags]
    private enum HeaderTest
    {
      Extract = 1,
      Header = 2,
    }

    private enum UpdateCommand
    {
      Copy,
      Modify,
      Add,
    }

    private class UpdateComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        ZipFile.ZipUpdate zipUpdate1 = x as ZipFile.ZipUpdate;
        ZipFile.ZipUpdate zipUpdate2 = y as ZipFile.ZipUpdate;
        int num1;
        if (zipUpdate1 == null)
          num1 = zipUpdate2 != null ? -1 : 0;
        else if (zipUpdate2 == null)
        {
          num1 = 1;
        }
        else
        {
          num1 = (zipUpdate1.Command == ZipFile.UpdateCommand.Copy || zipUpdate1.Command == ZipFile.UpdateCommand.Modify ? 0 : 1) - (zipUpdate2.Command == ZipFile.UpdateCommand.Copy || zipUpdate2.Command == ZipFile.UpdateCommand.Modify ? 0 : 1);
          if (num1 == 0)
          {
            long num2 = zipUpdate1.Entry.Offset - zipUpdate2.Entry.Offset;
            num1 = num2 >= 0L ? (num2 != 0L ? 1 : 0) : -1;
          }
        }
        return num1;
      }
    }

    private class ZipUpdate
    {
      private long sizePatchOffset_ = -1L;
      private long crcPatchOffset_ = -1L;
      private long _offsetBasedSize = -1L;
      private ZipEntry entry_;
      private ZipEntry outEntry_;
      private ZipFile.UpdateCommand command_;
      private IStaticDataSource dataSource_;
      private string filename_;

      public ZipEntry Entry
      {
        get
        {
          return this.entry_;
        }
      }

      public ZipEntry OutEntry
      {
        get
        {
          if (this.outEntry_ == null)
            this.outEntry_ = (ZipEntry) this.entry_.Clone();
          return this.outEntry_;
        }
      }

      public ZipFile.UpdateCommand Command
      {
        get
        {
          return this.command_;
        }
      }

      public string Filename
      {
        get
        {
          return this.filename_;
        }
      }

      public long SizePatchOffset
      {
        get
        {
          return this.sizePatchOffset_;
        }
        set
        {
          this.sizePatchOffset_ = value;
        }
      }

      public long CrcPatchOffset
      {
        get
        {
          return this.crcPatchOffset_;
        }
        set
        {
          this.crcPatchOffset_ = value;
        }
      }

      public long OffsetBasedSize
      {
        get
        {
          return this._offsetBasedSize;
        }
        set
        {
          this._offsetBasedSize = value;
        }
      }

      public ZipUpdate(string fileName, ZipEntry entry)
      {
        this.command_ = ZipFile.UpdateCommand.Add;
        this.entry_ = entry;
        this.filename_ = fileName;
      }

      [Obsolete]
      public ZipUpdate(string fileName, string entryName, CompressionMethod compressionMethod)
      {
        this.command_ = ZipFile.UpdateCommand.Add;
        this.entry_ = new ZipEntry(entryName);
        this.entry_.CompressionMethod = compressionMethod;
        this.filename_ = fileName;
      }

      [Obsolete]
      public ZipUpdate(string fileName, string entryName)
        : this(fileName, entryName, CompressionMethod.Deflated)
      {
      }

      [Obsolete]
      public ZipUpdate(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
      {
        this.command_ = ZipFile.UpdateCommand.Add;
        this.entry_ = new ZipEntry(entryName);
        this.entry_.CompressionMethod = compressionMethod;
        this.dataSource_ = dataSource;
      }

      public ZipUpdate(IStaticDataSource dataSource, ZipEntry entry)
      {
        this.command_ = ZipFile.UpdateCommand.Add;
        this.entry_ = entry;
        this.dataSource_ = dataSource;
      }

      public ZipUpdate(ZipEntry original, ZipEntry updated)
      {
        throw new ZipException("Modify not currently supported");
      }

      public ZipUpdate(ZipFile.UpdateCommand command, ZipEntry entry)
      {
        this.command_ = command;
        this.entry_ = (ZipEntry) entry.Clone();
      }

      public ZipUpdate(ZipEntry entry)
        : this(ZipFile.UpdateCommand.Copy, entry)
      {
      }

      public Stream GetSource()
      {
        Stream stream = (Stream) null;
        if (this.dataSource_ != null)
          stream = this.dataSource_.GetSource();
        return stream;
      }
    }

    private class ZipString
    {
      private string comment_;
      private byte[] rawComment_;
      private bool isSourceString_;

      public bool IsSourceString
      {
        get
        {
          return this.isSourceString_;
        }
      }

      public int RawLength
      {
        get
        {
          this.MakeBytesAvailable();
          return this.rawComment_.Length;
        }
      }

      public byte[] RawComment
      {
        get
        {
          this.MakeBytesAvailable();
          return (byte[]) this.rawComment_.Clone();
        }
      }

      public ZipString(string comment)
      {
        this.comment_ = comment;
        this.isSourceString_ = true;
      }

      public ZipString(byte[] rawString)
      {
        this.rawComment_ = rawString;
      }

      public static implicit operator string(ZipFile.ZipString zipString)
      {
        zipString.MakeTextAvailable();
        return zipString.comment_;
      }

      public void Reset()
      {
        if (this.isSourceString_)
          this.rawComment_ = (byte[]) null;
        else
          this.comment_ = (string) null;
      }

      private void MakeTextAvailable()
      {
        if (this.comment_ != null)
          return;
        this.comment_ = ZipConstants.ConvertToString(this.rawComment_);
      }

      private void MakeBytesAvailable()
      {
        if (this.rawComment_ != null)
          return;
        this.rawComment_ = ZipConstants.ConvertToArray(this.comment_);
      }
    }

    private class ZipEntryEnumerator : IEnumerator
    {
      private int index = -1;
      private ZipEntry[] array;

      public object Current
      {
        get
        {
          return (object) this.array[this.index];
        }
      }

      public ZipEntryEnumerator(ZipEntry[] entries)
      {
        this.array = entries;
      }

      public void Reset()
      {
        this.index = -1;
      }

      public bool MoveNext()
      {
        return ++this.index < this.array.Length;
      }
    }

    private class UncompressedStream : Stream
    {
      private Stream baseStream_;

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
          return this.baseStream_.CanWrite;
        }
      }

      public override bool CanSeek
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
          return 0L;
        }
      }

      public override long Position
      {
        get
        {
          return this.baseStream_.Position;
        }
        set
        {
        }
      }

      public UncompressedStream(Stream baseStream)
      {
        this.baseStream_ = baseStream;
      }

      public override void Close()
      {
      }

      public override void Flush()
      {
        this.baseStream_.Flush();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return 0;
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return 0L;
      }

      public override void SetLength(long value)
      {
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.baseStream_.Write(buffer, offset, count);
      }
    }

    private class PartialInputStream : Stream
    {
      private ZipFile zipFile_;
      private Stream baseStream_;
      private long start_;
      private long length_;
      private long readPos_;
      private long end_;

      public override long Position
      {
        get
        {
          return this.readPos_ - this.start_;
        }
        set
        {
          long num = this.start_ + value;
          if (num < this.start_)
            throw new ArgumentException("Negative position is invalid");
          if (num >= this.end_)
            throw new InvalidOperationException("Cannot seek past end");
          this.readPos_ = num;
        }
      }

      public override long Length
      {
        get
        {
          return this.length_;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return false;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return true;
        }
      }

      public override bool CanRead
      {
        get
        {
          return true;
        }
      }

      public override bool CanTimeout
      {
        get
        {
          return this.baseStream_.CanTimeout;
        }
      }

      public PartialInputStream(ZipFile zipFile, long start, long length)
      {
        this.start_ = start;
        this.length_ = length;
        this.zipFile_ = zipFile;
        this.baseStream_ = this.zipFile_.baseStream_;
        this.readPos_ = start;
        this.end_ = start + length;
      }

      public override int ReadByte()
      {
        if (this.readPos_ >= this.end_)
          return -1;
        lock (this.baseStream_)
        {
          this.baseStream_.Seek(this.readPos_++, SeekOrigin.Begin);
          return this.baseStream_.ReadByte();
        }
      }

      public override void Close()
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        lock (this.baseStream_)
        {
          if ((long) count > this.end_ - this.readPos_)
          {
            count = (int) (this.end_ - this.readPos_);
            if (count == 0)
              return 0;
          }
          this.baseStream_.Seek(this.readPos_, SeekOrigin.Begin);
          int local_0 = this.baseStream_.Read(buffer, offset, count);
          if (local_0 > 0)
            this.readPos_ += (long) local_0;
          return local_0;
        }
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override void SetLength(long value)
      {
        throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        long num = this.readPos_;
        switch (origin)
        {
          case SeekOrigin.Begin:
            num = this.start_ + offset;
            break;
          case SeekOrigin.Current:
            num = this.readPos_ + offset;
            break;
          case SeekOrigin.End:
            num = this.end_ + offset;
            break;
        }
        if (num < this.start_)
          throw new ArgumentException("Negative position is invalid");
        if (num >= this.end_)
          throw new IOException("Cannot seek past end");
        this.readPos_ = num;
        return this.readPos_;
      }

      public override void Flush()
      {
      }
    }
  }
}
