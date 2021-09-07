
// Type: Ionic.Zip.ZipEntry


// Hacked by SystemAce

using Ionic.Crc;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Ionic.Zip
{
  [Guid("ebc25cf6-9120-4283-b972-0e5520d00004")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  public class ZipEntry
  {
    private static Encoding ibm437 = Encoding.GetEncoding("IBM437");
    private static DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static DateTime _win32Epoch = DateTime.FromFileTimeUtc(0L);
    private static DateTime _zeroHour = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private bool _emitNtfsTimes = true;
    private bool _TrimVolumeFromFullyQualifiedPaths = true;
    private long __FileDataPosition = -1L;
    private object _outputLock = new object();
    private short _VersionMadeBy;
    private short _InternalFileAttrs;
    private int _ExternalFileAttrs;
    private short _filenameLength;
    private short _extraFieldLength;
    private short _commentLength;
    private ZipCrypto _zipCrypto_forExtract;
    private ZipCrypto _zipCrypto_forWrite;
    internal DateTime _LastModified;
    private DateTime _Mtime;
    private DateTime _Atime;
    private DateTime _Ctime;
    private bool _ntfsTimesAreSet;
    private bool _emitUnixTimes;
    internal string _LocalFileName;
    private string _FileNameInArchive;
    internal short _VersionNeeded;
    internal short _BitField;
    internal short _CompressionMethod;
    private short _CompressionMethod_FromZipFile;
    private CompressionLevel _CompressionLevel;
    internal string _Comment;
    private bool _IsDirectory;
    private byte[] _CommentBytes;
    internal long _CompressedSize;
    internal long _CompressedFileDataSize;
    internal long _UncompressedSize;
    internal int _TimeBlob;
    private bool _crcCalculated;
    internal int _Crc32;
    internal byte[] _Extra;
    private bool _metadataChanged;
    private bool _restreamRequiredOnSave;
    private bool _sourceIsEncrypted;
    private bool _skippedDuringSave;
    private uint _diskNumber;
    private Encoding _actualEncoding;
    internal ZipContainer _container;
    private byte[] _EntryHeader;
    internal long _RelativeOffsetOfLocalHeader;
    private long _future_ROLH;
    private long _TotalEntrySize;
    private int _LengthOfHeader;
    private int _LengthOfTrailer;
    internal bool _InputUsesZip64;
    private uint _UnsupportedAlgorithmId;
    internal string _Password;
    internal ZipEntrySource _Source;
    internal EncryptionAlgorithm _Encryption;
    internal EncryptionAlgorithm _Encryption_FromZipFile;
    internal byte[] _WeakEncryptionHeader;
    internal Stream _archiveStream;
    private Stream _sourceStream;
    private long? _sourceStreamOriginalPosition;
    private bool _sourceWasJitProvided;
    private bool _ioOperationCanceled;
    private bool _presumeZip64;
    private bool? _entryRequiresZip64;
    private bool? _OutputUsesZip64;
    private bool _IsText;
    private ZipEntryTimestamp _timestamp;
    private WriteDelegate _WriteDelegate;
    private OpenDelegate _OpenDelegate;
    private CloseDelegate _CloseDelegate;
    private Stream _inputDecryptorStream;
    private int _readExtraDepth;

    internal bool AttributesIndicateDirectory
    {
      get
      {
        if ((int) this._InternalFileAttrs == 0)
          return (this._ExternalFileAttrs & 16) == 16;
        return false;
      }
    }

    public string Info
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(string.Format("          ZipEntry: {0}\n", (object) this.FileName)).Append(string.Format("   Version Made By: {0}\n", (object) this._VersionMadeBy)).Append(string.Format(" Needed to extract: {0}\n", (object) this.VersionNeeded));
        if (this._IsDirectory)
          stringBuilder.Append("        Entry type: directory\n");
        else
          stringBuilder.Append(string.Format("         File type: {0}\n", this._IsText ? (object) "text" : (object) "binary")).Append(string.Format("       Compression: {0}\n", (object) this.CompressionMethod)).Append(string.Format("        Compressed: 0x{0:X}\n", (object) this.CompressedSize)).Append(string.Format("      Uncompressed: 0x{0:X}\n", (object) this.UncompressedSize)).Append(string.Format("             CRC32: 0x{0:X8}\n", (object) this._Crc32));
        stringBuilder.Append(string.Format("       Disk Number: {0}\n", (object) this._diskNumber));
        if (this._RelativeOffsetOfLocalHeader > (long) uint.MaxValue)
          stringBuilder.Append(string.Format("   Relative Offset: 0x{0:X16}\n", (object) this._RelativeOffsetOfLocalHeader));
        else
          stringBuilder.Append(string.Format("   Relative Offset: 0x{0:X8}\n", (object) this._RelativeOffsetOfLocalHeader));
        if (!string.IsNullOrEmpty(this._Comment))
          stringBuilder.Append(string.Format("           Comment: {0}\n", (object) this._Comment));
        stringBuilder.Append("\n");
        return stringBuilder.ToString();
      }
    }

    public DateTime LastModified
    {
      get
      {
        return this._LastModified.ToLocalTime();
      }
      set
      {
        this._LastModified = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Local) : value.ToLocalTime();
        this._Mtime = SharedUtilities.AdjustTime_Reverse(this._LastModified).ToUniversalTime();
        this._metadataChanged = true;
      }
    }

    private int BufferSize
    {
      get
      {
        return this._container.BufferSize;
      }
    }

    public DateTime ModifiedTime
    {
      get
      {
        return this._Mtime;
      }
      set
      {
        this.SetEntryTimes(this._Ctime, this._Atime, value);
      }
    }

    public DateTime AccessedTime
    {
      get
      {
        return this._Atime;
      }
      set
      {
        this.SetEntryTimes(this._Ctime, value, this._Mtime);
      }
    }

    public DateTime CreationTime
    {
      get
      {
        return this._Ctime;
      }
      set
      {
        this.SetEntryTimes(value, this._Atime, this._Mtime);
      }
    }

    public bool EmitTimesInWindowsFormatWhenSaving
    {
      get
      {
        return this._emitNtfsTimes;
      }
      set
      {
        this._emitNtfsTimes = value;
        this._metadataChanged = true;
      }
    }

    public bool EmitTimesInUnixFormatWhenSaving
    {
      get
      {
        return this._emitUnixTimes;
      }
      set
      {
        this._emitUnixTimes = value;
        this._metadataChanged = true;
      }
    }

    public ZipEntryTimestamp Timestamp
    {
      get
      {
        return this._timestamp;
      }
    }

    public FileAttributes Attributes
    {
      get
      {
        return (FileAttributes) this._ExternalFileAttrs;
      }
      set
      {
        this._ExternalFileAttrs = (int) value;
        this._VersionMadeBy = (short) 45;
        this._metadataChanged = true;
      }
    }

    internal string LocalFileName
    {
      get
      {
        return this._LocalFileName;
      }
    }

    public string FileName
    {
      get
      {
        return this._FileNameInArchive;
      }
      set
      {
        if (this._container.ZipFile == null)
          throw new ZipException("Cannot rename; this is not supported in ZipOutputStream/ZipInputStream.");
        if (string.IsNullOrEmpty(value))
          throw new ZipException("The FileName must be non empty and non-null.");
        string name = ZipEntry.NameInArchive(value, (string) null);
        if (this._FileNameInArchive == name)
          return;
        this._container.ZipFile.RemoveEntry(this);
        this._container.ZipFile.InternalAddEntry(name, this);
        this._FileNameInArchive = name;
        this._container.ZipFile.NotifyEntryChanged();
        this._metadataChanged = true;
      }
    }

    public Stream InputStream
    {
      get
      {
        return this._sourceStream;
      }
      set
      {
        if (this._Source != ZipEntrySource.Stream)
          throw new ZipException("You must not set the input stream for this entry.");
        this._sourceWasJitProvided = true;
        this._sourceStream = value;
      }
    }

    public bool InputStreamWasJitProvided
    {
      get
      {
        return this._sourceWasJitProvided;
      }
    }

    public ZipEntrySource Source
    {
      get
      {
        return this._Source;
      }
    }

    public short VersionNeeded
    {
      get
      {
        return this._VersionNeeded;
      }
    }

    public string Comment
    {
      get
      {
        return this._Comment;
      }
      set
      {
        this._Comment = value;
        this._metadataChanged = true;
      }
    }

    public bool? RequiresZip64
    {
      get
      {
        return this._entryRequiresZip64;
      }
    }

    public bool? OutputUsedZip64
    {
      get
      {
        return this._OutputUsesZip64;
      }
    }

    public short BitField
    {
      get
      {
        return this._BitField;
      }
    }

    public CompressionMethod CompressionMethod
    {
      get
      {
        return (CompressionMethod) this._CompressionMethod;
      }
      set
      {
        if (value == (CompressionMethod) this._CompressionMethod)
          return;
        if (value != CompressionMethod.None && value != CompressionMethod.Deflate)
          throw new InvalidOperationException("Unsupported compression method.");
        this._CompressionMethod = (short) value;
        if ((int) this._CompressionMethod == 0)
          this._CompressionLevel = CompressionLevel.None;
        else if (this.CompressionLevel == CompressionLevel.None)
          this._CompressionLevel = CompressionLevel.Default;
        if (this._container.ZipFile != null)
          this._container.ZipFile.NotifyEntryChanged();
        this._restreamRequiredOnSave = true;
      }
    }

    public CompressionLevel CompressionLevel
    {
      get
      {
        return this._CompressionLevel;
      }
      set
      {
        if ((int) this._CompressionMethod != 8 && (int) this._CompressionMethod != 0 || value == CompressionLevel.Default && (int) this._CompressionMethod == 8)
          return;
        this._CompressionLevel = value;
        if (value == CompressionLevel.None && (int) this._CompressionMethod == 0)
          return;
        this._CompressionMethod = this._CompressionLevel != CompressionLevel.None ? (short) 8 : (short) 0;
        if (this._container.ZipFile != null)
          this._container.ZipFile.NotifyEntryChanged();
        this._restreamRequiredOnSave = true;
      }
    }

    public long CompressedSize
    {
      get
      {
        return this._CompressedSize;
      }
    }

    public long UncompressedSize
    {
      get
      {
        return this._UncompressedSize;
      }
    }

    public double CompressionRatio
    {
      get
      {
        if (this.UncompressedSize == 0L)
          return 0.0;
        return 100.0 * (1.0 - 1.0 * (double) this.CompressedSize / (1.0 * (double) this.UncompressedSize));
      }
    }

    public int Crc
    {
      get
      {
        return this._Crc32;
      }
    }

    public bool IsDirectory
    {
      get
      {
        return this._IsDirectory;
      }
    }

    public bool UsesEncryption
    {
      get
      {
        return this._Encryption_FromZipFile != EncryptionAlgorithm.None;
      }
    }

    public EncryptionAlgorithm Encryption
    {
      get
      {
        return this._Encryption;
      }
      set
      {
        if (value == this._Encryption)
          return;
        if (value == EncryptionAlgorithm.Unsupported)
          throw new InvalidOperationException("You may not set Encryption to that value.");
        this._Encryption = value;
        this._restreamRequiredOnSave = true;
        if (this._container.ZipFile == null)
          return;
        this._container.ZipFile.NotifyEntryChanged();
      }
    }

    public string Password
    {
      private get
      {
        return this._Password;
      }
      set
      {
        this._Password = value;
        if (this._Password == null)
        {
          this._Encryption = EncryptionAlgorithm.None;
        }
        else
        {
          if (this._Source == ZipEntrySource.ZipFile && !this._sourceIsEncrypted)
            this._restreamRequiredOnSave = true;
          if (this.Encryption != EncryptionAlgorithm.None)
            return;
          this._Encryption = EncryptionAlgorithm.PkzipWeak;
        }
      }
    }

    internal bool IsChanged
    {
      get
      {
        return this._restreamRequiredOnSave | this._metadataChanged;
      }
    }

    public ExtractExistingFileAction ExtractExistingFile { get; set; }

    public ZipErrorAction ZipErrorAction { get; set; }

    public bool IncludedInMostRecentSave
    {
      get
      {
        return !this._skippedDuringSave;
      }
    }

    public SetCompressionCallback SetCompression { get; set; }

    [Obsolete("Beginning with v1.9.1.6 of DotNetZip, this property is obsolete.  It will be removed in a future version of the library. Your applications should  use AlternateEncoding and AlternateEncodingUsage instead.")]
    public bool UseUnicodeAsNecessary
    {
      get
      {
        if (this.AlternateEncoding == Encoding.GetEncoding("UTF-8"))
          return this.AlternateEncodingUsage == ZipOption.AsNecessary;
        return false;
      }
      set
      {
        if (value)
        {
          this.AlternateEncoding = Encoding.GetEncoding("UTF-8");
          this.AlternateEncodingUsage = ZipOption.AsNecessary;
        }
        else
        {
          this.AlternateEncoding = ZipFile.DefaultEncoding;
          this.AlternateEncodingUsage = ZipOption.Default;
        }
      }
    }

    [Obsolete("This property is obsolete since v1.9.1.6. Use AlternateEncoding and AlternateEncodingUsage instead.", true)]
    public Encoding ProvisionalAlternateEncoding { get; set; }

    public Encoding AlternateEncoding { get; set; }

    public ZipOption AlternateEncodingUsage { get; set; }

    public bool IsText
    {
      get
      {
        return this._IsText;
      }
      set
      {
        this._IsText = value;
      }
    }

    internal Stream ArchiveStream
    {
      get
      {
        if (this._archiveStream == null)
        {
          if (this._container.ZipFile != null)
          {
            ZipFile zipFile = this._container.ZipFile;
            zipFile.Reset(false);
            this._archiveStream = zipFile.StreamForDiskNumber(this._diskNumber);
          }
          else
            this._archiveStream = this._container.ZipOutputStream.OutputStream;
        }
        return this._archiveStream;
      }
    }

    internal long FileDataPosition
    {
      get
      {
        if (this.__FileDataPosition == -1L)
          this.SetFdpLoh();
        return this.__FileDataPosition;
      }
    }

    private int LengthOfHeader
    {
      get
      {
        if (this._LengthOfHeader == 0)
          this.SetFdpLoh();
        return this._LengthOfHeader;
      }
    }

    private string UnsupportedAlgorithm
    {
      get
      {
        string str1 = string.Empty;
        uint num = this._UnsupportedAlgorithmId;
        string str2;
        if (num <= 26128U)
        {
          if (num <= 26115U)
          {
            switch (num)
            {
              case 0U:
                str2 = "--";
                goto label_21;
              case 26113U:
                str2 = "DES";
                goto label_21;
              case 26114U:
                str2 = "RC2";
                goto label_21;
              case 26115U:
                str2 = "3DES-168";
                goto label_21;
            }
          }
          else
          {
            switch (num)
            {
              case 26121U:
                str2 = "3DES-112";
                goto label_21;
              case 26126U:
                str2 = "PKWare AES128";
                goto label_21;
              case 26127U:
                str2 = "PKWare AES192";
                goto label_21;
              case 26128U:
                str2 = "PKWare AES256";
                goto label_21;
            }
          }
        }
        else if (num <= 26401U)
        {
          switch (num)
          {
            case 26370U:
              str2 = "RC2";
              goto label_21;
            case 26400U:
              str2 = "Blowfish";
              goto label_21;
            case 26401U:
              str2 = "Twofish";
              goto label_21;
          }
        }
        else if ((int) num != 26625)
        {
          if ((int) num == (int) ushort.MaxValue)
            ;
        }
        else
        {
          str2 = "RC4";
          goto label_21;
        }
        str2 = string.Format("Unknown (0x{0:X4})", (object) this._UnsupportedAlgorithmId);
label_21:
        return str2;
      }
    }

    private string UnsupportedCompressionMethod
    {
      get
      {
        string str1 = string.Empty;
        string str2;
        switch (this._CompressionMethod)
        {
          case (short) 19:
            str2 = "LZ77";
            break;
          case (short) 98:
            str2 = "PPMd";
            break;
          case (short) 0:
            str2 = "Store";
            break;
          case (short) 1:
            str2 = "Shrink";
            break;
          case (short) 8:
            str2 = "DEFLATE";
            break;
          case (short) 9:
            str2 = "Deflate64";
            break;
          case (short) 12:
            str2 = "BZIP2";
            break;
          case (short) 14:
            str2 = "LZMA";
            break;
          default:
            str2 = string.Format("Unknown (0x{0:X4})", (object) this._CompressionMethod);
            break;
        }
        return str2;
      }
    }

    public ZipEntry()
    {
      this._CompressionMethod = (short) 8;
      this._CompressionLevel = CompressionLevel.Default;
      this._Encryption = EncryptionAlgorithm.None;
      this._Source = ZipEntrySource.None;
      this.AlternateEncoding = Encoding.GetEncoding("IBM437");
      this.AlternateEncodingUsage = ZipOption.Default;
    }

    internal void ResetDirEntry()
    {
      this.__FileDataPosition = -1L;
      this._LengthOfHeader = 0;
    }

    internal static ZipEntry ReadDirEntry(ZipFile zf, Dictionary<string, object> previouslySeen)
    {
      Stream readStream = zf.ReadStream;
      Encoding encoding = zf.AlternateEncodingUsage == ZipOption.Always ? zf.AlternateEncoding : ZipFile.DefaultEncoding;
      int signature = SharedUtilities.ReadSignature(readStream);
      if (ZipEntry.IsNotValidZipDirEntrySig(signature))
      {
        readStream.Seek(-4L, SeekOrigin.Current);
        if ((long) signature != 101010256L && (long) signature != 101075792L && signature != 67324752)
          throw new BadReadException(string.Format("  Bad signature (0x{0:X8}) at position 0x{1:X8}", (object) signature, (object) readStream.Position));
        return (ZipEntry) null;
      }
      int num1 = 46;
      byte[] buffer = new byte[42];
      if (readStream.Read(buffer, 0, buffer.Length) != buffer.Length)
        return (ZipEntry) null;
      int num2 = 0;
      ZipEntry zipEntry1 = new ZipEntry();
      zipEntry1.AlternateEncoding = encoding;
      zipEntry1._Source = ZipEntrySource.ZipFile;
      zipEntry1._container = new ZipContainer((object) zf);
      ZipEntry zipEntry2 = zipEntry1;
      byte[] numArray1 = buffer;
      int index1 = num2;
      int num3 = 1;
      int num4 = index1 + num3;
      int num5 = (int) numArray1[index1];
      byte[] numArray2 = buffer;
      int index2 = num4;
      int num6 = 1;
      int num7 = index2 + num6;
      int num8 = (int) numArray2[index2] * 256;
      int num9 = (int) (short) (num5 + num8);
      zipEntry2._VersionMadeBy = (short) num9;
      ZipEntry zipEntry3 = zipEntry1;
      byte[] numArray3 = buffer;
      int index3 = num7;
      int num10 = 1;
      int num11 = index3 + num10;
      int num12 = (int) numArray3[index3];
      byte[] numArray4 = buffer;
      int index4 = num11;
      int num13 = 1;
      int num14 = index4 + num13;
      int num15 = (int) numArray4[index4] * 256;
      int num16 = (int) (short) (num12 + num15);
      zipEntry3._VersionNeeded = (short) num16;
      ZipEntry zipEntry4 = zipEntry1;
      byte[] numArray5 = buffer;
      int index5 = num14;
      int num17 = 1;
      int num18 = index5 + num17;
      int num19 = (int) numArray5[index5];
      byte[] numArray6 = buffer;
      int index6 = num18;
      int num20 = 1;
      int num21 = index6 + num20;
      int num22 = (int) numArray6[index6] * 256;
      int num23 = (int) (short) (num19 + num22);
      zipEntry4._BitField = (short) num23;
      ZipEntry zipEntry5 = zipEntry1;
      byte[] numArray7 = buffer;
      int index7 = num21;
      int num24 = 1;
      int num25 = index7 + num24;
      int num26 = (int) numArray7[index7];
      byte[] numArray8 = buffer;
      int index8 = num25;
      int num27 = 1;
      int num28 = index8 + num27;
      int num29 = (int) numArray8[index8] * 256;
      int num30 = (int) (short) (num26 + num29);
      zipEntry5._CompressionMethod = (short) num30;
      ZipEntry zipEntry6 = zipEntry1;
      byte[] numArray9 = buffer;
      int index9 = num28;
      int num31 = 1;
      int num32 = index9 + num31;
      int num33 = (int) numArray9[index9];
      byte[] numArray10 = buffer;
      int index10 = num32;
      int num34 = 1;
      int num35 = index10 + num34;
      int num36 = (int) numArray10[index10] * 256;
      int num37 = num33 + num36;
      byte[] numArray11 = buffer;
      int index11 = num35;
      int num38 = 1;
      int num39 = index11 + num38;
      int num40 = (int) numArray11[index11] * 256 * 256;
      int num41 = num37 + num40;
      byte[] numArray12 = buffer;
      int index12 = num39;
      int num42 = 1;
      int num43 = index12 + num42;
      int num44 = (int) numArray12[index12] * 256 * 256 * 256;
      int num45 = num41 + num44;
      zipEntry6._TimeBlob = num45;
      zipEntry1._LastModified = SharedUtilities.PackedToDateTime(zipEntry1._TimeBlob);
      zipEntry1._timestamp |= ZipEntryTimestamp.DOS;
      ZipEntry zipEntry7 = zipEntry1;
      byte[] numArray13 = buffer;
      int index13 = num43;
      int num46 = 1;
      int num47 = index13 + num46;
      int num48 = (int) numArray13[index13];
      byte[] numArray14 = buffer;
      int index14 = num47;
      int num49 = 1;
      int num50 = index14 + num49;
      int num51 = (int) numArray14[index14] * 256;
      int num52 = num48 + num51;
      byte[] numArray15 = buffer;
      int index15 = num50;
      int num53 = 1;
      int num54 = index15 + num53;
      int num55 = (int) numArray15[index15] * 256 * 256;
      int num56 = num52 + num55;
      byte[] numArray16 = buffer;
      int index16 = num54;
      int num57 = 1;
      int num58 = index16 + num57;
      int num59 = (int) numArray16[index16] * 256 * 256 * 256;
      int num60 = num56 + num59;
      zipEntry7._Crc32 = num60;
      ZipEntry zipEntry8 = zipEntry1;
      byte[] numArray17 = buffer;
      int index17 = num58;
      int num61 = 1;
      int num62 = index17 + num61;
      int num63 = (int) numArray17[index17];
      byte[] numArray18 = buffer;
      int index18 = num62;
      int num64 = 1;
      int num65 = index18 + num64;
      int num66 = (int) numArray18[index18] * 256;
      int num67 = num63 + num66;
      byte[] numArray19 = buffer;
      int index19 = num65;
      int num68 = 1;
      int num69 = index19 + num68;
      int num70 = (int) numArray19[index19] * 256 * 256;
      int num71 = num67 + num70;
      byte[] numArray20 = buffer;
      int index20 = num69;
      int num72 = 1;
      int num73 = index20 + num72;
      int num74 = (int) numArray20[index20] * 256 * 256 * 256;
      long num75 = (long) (uint) (num71 + num74);
      zipEntry8._CompressedSize = num75;
      ZipEntry zipEntry9 = zipEntry1;
      byte[] numArray21 = buffer;
      int index21 = num73;
      int num76 = 1;
      int num77 = index21 + num76;
      int num78 = (int) numArray21[index21];
      byte[] numArray22 = buffer;
      int index22 = num77;
      int num79 = 1;
      int num80 = index22 + num79;
      int num81 = (int) numArray22[index22] * 256;
      int num82 = num78 + num81;
      byte[] numArray23 = buffer;
      int index23 = num80;
      int num83 = 1;
      int num84 = index23 + num83;
      int num85 = (int) numArray23[index23] * 256 * 256;
      int num86 = num82 + num85;
      byte[] numArray24 = buffer;
      int index24 = num84;
      int num87 = 1;
      int num88 = index24 + num87;
      int num89 = (int) numArray24[index24] * 256 * 256 * 256;
      long num90 = (long) (uint) (num86 + num89);
      zipEntry9._UncompressedSize = num90;
      zipEntry1._CompressionMethod_FromZipFile = zipEntry1._CompressionMethod;
      ZipEntry zipEntry10 = zipEntry1;
      byte[] numArray25 = buffer;
      int index25 = num88;
      int num91 = 1;
      int num92 = index25 + num91;
      int num93 = (int) numArray25[index25];
      byte[] numArray26 = buffer;
      int index26 = num92;
      int num94 = 1;
      int num95 = index26 + num94;
      int num96 = (int) numArray26[index26] * 256;
      int num97 = (int) (short) (num93 + num96);
      zipEntry10._filenameLength = (short) num97;
      ZipEntry zipEntry11 = zipEntry1;
      byte[] numArray27 = buffer;
      int index27 = num95;
      int num98 = 1;
      int num99 = index27 + num98;
      int num100 = (int) numArray27[index27];
      byte[] numArray28 = buffer;
      int index28 = num99;
      int num101 = 1;
      int num102 = index28 + num101;
      int num103 = (int) numArray28[index28] * 256;
      int num104 = (int) (short) (num100 + num103);
      zipEntry11._extraFieldLength = (short) num104;
      ZipEntry zipEntry12 = zipEntry1;
      byte[] numArray29 = buffer;
      int index29 = num102;
      int num105 = 1;
      int num106 = index29 + num105;
      int num107 = (int) numArray29[index29];
      byte[] numArray30 = buffer;
      int index30 = num106;
      int num108 = 1;
      int num109 = index30 + num108;
      int num110 = (int) numArray30[index30] * 256;
      int num111 = (int) (short) (num107 + num110);
      zipEntry12._commentLength = (short) num111;
      ZipEntry zipEntry13 = zipEntry1;
      byte[] numArray31 = buffer;
      int index31 = num109;
      int num112 = 1;
      int num113 = index31 + num112;
      int num114 = (int) numArray31[index31];
      byte[] numArray32 = buffer;
      int index32 = num113;
      int num115 = 1;
      int num116 = index32 + num115;
      int num117 = (int) numArray32[index32] * 256;
      int num118 = num114 + num117;
      zipEntry13._diskNumber = (uint) num118;
      ZipEntry zipEntry14 = zipEntry1;
      byte[] numArray33 = buffer;
      int index33 = num116;
      int num119 = 1;
      int num120 = index33 + num119;
      int num121 = (int) numArray33[index33];
      byte[] numArray34 = buffer;
      int index34 = num120;
      int num122 = 1;
      int num123 = index34 + num122;
      int num124 = (int) numArray34[index34] * 256;
      int num125 = (int) (short) (num121 + num124);
      zipEntry14._InternalFileAttrs = (short) num125;
      ZipEntry zipEntry15 = zipEntry1;
      byte[] numArray35 = buffer;
      int index35 = num123;
      int num126 = 1;
      int num127 = index35 + num126;
      int num128 = (int) numArray35[index35];
      byte[] numArray36 = buffer;
      int index36 = num127;
      int num129 = 1;
      int num130 = index36 + num129;
      int num131 = (int) numArray36[index36] * 256;
      int num132 = num128 + num131;
      byte[] numArray37 = buffer;
      int index37 = num130;
      int num133 = 1;
      int num134 = index37 + num133;
      int num135 = (int) numArray37[index37] * 256 * 256;
      int num136 = num132 + num135;
      byte[] numArray38 = buffer;
      int index38 = num134;
      int num137 = 1;
      int num138 = index38 + num137;
      int num139 = (int) numArray38[index38] * 256 * 256 * 256;
      int num140 = num136 + num139;
      zipEntry15._ExternalFileAttrs = num140;
      ZipEntry zipEntry16 = zipEntry1;
      byte[] numArray39 = buffer;
      int index39 = num138;
      int num141 = 1;
      int num142 = index39 + num141;
      int num143 = (int) numArray39[index39];
      byte[] numArray40 = buffer;
      int index40 = num142;
      int num144 = 1;
      int num145 = index40 + num144;
      int num146 = (int) numArray40[index40] * 256;
      int num147 = num143 + num146;
      byte[] numArray41 = buffer;
      int index41 = num145;
      int num148 = 1;
      int num149 = index41 + num148;
      int num150 = (int) numArray41[index41] * 256 * 256;
      int num151 = num147 + num150;
      byte[] numArray42 = buffer;
      int index42 = num149;
      int num152 = 1;
      int num153 = index42 + num152;
      int num154 = (int) numArray42[index42] * 256 * 256 * 256;
      long num155 = (long) (uint) (num151 + num154);
      zipEntry16._RelativeOffsetOfLocalHeader = num155;
      zipEntry1.IsText = ((int) zipEntry1._InternalFileAttrs & 1) == 1;
      byte[] numArray43 = new byte[(int) zipEntry1._filenameLength];
      int num156 = readStream.Read(numArray43, 0, numArray43.Length);
      int num157 = num1 + num156;
      zipEntry1._FileNameInArchive = ((int) zipEntry1._BitField & 2048) != 2048 ? SharedUtilities.StringFromBuffer(numArray43, encoding) : SharedUtilities.Utf8StringFromBuffer(numArray43);
      while (previouslySeen.ContainsKey(zipEntry1._FileNameInArchive))
      {
        zipEntry1._FileNameInArchive = ZipEntry.CopyHelper.AppendCopyToFileName(zipEntry1._FileNameInArchive);
        zipEntry1._metadataChanged = true;
      }
      if (zipEntry1.AttributesIndicateDirectory)
        zipEntry1.MarkAsDirectory();
      else if (zipEntry1._FileNameInArchive.EndsWith("/"))
        zipEntry1.MarkAsDirectory();
      zipEntry1._CompressedFileDataSize = zipEntry1._CompressedSize;
      if (((int) zipEntry1._BitField & 1) == 1)
      {
        zipEntry1._Encryption_FromZipFile = zipEntry1._Encryption = EncryptionAlgorithm.PkzipWeak;
        zipEntry1._sourceIsEncrypted = true;
      }
      if ((int) zipEntry1._extraFieldLength > 0)
      {
        zipEntry1._InputUsesZip64 = zipEntry1._CompressedSize == (long) uint.MaxValue || zipEntry1._UncompressedSize == (long) uint.MaxValue || zipEntry1._RelativeOffsetOfLocalHeader == (long) uint.MaxValue;
        num157 += zipEntry1.ProcessExtraField(readStream, zipEntry1._extraFieldLength);
        zipEntry1._CompressedFileDataSize = zipEntry1._CompressedSize;
      }
      if (zipEntry1._Encryption == EncryptionAlgorithm.PkzipWeak)
        zipEntry1._CompressedFileDataSize -= 12L;
      if (((int) zipEntry1._BitField & 8) == 8)
      {
        if (zipEntry1._InputUsesZip64)
          zipEntry1._LengthOfTrailer += 24;
        else
          zipEntry1._LengthOfTrailer += 16;
      }
      zipEntry1.AlternateEncoding = ((int) zipEntry1._BitField & 2048) == 2048 ? Encoding.UTF8 : encoding;
      zipEntry1.AlternateEncodingUsage = ZipOption.Always;
      if ((int) zipEntry1._commentLength > 0)
      {
        byte[] numArray44 = new byte[(int) zipEntry1._commentLength];
        int num158 = readStream.Read(numArray44, 0, numArray44.Length);
        int num159 = num157 + num158;
        zipEntry1._Comment = ((int) zipEntry1._BitField & 2048) != 2048 ? SharedUtilities.StringFromBuffer(numArray44, encoding) : SharedUtilities.Utf8StringFromBuffer(numArray44);
      }
      return zipEntry1;
    }

    internal static bool IsNotValidZipDirEntrySig(int signature)
    {
      return signature != 33639248;
    }

    public void SetEntryTimes(DateTime created, DateTime accessed, DateTime modified)
    {
      this._ntfsTimesAreSet = true;
      if (created == ZipEntry._zeroHour && created.Kind == ZipEntry._zeroHour.Kind)
        created = ZipEntry._win32Epoch;
      if (accessed == ZipEntry._zeroHour && accessed.Kind == ZipEntry._zeroHour.Kind)
        accessed = ZipEntry._win32Epoch;
      if (modified == ZipEntry._zeroHour && modified.Kind == ZipEntry._zeroHour.Kind)
        modified = ZipEntry._win32Epoch;
      this._Ctime = created.ToUniversalTime();
      this._Atime = accessed.ToUniversalTime();
      this._Mtime = modified.ToUniversalTime();
      this._LastModified = this._Mtime;
      if (!this._emitUnixTimes && !this._emitNtfsTimes)
        this._emitNtfsTimes = true;
      this._metadataChanged = true;
    }

    internal static string NameInArchive(string filename, string directoryPathInArchive)
    {
      return SharedUtilities.NormalizePathForUseInZipFile(directoryPathInArchive != null ? (!string.IsNullOrEmpty(directoryPathInArchive) ? Path.Combine(directoryPathInArchive, Path.GetFileName(filename)) : Path.GetFileName(filename)) : filename);
    }

    internal static ZipEntry CreateFromNothing(string nameInArchive)
    {
      return ZipEntry.Create(nameInArchive, ZipEntrySource.None, (object) null, (object) null);
    }

    internal static ZipEntry CreateFromFile(string filename, string nameInArchive)
    {
      return ZipEntry.Create(nameInArchive, ZipEntrySource.FileSystem, (object) filename, (object) null);
    }

    internal static ZipEntry CreateForStream(string entryName, Stream s)
    {
      return ZipEntry.Create(entryName, ZipEntrySource.Stream, (object) s, (object) null);
    }

    internal static ZipEntry CreateForWriter(string entryName, WriteDelegate d)
    {
      return ZipEntry.Create(entryName, ZipEntrySource.WriteDelegate, (object) d, (object) null);
    }

    internal static ZipEntry CreateForJitStreamProvider(string nameInArchive, OpenDelegate opener, CloseDelegate closer)
    {
      return ZipEntry.Create(nameInArchive, ZipEntrySource.JitStream, (object) opener, (object) closer);
    }

    internal static ZipEntry CreateForZipOutputStream(string nameInArchive)
    {
      return ZipEntry.Create(nameInArchive, ZipEntrySource.ZipOutputStream, (object) null, (object) null);
    }

    private static ZipEntry Create(string nameInArchive, ZipEntrySource source, object arg1, object arg2)
    {
      if (string.IsNullOrEmpty(nameInArchive))
        throw new ZipException("The entry name must be non-null and non-empty.");
      ZipEntry zipEntry = new ZipEntry();
      zipEntry._VersionMadeBy = (short) 45;
      zipEntry._Source = source;
      zipEntry._Mtime = zipEntry._Atime = zipEntry._Ctime = DateTime.UtcNow;
      if (source == ZipEntrySource.Stream)
        zipEntry._sourceStream = arg1 as Stream;
      else if (source == ZipEntrySource.WriteDelegate)
        zipEntry._WriteDelegate = arg1 as WriteDelegate;
      else if (source == ZipEntrySource.JitStream)
      {
        zipEntry._OpenDelegate = arg1 as OpenDelegate;
        zipEntry._CloseDelegate = arg2 as CloseDelegate;
      }
      else if (source != ZipEntrySource.ZipOutputStream)
      {
        if (source == ZipEntrySource.None)
        {
          zipEntry._Source = ZipEntrySource.FileSystem;
        }
        else
        {
          string path = arg1 as string;
          if (string.IsNullOrEmpty(path))
            throw new ZipException("The filename must be non-null and non-empty.");
          try
          {
            zipEntry._Mtime = File.GetLastWriteTime(path).ToUniversalTime();
            zipEntry._Ctime = File.GetCreationTime(path).ToUniversalTime();
            zipEntry._Atime = File.GetLastAccessTime(path).ToUniversalTime();
            if (File.Exists(path) || Directory.Exists(path))
              zipEntry._ExternalFileAttrs = (int) File.GetAttributes(path);
            zipEntry._ntfsTimesAreSet = true;
            zipEntry._LocalFileName = Path.GetFullPath(path);
          }
          catch (PathTooLongException ex)
          {
            throw new ZipException(string.Format("The path is too long, filename={0}", (object) path), (Exception) ex);
          }
        }
      }
      zipEntry._LastModified = zipEntry._Mtime;
      zipEntry._FileNameInArchive = SharedUtilities.NormalizePathForUseInZipFile(nameInArchive);
      return zipEntry;
    }

    internal void MarkAsDirectory()
    {
      this._IsDirectory = true;
      if (this._FileNameInArchive.EndsWith("/"))
        return;
      this._FileNameInArchive += "/";
    }

    public override string ToString()
    {
      return string.Format("ZipEntry::{0}", (object) this.FileName);
    }

    private void SetFdpLoh()
    {
      long position = this.ArchiveStream.Position;
      try
      {
        this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
      }
      catch (IOException ex)
      {
        throw new BadStateException(string.Format("Exception seeking  entry({0}) offset(0x{1:X8}) len(0x{2:X8})", (object) this.FileName, (object) this._RelativeOffsetOfLocalHeader, (object) this.ArchiveStream.Length), (Exception) ex);
      }
      byte[] buffer = new byte[30];
      this.ArchiveStream.Read(buffer, 0, buffer.Length);
      short num1 = (short) ((int) buffer[26] + (int) buffer[27] * 256);
      short num2 = (short) ((int) buffer[28] + (int) buffer[29] * 256);
      this.ArchiveStream.Seek((long) ((int) num1 + (int) num2), SeekOrigin.Current);
      this._LengthOfHeader = 30 + (int) num2 + (int) num1 + ZipEntry.GetLengthOfCryptoHeaderBytes(this._Encryption_FromZipFile);
      this.__FileDataPosition = this._RelativeOffsetOfLocalHeader + (long) this._LengthOfHeader;
      this.ArchiveStream.Seek(position, SeekOrigin.Begin);
    }

    internal static int GetLengthOfCryptoHeaderBytes(EncryptionAlgorithm a)
    {
      if (a == EncryptionAlgorithm.None)
        return 0;
      if (a == EncryptionAlgorithm.PkzipWeak)
        return 12;
      throw new ZipException("internal error");
    }

    public void Extract()
    {
      this.InternalExtract(".", (Stream) null, (string) null);
    }

    public void Extract(ExtractExistingFileAction extractExistingFile)
    {
      this.ExtractExistingFile = extractExistingFile;
      this.InternalExtract(".", (Stream) null, (string) null);
    }

    public void Extract(Stream stream)
    {
      this.InternalExtract((string) null, stream, (string) null);
    }

    public void Extract(string baseDirectory)
    {
      this.InternalExtract(baseDirectory, (Stream) null, (string) null);
    }

    public void Extract(string baseDirectory, ExtractExistingFileAction extractExistingFile)
    {
      this.ExtractExistingFile = extractExistingFile;
      this.InternalExtract(baseDirectory, (Stream) null, (string) null);
    }

    public void ExtractWithPassword(string password)
    {
      this.InternalExtract(".", (Stream) null, password);
    }

    public void ExtractWithPassword(string baseDirectory, string password)
    {
      this.InternalExtract(baseDirectory, (Stream) null, password);
    }

    public void ExtractWithPassword(ExtractExistingFileAction extractExistingFile, string password)
    {
      this.ExtractExistingFile = extractExistingFile;
      this.InternalExtract(".", (Stream) null, password);
    }

    public void ExtractWithPassword(string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
    {
      this.ExtractExistingFile = extractExistingFile;
      this.InternalExtract(baseDirectory, (Stream) null, password);
    }

    public void ExtractWithPassword(Stream stream, string password)
    {
      this.InternalExtract((string) null, stream, password);
    }

    public CrcCalculatorStream OpenReader()
    {
      if (this._container.ZipFile == null)
        throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
      return this.InternalOpenReader(this._Password ?? this._container.Password);
    }

    public CrcCalculatorStream OpenReader(string password)
    {
      if (this._container.ZipFile == null)
        throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
      return this.InternalOpenReader(password);
    }

    internal CrcCalculatorStream InternalOpenReader(string password)
    {
      this.ValidateCompression();
      this.ValidateEncryption();
      this.SetupCryptoForExtract(password);
      if (this._Source != ZipEntrySource.ZipFile)
        throw new BadStateException("You must call ZipFile.Save before calling OpenReader");
      long length = (int) this._CompressionMethod_FromZipFile == 0 ? this._CompressedFileDataSize : this.UncompressedSize;
      Stream archiveStream = this.ArchiveStream;
      this.ArchiveStream.Seek(this.FileDataPosition, SeekOrigin.Begin);
      this._inputDecryptorStream = this.GetExtractDecryptor(archiveStream);
      return new CrcCalculatorStream(this.GetExtractDecompressor(this._inputDecryptorStream), length);
    }

    private void OnExtractProgress(long bytesWritten, long totalBytesToWrite)
    {
      if (this._container.ZipFile == null)
        return;
      this._ioOperationCanceled = this._container.ZipFile.OnExtractBlock(this, bytesWritten, totalBytesToWrite);
    }

    private void OnBeforeExtract(string path)
    {
      if (this._container.ZipFile == null || this._container.ZipFile._inExtractAll)
        return;
      this._ioOperationCanceled = this._container.ZipFile.OnSingleEntryExtract(this, path, true);
    }

    private void OnAfterExtract(string path)
    {
      if (this._container.ZipFile == null || this._container.ZipFile._inExtractAll)
        return;
      this._container.ZipFile.OnSingleEntryExtract(this, path, false);
    }

    private void OnExtractExisting(string path)
    {
      if (this._container.ZipFile == null)
        return;
      this._ioOperationCanceled = this._container.ZipFile.OnExtractExisting(this, path);
    }

    private static void ReallyDelete(string fileName)
    {
      if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        File.SetAttributes(fileName, FileAttributes.Normal);
      File.Delete(fileName);
    }

    private void WriteStatus(string format, params object[] args)
    {
      if (this._container.ZipFile == null || !this._container.ZipFile.Verbose)
        return;
      this._container.ZipFile.StatusMessageTextWriter.WriteLine(format, args);
    }

    private void InternalExtract(string baseDir, Stream outstream, string password)
    {
      if (this._container == null)
        throw new BadStateException("This entry is an orphan");
      if (this._container.ZipFile == null)
        throw new InvalidOperationException("Use Extract() only with ZipFile.");
      this._container.ZipFile.Reset(false);
      if (this._Source != ZipEntrySource.ZipFile)
        throw new BadStateException("You must call ZipFile.Save before calling any Extract method");
      this.OnBeforeExtract(baseDir);
      this._ioOperationCanceled = false;
      string outFileName = (string) null;
      Stream output = (Stream) null;
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        this.ValidateCompression();
        this.ValidateEncryption();
        if (this.ValidateOutput(baseDir, outstream, out outFileName))
        {
          this.WriteStatus("extract dir {0}...", (object) outFileName);
          this.OnAfterExtract(baseDir);
        }
        else
        {
          if (outFileName != null && File.Exists(outFileName))
          {
            flag1 = true;
            switch (this.CheckExtractExistingFile(baseDir, outFileName))
            {
              case 2:
                return;
              case 1:
                return;
            }
          }
          string password1 = password ?? this._Password ?? this._container.Password;
          if (this._Encryption_FromZipFile != EncryptionAlgorithm.None)
          {
            if (password1 == null)
              throw new BadPasswordException();
            this.SetupCryptoForExtract(password1);
          }
          if (outFileName != null)
          {
            this.WriteStatus("extract file {0}...", (object) outFileName);
            outFileName += ".tmp";
            string directoryName = Path.GetDirectoryName(outFileName);
            if (!Directory.Exists(directoryName))
              Directory.CreateDirectory(directoryName);
            else if (this._container.ZipFile != null)
              flag2 = this._container.ZipFile._inExtractAll;
            output = (Stream) new FileStream(outFileName, FileMode.Create, FileAccess.ReadWrite);
          }
          else
          {
            this.WriteStatus("extract entry {0} to stream...", (object) this.FileName);
            output = outstream;
          }
          if (this._ioOperationCanceled)
            return;
          int one = this.ExtractOne(output);
          if (this._ioOperationCanceled)
            return;
          this.VerifyCrcAfterExtract(one);
          if (outFileName != null)
          {
            output.Close();
            output = (Stream) null;
            string sourceFileName = outFileName;
            string str = (string) null;
            outFileName = sourceFileName.Substring(0, sourceFileName.Length - 4);
            if (flag1)
            {
              str = outFileName + ".PendingOverwrite";
              File.Move(outFileName, str);
            }
            File.Move(sourceFileName, outFileName);
            this._SetTimes(outFileName, true);
            if (str != null && File.Exists(str))
              ZipEntry.ReallyDelete(str);
            if (flag2 && this.FileName.IndexOf('/') != -1 && this._container.ZipFile[Path.GetDirectoryName(this.FileName)] == null)
              this._SetTimes(Path.GetDirectoryName(outFileName), false);
            if (((int) this._VersionMadeBy & 65280) == 2560 || ((int) this._VersionMadeBy & 65280) == 0)
              File.SetAttributes(outFileName, (FileAttributes) this._ExternalFileAttrs);
          }
          this.OnAfterExtract(baseDir);
        }
      }
      catch (Exception ex)
      {
        this._ioOperationCanceled = true;
        throw;
      }
      finally
      {
        if (this._ioOperationCanceled && outFileName != null)
        {
          if (output != null)
            output.Close();
          if (File.Exists(outFileName) && !flag1)
            File.Delete(outFileName);
        }
      }
    }

    internal void VerifyCrcAfterExtract(int actualCrc32)
    {
      if (actualCrc32 != this._Crc32)
        throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " + string.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", (object) this._Crc32, (object) actualCrc32));
    }

    private int CheckExtractExistingFile(string baseDir, string targetFileName)
    {
      int num = 0;
      while (true)
      {
        switch (this.ExtractExistingFile)
        {
          case ExtractExistingFileAction.OverwriteSilently:
            goto label_2;
          case ExtractExistingFileAction.DoNotOverwrite:
            goto label_3;
          case ExtractExistingFileAction.InvokeExtractProgressEvent:
            if (num <= 0)
            {
              this.OnExtractExisting(baseDir);
              if (!this._ioOperationCanceled)
              {
                ++num;
                continue;
              }
              goto label_7;
            }
            else
              goto label_5;
          default:
            goto label_8;
        }
      }
label_2:
      this.WriteStatus("the file {0} exists; will overwrite it...", (object) targetFileName);
      return 0;
label_3:
      this.WriteStatus("the file {0} exists; not extracting entry...", (object) this.FileName);
      this.OnAfterExtract(baseDir);
      return 1;
label_5:
      throw new ZipException(string.Format("The file {0} already exists.", (object) targetFileName));
label_7:
      return 2;
label_8:
      throw new ZipException(string.Format("The file {0} already exists.", (object) targetFileName));
    }

    private void _CheckRead(int nbytes)
    {
      if (nbytes == 0)
        throw new BadReadException(string.Format("bad read of entry {0} from compressed archive.", (object) this.FileName));
    }

    private int ExtractOne(Stream output)
    {
      Stream archiveStream = this.ArchiveStream;
      try
      {
        archiveStream.Seek(this.FileDataPosition, SeekOrigin.Begin);
        byte[] buffer = new byte[this.BufferSize];
        long num1 = (int) this._CompressionMethod_FromZipFile != 0 ? this.UncompressedSize : this._CompressedFileDataSize;
        this._inputDecryptorStream = this.GetExtractDecryptor(archiveStream);
        Stream extractDecompressor = this.GetExtractDecompressor(this._inputDecryptorStream);
        long bytesWritten = 0L;
        using (CrcCalculatorStream calculatorStream = new CrcCalculatorStream(extractDecompressor))
        {
          while (num1 > 0L)
          {
            int count = num1 > (long) buffer.Length ? buffer.Length : (int) num1;
            int num2 = calculatorStream.Read(buffer, 0, count);
            this._CheckRead(num2);
            output.Write(buffer, 0, num2);
            num1 -= (long) num2;
            bytesWritten += (long) num2;
            this.OnExtractProgress(bytesWritten, this.UncompressedSize);
            if (this._ioOperationCanceled)
              break;
          }
          return calculatorStream.Crc;
        }
      }
      finally
      {
        ZipSegmentedStream zipSegmentedStream = archiveStream as ZipSegmentedStream;
        if (zipSegmentedStream != null)
        {
          zipSegmentedStream.Dispose();
          this._archiveStream = (Stream) null;
        }
      }
    }

    internal Stream GetExtractDecompressor(Stream input2)
    {
      switch (this._CompressionMethod_FromZipFile)
      {
        case (short) 0:
          return input2;
        case (short) 8:
          return (Stream) new DeflateStream(input2, CompressionMode.Decompress, true);
        default:
          return (Stream) null;
      }
    }

    internal Stream GetExtractDecryptor(Stream input)
    {
      return this._Encryption_FromZipFile != EncryptionAlgorithm.PkzipWeak ? input : (Stream) new ZipCipherStream(input, this._zipCrypto_forExtract, CryptoMode.Decrypt);
    }

    internal void _SetTimes(string fileOrDirectory, bool isFile)
    {
      try
      {
        if (this._ntfsTimesAreSet)
        {
          if (isFile)
          {
            if (!File.Exists(fileOrDirectory))
              return;
            File.SetCreationTimeUtc(fileOrDirectory, this._Ctime);
            File.SetLastAccessTimeUtc(fileOrDirectory, this._Atime);
            File.SetLastWriteTimeUtc(fileOrDirectory, this._Mtime);
          }
          else
          {
            if (!Directory.Exists(fileOrDirectory))
              return;
            Directory.SetCreationTimeUtc(fileOrDirectory, this._Ctime);
            Directory.SetLastAccessTimeUtc(fileOrDirectory, this._Atime);
            Directory.SetLastWriteTimeUtc(fileOrDirectory, this._Mtime);
          }
        }
        else
        {
          DateTime lastWriteTime = SharedUtilities.AdjustTime_Reverse(this.LastModified);
          if (isFile)
            File.SetLastWriteTime(fileOrDirectory, lastWriteTime);
          else
            Directory.SetLastWriteTime(fileOrDirectory, lastWriteTime);
        }
      }
      catch (IOException ex)
      {
        this.WriteStatus("failed to set time on {0}: {1}", (object) fileOrDirectory, (object) ex.Message);
      }
    }

    internal void ValidateEncryption()
    {
      if (this.Encryption == EncryptionAlgorithm.PkzipWeak || this.Encryption == EncryptionAlgorithm.None)
        return;
      if ((int) this._UnsupportedAlgorithmId != 0)
        throw new ZipException(string.Format("Cannot extract: Entry {0} is encrypted with an algorithm not supported by DotNetZip: {1}", (object) this.FileName, (object) this.UnsupportedAlgorithm));
      throw new ZipException(string.Format("Cannot extract: Entry {0} uses an unsupported encryption algorithm ({1:X2})", (object) this.FileName, (object) this.Encryption));
    }

    private void ValidateCompression()
    {
      if ((int) this._CompressionMethod_FromZipFile != 0 && (int) this._CompressionMethod_FromZipFile != 8)
        throw new ZipException(string.Format("Entry {0} uses an unsupported compression method (0x{1:X2}, {2})", (object) this.FileName, (object) this._CompressionMethod_FromZipFile, (object) this.UnsupportedCompressionMethod));
    }

    private void SetupCryptoForExtract(string password)
    {
      if (this._Encryption_FromZipFile == EncryptionAlgorithm.None || this._Encryption_FromZipFile != EncryptionAlgorithm.PkzipWeak)
        return;
      if (password == null)
        throw new ZipException("Missing password.");
      this.ArchiveStream.Seek(this.FileDataPosition - 12L, SeekOrigin.Begin);
      this._zipCrypto_forExtract = ZipCrypto.ForRead(password, this);
    }

    private bool ValidateOutput(string basedir, Stream outstream, out string outFileName)
    {
      if (basedir != null)
      {
        string str = this.FileName.Replace("\\", "/");
        if (str.IndexOf(':') == 1)
          str = str.Substring(2);
        if (str.StartsWith("/"))
          str = str.Substring(1);
        outFileName = !this._container.ZipFile.FlattenFoldersOnExtract ? Path.Combine(basedir, str) : Path.Combine(basedir, str.IndexOf('/') != -1 ? Path.GetFileName(str) : str);
        outFileName = outFileName.Replace("/", "\\");
        if (!this.IsDirectory && !this.FileName.EndsWith("/"))
          return false;
        if (!Directory.Exists(outFileName))
        {
          Directory.CreateDirectory(outFileName);
          this._SetTimes(outFileName, false);
        }
        else if (this.ExtractExistingFile == ExtractExistingFileAction.OverwriteSilently)
          this._SetTimes(outFileName, false);
        return true;
      }
      if (outstream == null)
        throw new ArgumentNullException("outstream");
      outFileName = (string) null;
      return this.IsDirectory || this.FileName.EndsWith("/");
    }

    private void ReadExtraField()
    {
      ++this._readExtraDepth;
      long position = this.ArchiveStream.Position;
      this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
      byte[] buffer = new byte[30];
      this.ArchiveStream.Read(buffer, 0, buffer.Length);
      int num1 = 26;
      byte[] numArray1 = buffer;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2;
      int num4 = (int) numArray1[index1];
      byte[] numArray2 = buffer;
      int index2 = num3;
      int num5 = 1;
      int num6 = index2 + num5;
      int num7 = (int) numArray2[index2] * 256;
      short num8 = (short) (num4 + num7);
      byte[] numArray3 = buffer;
      int index3 = num6;
      int num9 = 1;
      int num10 = index3 + num9;
      int num11 = (int) numArray3[index3];
      byte[] numArray4 = buffer;
      int index4 = num10;
      int num12 = 1;
      int num13 = index4 + num12;
      int num14 = (int) numArray4[index4] * 256;
      short extraFieldLength = (short) (num11 + num14);
      this.ArchiveStream.Seek((long) num8, SeekOrigin.Current);
      this.ProcessExtraField(this.ArchiveStream, extraFieldLength);
      this.ArchiveStream.Seek(position, SeekOrigin.Begin);
      --this._readExtraDepth;
    }

    private static bool ReadHeader(ZipEntry ze, Encoding defaultEncoding)
    {
      int num1 = 0;
      ze._RelativeOffsetOfLocalHeader = ze.ArchiveStream.Position;
      int signature1 = SharedUtilities.ReadEntrySignature(ze.ArchiveStream);
      int num2 = num1 + 4;
      if (ZipEntry.IsNotValidSig(signature1))
      {
        ze.ArchiveStream.Seek(-4L, SeekOrigin.Current);
        if (ZipEntry.IsNotValidZipDirEntrySig(signature1) && (long) signature1 != 101010256L)
          throw new BadReadException(string.Format("  Bad signature (0x{0:X8}) at position  0x{1:X8}", (object) signature1, (object) ze.ArchiveStream.Position));
        return false;
      }
      byte[] buffer1 = new byte[26];
      int num3 = ze.ArchiveStream.Read(buffer1, 0, buffer1.Length);
      if (num3 != buffer1.Length)
        return false;
      int num4 = num2 + num3;
      int num5 = 0;
      ZipEntry zipEntry1 = ze;
      byte[] numArray1 = buffer1;
      int index1 = num5;
      int num6 = 1;
      int num7 = index1 + num6;
      int num8 = (int) numArray1[index1];
      byte[] numArray2 = buffer1;
      int index2 = num7;
      int num9 = 1;
      int num10 = index2 + num9;
      int num11 = (int) numArray2[index2] * 256;
      int num12 = (int) (short) (num8 + num11);
      zipEntry1._VersionNeeded = (short) num12;
      ZipEntry zipEntry2 = ze;
      byte[] numArray3 = buffer1;
      int index3 = num10;
      int num13 = 1;
      int num14 = index3 + num13;
      int num15 = (int) numArray3[index3];
      byte[] numArray4 = buffer1;
      int index4 = num14;
      int num16 = 1;
      int num17 = index4 + num16;
      int num18 = (int) numArray4[index4] * 256;
      int num19 = (int) (short) (num15 + num18);
      zipEntry2._BitField = (short) num19;
      ZipEntry zipEntry3 = ze;
      ZipEntry zipEntry4 = ze;
      byte[] numArray5 = buffer1;
      int index5 = num17;
      int num20 = 1;
      int num21 = index5 + num20;
      int num22 = (int) numArray5[index5];
      byte[] numArray6 = buffer1;
      int index6 = num21;
      int num23 = 1;
      int num24 = index6 + num23;
      int num25 = (int) numArray6[index6] * 256;
      int num26;
      short num27 = (short) (num26 = (int) (short) (num22 + num25));
      zipEntry4._CompressionMethod = (short) num26;
      int num28 = (int) num27;
      zipEntry3._CompressionMethod_FromZipFile = (short) num28;
      ZipEntry zipEntry5 = ze;
      byte[] numArray7 = buffer1;
      int index7 = num24;
      int num29 = 1;
      int num30 = index7 + num29;
      int num31 = (int) numArray7[index7];
      byte[] numArray8 = buffer1;
      int index8 = num30;
      int num32 = 1;
      int num33 = index8 + num32;
      int num34 = (int) numArray8[index8] * 256;
      int num35 = num31 + num34;
      byte[] numArray9 = buffer1;
      int index9 = num33;
      int num36 = 1;
      int num37 = index9 + num36;
      int num38 = (int) numArray9[index9] * 256 * 256;
      int num39 = num35 + num38;
      byte[] numArray10 = buffer1;
      int index10 = num37;
      int num40 = 1;
      int num41 = index10 + num40;
      int num42 = (int) numArray10[index10] * 256 * 256 * 256;
      int num43 = num39 + num42;
      zipEntry5._TimeBlob = num43;
      ze._LastModified = SharedUtilities.PackedToDateTime(ze._TimeBlob);
      ze._timestamp |= ZipEntryTimestamp.DOS;
      if (((int) ze._BitField & 1) == 1)
      {
        ze._Encryption_FromZipFile = ze._Encryption = EncryptionAlgorithm.PkzipWeak;
        ze._sourceIsEncrypted = true;
      }
      ZipEntry zipEntry6 = ze;
      byte[] numArray11 = buffer1;
      int index11 = num41;
      int num44 = 1;
      int num45 = index11 + num44;
      int num46 = (int) numArray11[index11];
      byte[] numArray12 = buffer1;
      int index12 = num45;
      int num47 = 1;
      int num48 = index12 + num47;
      int num49 = (int) numArray12[index12] * 256;
      int num50 = num46 + num49;
      byte[] numArray13 = buffer1;
      int index13 = num48;
      int num51 = 1;
      int num52 = index13 + num51;
      int num53 = (int) numArray13[index13] * 256 * 256;
      int num54 = num50 + num53;
      byte[] numArray14 = buffer1;
      int index14 = num52;
      int num55 = 1;
      int num56 = index14 + num55;
      int num57 = (int) numArray14[index14] * 256 * 256 * 256;
      int num58 = num54 + num57;
      zipEntry6._Crc32 = num58;
      ZipEntry zipEntry7 = ze;
      byte[] numArray15 = buffer1;
      int index15 = num56;
      int num59 = 1;
      int num60 = index15 + num59;
      int num61 = (int) numArray15[index15];
      byte[] numArray16 = buffer1;
      int index16 = num60;
      int num62 = 1;
      int num63 = index16 + num62;
      int num64 = (int) numArray16[index16] * 256;
      int num65 = num61 + num64;
      byte[] numArray17 = buffer1;
      int index17 = num63;
      int num66 = 1;
      int num67 = index17 + num66;
      int num68 = (int) numArray17[index17] * 256 * 256;
      int num69 = num65 + num68;
      byte[] numArray18 = buffer1;
      int index18 = num67;
      int num70 = 1;
      int num71 = index18 + num70;
      int num72 = (int) numArray18[index18] * 256 * 256 * 256;
      long num73 = (long) (uint) (num69 + num72);
      zipEntry7._CompressedSize = num73;
      ZipEntry zipEntry8 = ze;
      byte[] numArray19 = buffer1;
      int index19 = num71;
      int num74 = 1;
      int num75 = index19 + num74;
      int num76 = (int) numArray19[index19];
      byte[] numArray20 = buffer1;
      int index20 = num75;
      int num77 = 1;
      int num78 = index20 + num77;
      int num79 = (int) numArray20[index20] * 256;
      int num80 = num76 + num79;
      byte[] numArray21 = buffer1;
      int index21 = num78;
      int num81 = 1;
      int num82 = index21 + num81;
      int num83 = (int) numArray21[index21] * 256 * 256;
      int num84 = num80 + num83;
      byte[] numArray22 = buffer1;
      int index22 = num82;
      int num85 = 1;
      int num86 = index22 + num85;
      int num87 = (int) numArray22[index22] * 256 * 256 * 256;
      long num88 = (long) (uint) (num84 + num87);
      zipEntry8._UncompressedSize = num88;
      if ((int) (uint) ze._CompressedSize == -1 || (int) (uint) ze._UncompressedSize == -1)
        ze._InputUsesZip64 = true;
      byte[] numArray23 = buffer1;
      int index23 = num86;
      int num89 = 1;
      int num90 = index23 + num89;
      int num91 = (int) numArray23[index23];
      byte[] numArray24 = buffer1;
      int index24 = num90;
      int num92 = 1;
      int num93 = index24 + num92;
      int num94 = (int) numArray24[index24] * 256;
      short num95 = (short) (num91 + num94);
      byte[] numArray25 = buffer1;
      int index25 = num93;
      int num96 = 1;
      int num97 = index25 + num96;
      int num98 = (int) numArray25[index25];
      byte[] numArray26 = buffer1;
      int index26 = num97;
      int num99 = 1;
      int num100 = index26 + num99;
      int num101 = (int) numArray26[index26] * 256;
      short extraFieldLength = (short) (num98 + num101);
      byte[] numArray27 = new byte[(int) num95];
      int num102 = ze.ArchiveStream.Read(numArray27, 0, numArray27.Length);
      int num103 = num4 + num102;
      if (((int) ze._BitField & 2048) == 2048)
      {
        ze.AlternateEncoding = Encoding.UTF8;
        ze.AlternateEncodingUsage = ZipOption.Always;
      }
      ze._FileNameInArchive = ze.AlternateEncoding.GetString(numArray27, 0, numArray27.Length);
      if (ze._FileNameInArchive.EndsWith("/"))
        ze.MarkAsDirectory();
      int num104 = num103 + ze.ProcessExtraField(ze.ArchiveStream, extraFieldLength);
      ze._LengthOfTrailer = 0;
      if (!ze._FileNameInArchive.EndsWith("/") && ((int) ze._BitField & 8) == 8)
      {
        long position = ze.ArchiveStream.Position;
        bool flag = true;
        long num105 = 0L;
        int num106 = 0;
        while (flag)
        {
          ++num106;
          if (ze._container.ZipFile != null)
            ze._container.ZipFile.OnReadBytes(ze);
          long signature2 = SharedUtilities.FindSignature(ze.ArchiveStream, 134695760);
          if (signature2 == -1L)
            return false;
          num105 += signature2;
          if (ze._InputUsesZip64)
          {
            byte[] buffer2 = new byte[20];
            if (ze.ArchiveStream.Read(buffer2, 0, buffer2.Length) != 20)
              return false;
            int num107 = 0;
            ZipEntry zipEntry9 = ze;
            byte[] numArray28 = buffer2;
            int index27 = num107;
            int num108 = 1;
            int num109 = index27 + num108;
            int num110 = (int) numArray28[index27];
            byte[] numArray29 = buffer2;
            int index28 = num109;
            int num111 = 1;
            int num112 = index28 + num111;
            int num113 = (int) numArray29[index28] * 256;
            int num114 = num110 + num113;
            byte[] numArray30 = buffer2;
            int index29 = num112;
            int num115 = 1;
            int num116 = index29 + num115;
            int num117 = (int) numArray30[index29] * 256 * 256;
            int num118 = num114 + num117;
            byte[] numArray31 = buffer2;
            int index30 = num116;
            int num119 = 1;
            int startIndex1 = index30 + num119;
            int num120 = (int) numArray31[index30] * 256 * 256 * 256;
            int num121 = num118 + num120;
            zipEntry9._Crc32 = num121;
            ze._CompressedSize = BitConverter.ToInt64(buffer2, startIndex1);
            int startIndex2 = startIndex1 + 8;
            ze._UncompressedSize = BitConverter.ToInt64(buffer2, startIndex2);
            num100 = startIndex2 + 8;
            ze._LengthOfTrailer += 24;
          }
          else
          {
            byte[] buffer2 = new byte[12];
            if (ze.ArchiveStream.Read(buffer2, 0, buffer2.Length) != 12)
              return false;
            int num107 = 0;
            ZipEntry zipEntry9 = ze;
            byte[] numArray28 = buffer2;
            int index27 = num107;
            int num108 = 1;
            int num109 = index27 + num108;
            int num110 = (int) numArray28[index27];
            byte[] numArray29 = buffer2;
            int index28 = num109;
            int num111 = 1;
            int num112 = index28 + num111;
            int num113 = (int) numArray29[index28] * 256;
            int num114 = num110 + num113;
            byte[] numArray30 = buffer2;
            int index29 = num112;
            int num115 = 1;
            int num116 = index29 + num115;
            int num117 = (int) numArray30[index29] * 256 * 256;
            int num118 = num114 + num117;
            byte[] numArray31 = buffer2;
            int index30 = num116;
            int num119 = 1;
            int num120 = index30 + num119;
            int num121 = (int) numArray31[index30] * 256 * 256 * 256;
            int num122 = num118 + num121;
            zipEntry9._Crc32 = num122;
            ZipEntry zipEntry10 = ze;
            byte[] numArray32 = buffer2;
            int index31 = num120;
            int num123 = 1;
            int num124 = index31 + num123;
            int num125 = (int) numArray32[index31];
            byte[] numArray33 = buffer2;
            int index32 = num124;
            int num126 = 1;
            int num127 = index32 + num126;
            int num128 = (int) numArray33[index32] * 256;
            int num129 = num125 + num128;
            byte[] numArray34 = buffer2;
            int index33 = num127;
            int num130 = 1;
            int num131 = index33 + num130;
            int num132 = (int) numArray34[index33] * 256 * 256;
            int num133 = num129 + num132;
            byte[] numArray35 = buffer2;
            int index34 = num131;
            int num134 = 1;
            int num135 = index34 + num134;
            int num136 = (int) numArray35[index34] * 256 * 256 * 256;
            long num137 = (long) (uint) (num133 + num136);
            zipEntry10._CompressedSize = num137;
            ZipEntry zipEntry11 = ze;
            byte[] numArray36 = buffer2;
            int index35 = num135;
            int num138 = 1;
            int num139 = index35 + num138;
            int num140 = (int) numArray36[index35];
            byte[] numArray37 = buffer2;
            int index36 = num139;
            int num141 = 1;
            int num142 = index36 + num141;
            int num143 = (int) numArray37[index36] * 256;
            int num144 = num140 + num143;
            byte[] numArray38 = buffer2;
            int index37 = num142;
            int num145 = 1;
            int num146 = index37 + num145;
            int num147 = (int) numArray38[index37] * 256 * 256;
            int num148 = num144 + num147;
            byte[] numArray39 = buffer2;
            int index38 = num146;
            int num149 = 1;
            num100 = index38 + num149;
            int num150 = (int) numArray39[index38] * 256 * 256 * 256;
            long num151 = (long) (uint) (num148 + num150);
            zipEntry11._UncompressedSize = num151;
            ze._LengthOfTrailer += 16;
          }
          flag = num105 != ze._CompressedSize;
          if (flag)
          {
            ze.ArchiveStream.Seek(-12L, SeekOrigin.Current);
            num105 += 4L;
          }
        }
        ze.ArchiveStream.Seek(position, SeekOrigin.Begin);
      }
      ze._CompressedFileDataSize = ze._CompressedSize;
      if (((int) ze._BitField & 1) == 1)
      {
        ze._WeakEncryptionHeader = new byte[12];
        num104 += ZipEntry.ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
        ze._CompressedFileDataSize -= 12L;
      }
      ze._LengthOfHeader = num104;
      ze._TotalEntrySize = (long) ze._LengthOfHeader + ze._CompressedFileDataSize + (long) ze._LengthOfTrailer;
      return true;
    }

    internal static int ReadWeakEncryptionHeader(Stream s, byte[] buffer)
    {
      int num = s.Read(buffer, 0, 12);
      if (num != 12)
        throw new ZipException(string.Format("Unexpected end of data at position 0x{0:X8}", (object) s.Position));
      return num;
    }

    private static bool IsNotValidSig(int signature)
    {
      return signature != 67324752;
    }

    internal static ZipEntry ReadEntry(ZipContainer zc, bool first)
    {
      ZipFile zipFile = zc.ZipFile;
      Stream readStream = zc.ReadStream;
      Encoding alternateEncoding = zc.AlternateEncoding;
      ZipEntry zipEntry = new ZipEntry();
      zipEntry._Source = ZipEntrySource.ZipFile;
      zipEntry._container = zc;
      zipEntry._archiveStream = readStream;
      if (zipFile != null)
        zipFile.OnReadEntry(true, (ZipEntry) null);
      if (first)
        ZipEntry.HandlePK00Prefix(readStream);
      if (!ZipEntry.ReadHeader(zipEntry, alternateEncoding))
        return (ZipEntry) null;
      zipEntry.__FileDataPosition = zipEntry.ArchiveStream.Position;
      readStream.Seek(zipEntry._CompressedFileDataSize + (long) zipEntry._LengthOfTrailer, SeekOrigin.Current);
      ZipEntry.HandleUnexpectedDataDescriptor(zipEntry);
      if (zipFile != null)
      {
        zipFile.OnReadBytes(zipEntry);
        zipFile.OnReadEntry(false, zipEntry);
      }
      return zipEntry;
    }

    internal static void HandlePK00Prefix(Stream s)
    {
      if (SharedUtilities.ReadInt(s) == 808471376)
        return;
      s.Seek(-4L, SeekOrigin.Current);
    }

    private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
    {
      Stream archiveStream = entry.ArchiveStream;
      if ((long) (uint) SharedUtilities.ReadInt(archiveStream) == (long) entry._Crc32)
      {
        if ((long) SharedUtilities.ReadInt(archiveStream) == entry._CompressedSize)
        {
          if ((long) SharedUtilities.ReadInt(archiveStream) == entry._UncompressedSize)
            return;
          archiveStream.Seek(-12L, SeekOrigin.Current);
        }
        else
          archiveStream.Seek(-8L, SeekOrigin.Current);
      }
      else
        archiveStream.Seek(-4L, SeekOrigin.Current);
    }

    internal static int FindExtraFieldSegment(byte[] extra, int offx, ushort targetHeaderId)
    {
      int num1;
      short num2;
      for (int index1 = offx; index1 + 3 < extra.Length; index1 = num1 + (int) num2)
      {
        byte[] numArray1 = extra;
        int index2 = index1;
        int num3 = 1;
        int num4 = index2 + num3;
        int num5 = (int) numArray1[index2];
        byte[] numArray2 = extra;
        int index3 = num4;
        int num6 = 1;
        int num7 = index3 + num6;
        int num8 = (int) numArray2[index3] * 256;
        if ((int) (ushort) (num5 + num8) == (int) targetHeaderId)
          return num7 - 2;
        byte[] numArray3 = extra;
        int index4 = num7;
        int num9 = 1;
        int num10 = index4 + num9;
        int num11 = (int) numArray3[index4];
        byte[] numArray4 = extra;
        int index5 = num10;
        int num12 = 1;
        num1 = index5 + num12;
        int num13 = (int) numArray4[index5] * 256;
        num2 = (short) (num11 + num13);
      }
      return -1;
    }

    internal int ProcessExtraField(Stream s, short extraFieldLength)
    {
      int num1 = 0;
      if ((int) extraFieldLength > 0)
      {
        byte[] numArray1 = this._Extra = new byte[(int) extraFieldLength];
        num1 = s.Read(numArray1, 0, numArray1.Length);
        long posn = s.Position - (long) num1;
        int num2;
        short dataSize;
        for (int index1 = 0; index1 + 3 < numArray1.Length; index1 = num2 + (int) dataSize + 4)
        {
          num2 = index1;
          byte[] numArray2 = numArray1;
          int index2 = index1;
          int num3 = 1;
          int num4 = index2 + num3;
          int num5 = (int) numArray2[index2];
          byte[] numArray3 = numArray1;
          int index3 = num4;
          int num6 = 1;
          int num7 = index3 + num6;
          int num8 = (int) numArray3[index3] * 256;
          ushort num9 = (ushort) (num5 + num8);
          byte[] numArray4 = numArray1;
          int index4 = num7;
          int num10 = 1;
          int num11 = index4 + num10;
          int num12 = (int) numArray4[index4];
          byte[] numArray5 = numArray1;
          int index5 = num11;
          int num13 = 1;
          int j = index5 + num13;
          int num14 = (int) numArray5[index5] * 256;
          dataSize = (short) (num12 + num14);
          ushort num15 = num9;
          int num16;
          if ((uint) num15 <= 23U)
          {
            if ((int) num15 != 1)
            {
              if ((int) num15 != 10)
              {
                if ((int) num15 == 23)
                  num16 = this.ProcessExtraFieldPkwareStrongEncryption(numArray1, j);
              }
              else
                num16 = this.ProcessExtraFieldWindowsTimes(numArray1, j, dataSize, posn);
            }
            else
              num16 = this.ProcessExtraFieldZip64(numArray1, j, dataSize, posn);
          }
          else if ((uint) num15 <= 22613U)
          {
            if ((int) num15 != 21589)
            {
              if ((int) num15 == 22613)
                num16 = this.ProcessExtraFieldInfoZipTimes(numArray1, j, dataSize, posn);
            }
            else
              num16 = this.ProcessExtraFieldUnixTimes(numArray1, j, dataSize, posn);
          }
          else if ((int) num15 == 30805 || (int) num15 == 30837)
            ;
        }
      }
      return num1;
    }

    private int ProcessExtraFieldPkwareStrongEncryption(byte[] Buffer, int j)
    {
      j += 2;
      this._UnsupportedAlgorithmId = (uint) (ushort) ((int) Buffer[j++] + (int) Buffer[j++] * 256);
      this._Encryption_FromZipFile = this._Encryption = EncryptionAlgorithm.Unsupported;
      return j;
    }

    private int ProcessExtraFieldZip64(byte[] buffer, int j, short dataSize, long posn)
    {
      this._InputUsesZip64 = true;
      if ((int) dataSize > 28)
        throw new BadReadException(string.Format("  Inconsistent size (0x{0:X4}) for ZIP64 extra field at position 0x{1:X16}", (object) dataSize, (object) posn));
      int remainingData = (int) dataSize;
      ZipEntry.Func<long> func = (ZipEntry.Func<long>) (() =>
      {
        if (remainingData < 8)
          throw new BadReadException(string.Format("  Missing data for ZIP64 extra field, position 0x{0:X16}", (object) posn));
        long num = BitConverter.ToInt64(buffer, j);
        j += 8;
        remainingData -= 8;
        return num;
      });
      if (this._UncompressedSize == (long) uint.MaxValue)
        this._UncompressedSize = func();
      if (this._CompressedSize == (long) uint.MaxValue)
        this._CompressedSize = func();
      if (this._RelativeOffsetOfLocalHeader == (long) uint.MaxValue)
        this._RelativeOffsetOfLocalHeader = func();
      return j;
    }

    private int ProcessExtraFieldInfoZipTimes(byte[] buffer, int j, short dataSize, long posn)
    {
      if ((int) dataSize != 12 && (int) dataSize != 8)
        throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for InfoZip v1 extra field at position 0x{1:X16}", (object) dataSize, (object) posn));
      int num1 = BitConverter.ToInt32(buffer, j);
      this._Mtime = ZipEntry._unixEpoch.AddSeconds((double) num1);
      j += 4;
      int num2 = BitConverter.ToInt32(buffer, j);
      this._Atime = ZipEntry._unixEpoch.AddSeconds((double) num2);
      j += 4;
      this._Ctime = DateTime.UtcNow;
      this._ntfsTimesAreSet = true;
      this._timestamp |= ZipEntryTimestamp.InfoZip1;
      return j;
    }

    private int ProcessExtraFieldUnixTimes(byte[] buffer, int j, short dataSize, long posn)
    {
      if ((int) dataSize != 13 && (int) dataSize != 9 && (int) dataSize != 5)
        throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for Extended Timestamp extra field at position 0x{1:X16}", (object) dataSize, (object) posn));
      int remainingData = (int) dataSize;
      ZipEntry.Func<DateTime> func = (ZipEntry.Func<DateTime>) (() =>
      {
        int num = BitConverter.ToInt32(buffer, j);
        j += 4;
        remainingData -= 4;
        return ZipEntry._unixEpoch.AddSeconds((double) num);
      });
      if ((int) dataSize == 13 || this._readExtraDepth > 0)
      {
        byte num = buffer[j++];
        --remainingData;
        if (((int) num & 1) != 0 && remainingData >= 4)
          this._Mtime = func();
        this._Atime = ((int) num & 2) == 0 || remainingData < 4 ? DateTime.UtcNow : func();
        this._Ctime = ((int) num & 4) == 0 || remainingData < 4 ? DateTime.UtcNow : func();
        this._timestamp |= ZipEntryTimestamp.Unix;
        this._ntfsTimesAreSet = true;
        this._emitUnixTimes = true;
      }
      else
        this.ReadExtraField();
      return j;
    }

    private int ProcessExtraFieldWindowsTimes(byte[] buffer, int j, short dataSize, long posn)
    {
      if ((int) dataSize != 32)
        throw new BadReadException(string.Format("  Unexpected size (0x{0:X4}) for NTFS times extra field at position 0x{1:X16}", (object) dataSize, (object) posn));
      j += 4;
      short num1 = (short) ((int) buffer[j] + (int) buffer[j + 1] * 256);
      short num2 = (short) ((int) buffer[j + 2] + (int) buffer[j + 3] * 256);
      j += 4;
      if ((int) num1 == 1 && (int) num2 == 24)
      {
        this._Mtime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(buffer, j));
        j += 8;
        this._Atime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(buffer, j));
        j += 8;
        this._Ctime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(buffer, j));
        j += 8;
        this._ntfsTimesAreSet = true;
        this._timestamp |= ZipEntryTimestamp.Windows;
        this._emitNtfsTimes = true;
      }
      return j;
    }

    internal void WriteCentralDirectoryEntry(Stream s)
    {
      byte[] buffer = new byte[4096];
      int num1 = 0;
      byte[] numArray1 = buffer;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2;
      int num4 = 80;
      numArray1[index1] = (byte) num4;
      byte[] numArray2 = buffer;
      int index2 = num3;
      int num5 = 1;
      int num6 = index2 + num5;
      int num7 = 75;
      numArray2[index2] = (byte) num7;
      byte[] numArray3 = buffer;
      int index3 = num6;
      int num8 = 1;
      int num9 = index3 + num8;
      int num10 = 1;
      numArray3[index3] = (byte) num10;
      byte[] numArray4 = buffer;
      int index4 = num9;
      int num11 = 1;
      int num12 = index4 + num11;
      int num13 = 2;
      numArray4[index4] = (byte) num13;
      byte[] numArray5 = buffer;
      int index5 = num12;
      int num14 = 1;
      int num15 = index5 + num14;
      int num16 = (int) (byte) ((uint) this._VersionMadeBy & (uint) byte.MaxValue);
      numArray5[index5] = (byte) num16;
      byte[] numArray6 = buffer;
      int index6 = num15;
      int num17 = 1;
      int num18 = index6 + num17;
      int num19 = (int) (byte) (((int) this._VersionMadeBy & 65280) >> 8);
      numArray6[index6] = (byte) num19;
      short num20 = (int) this.VersionNeeded != 0 ? this.VersionNeeded : (short) 20;
      if (!this._OutputUsesZip64.HasValue)
        this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always);
      short num21 = this._OutputUsesZip64.Value ? (short) 45 : num20;
      byte[] numArray7 = buffer;
      int index7 = num18;
      int num22 = 1;
      int num23 = index7 + num22;
      int num24 = (int) (byte) ((uint) num21 & (uint) byte.MaxValue);
      numArray7[index7] = (byte) num24;
      byte[] numArray8 = buffer;
      int index8 = num23;
      int num25 = 1;
      int num26 = index8 + num25;
      int num27 = (int) (byte) (((int) num21 & 65280) >> 8);
      numArray8[index8] = (byte) num27;
      byte[] numArray9 = buffer;
      int index9 = num26;
      int num28 = 1;
      int num29 = index9 + num28;
      int num30 = (int) (byte) ((uint) this._BitField & (uint) byte.MaxValue);
      numArray9[index9] = (byte) num30;
      byte[] numArray10 = buffer;
      int index10 = num29;
      int num31 = 1;
      int num32 = index10 + num31;
      int num33 = (int) (byte) (((int) this._BitField & 65280) >> 8);
      numArray10[index10] = (byte) num33;
      byte[] numArray11 = buffer;
      int index11 = num32;
      int num34 = 1;
      int num35 = index11 + num34;
      int num36 = (int) (byte) ((uint) this._CompressionMethod & (uint) byte.MaxValue);
      numArray11[index11] = (byte) num36;
      byte[] numArray12 = buffer;
      int index12 = num35;
      int num37 = 1;
      int num38 = index12 + num37;
      int num39 = (int) (byte) (((int) this._CompressionMethod & 65280) >> 8);
      numArray12[index12] = (byte) num39;
      byte[] numArray13 = buffer;
      int index13 = num38;
      int num40 = 1;
      int num41 = index13 + num40;
      int num42 = (int) (byte) (this._TimeBlob & (int) byte.MaxValue);
      numArray13[index13] = (byte) num42;
      byte[] numArray14 = buffer;
      int index14 = num41;
      int num43 = 1;
      int num44 = index14 + num43;
      int num45 = (int) (byte) ((this._TimeBlob & 65280) >> 8);
      numArray14[index14] = (byte) num45;
      byte[] numArray15 = buffer;
      int index15 = num44;
      int num46 = 1;
      int num47 = index15 + num46;
      int num48 = (int) (byte) ((this._TimeBlob & 16711680) >> 16);
      numArray15[index15] = (byte) num48;
      byte[] numArray16 = buffer;
      int index16 = num47;
      int num49 = 1;
      int num50 = index16 + num49;
      int num51 = (int) (byte) (((long) this._TimeBlob & 4278190080L) >> 24);
      numArray16[index16] = (byte) num51;
      byte[] numArray17 = buffer;
      int index17 = num50;
      int num52 = 1;
      int num53 = index17 + num52;
      int num54 = (int) (byte) (this._Crc32 & (int) byte.MaxValue);
      numArray17[index17] = (byte) num54;
      byte[] numArray18 = buffer;
      int index18 = num53;
      int num55 = 1;
      int num56 = index18 + num55;
      int num57 = (int) (byte) ((this._Crc32 & 65280) >> 8);
      numArray18[index18] = (byte) num57;
      byte[] numArray19 = buffer;
      int index19 = num56;
      int num58 = 1;
      int num59 = index19 + num58;
      int num60 = (int) (byte) ((this._Crc32 & 16711680) >> 16);
      numArray19[index19] = (byte) num60;
      byte[] numArray20 = buffer;
      int index20 = num59;
      int num61 = 1;
      int num62 = index20 + num61;
      int num63 = (int) (byte) (((long) this._Crc32 & 4278190080L) >> 24);
      numArray20[index20] = (byte) num63;
      if (this._OutputUsesZip64.Value)
      {
        for (int index21 = 0; index21 < 8; ++index21)
          buffer[num62++] = byte.MaxValue;
      }
      else
      {
        byte[] numArray21 = buffer;
        int index21 = num62;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = (int) (byte) ((ulong) this._CompressedSize & (ulong) byte.MaxValue);
        numArray21[index21] = (byte) num66;
        byte[] numArray22 = buffer;
        int index22 = num65;
        int num67 = 1;
        int num68 = index22 + num67;
        int num69 = (int) (byte) ((this._CompressedSize & 65280L) >> 8);
        numArray22[index22] = (byte) num69;
        byte[] numArray23 = buffer;
        int index23 = num68;
        int num70 = 1;
        int num71 = index23 + num70;
        int num72 = (int) (byte) ((this._CompressedSize & 16711680L) >> 16);
        numArray23[index23] = (byte) num72;
        byte[] numArray24 = buffer;
        int index24 = num71;
        int num73 = 1;
        int num74 = index24 + num73;
        int num75 = (int) (byte) ((this._CompressedSize & 4278190080L) >> 24);
        numArray24[index24] = (byte) num75;
        byte[] numArray25 = buffer;
        int index25 = num74;
        int num76 = 1;
        int num77 = index25 + num76;
        int num78 = (int) (byte) ((ulong) this._UncompressedSize & (ulong) byte.MaxValue);
        numArray25[index25] = (byte) num78;
        byte[] numArray26 = buffer;
        int index26 = num77;
        int num79 = 1;
        int num80 = index26 + num79;
        int num81 = (int) (byte) ((this._UncompressedSize & 65280L) >> 8);
        numArray26[index26] = (byte) num81;
        byte[] numArray27 = buffer;
        int index27 = num80;
        int num82 = 1;
        int num83 = index27 + num82;
        int num84 = (int) (byte) ((this._UncompressedSize & 16711680L) >> 16);
        numArray27[index27] = (byte) num84;
        byte[] numArray28 = buffer;
        int index28 = num83;
        int num85 = 1;
        num62 = index28 + num85;
        int num86 = (int) (byte) ((this._UncompressedSize & 4278190080L) >> 24);
        numArray28[index28] = (byte) num86;
      }
      byte[] encodedFileNameBytes = this.GetEncodedFileNameBytes();
      short num87 = (short) encodedFileNameBytes.Length;
      byte[] numArray29 = buffer;
      int index29 = num62;
      int num88 = 1;
      int num89 = index29 + num88;
      int num90 = (int) (byte) ((uint) num87 & (uint) byte.MaxValue);
      numArray29[index29] = (byte) num90;
      byte[] numArray30 = buffer;
      int index30 = num89;
      int num91 = 1;
      int num92 = index30 + num91;
      int num93 = (int) (byte) (((int) num87 & 65280) >> 8);
      numArray30[index30] = (byte) num93;
      this._presumeZip64 = this._OutputUsesZip64.Value;
      this._Extra = this.ConstructExtraField(true);
      short num94 = this._Extra == null ? (short) 0 : (short) this._Extra.Length;
      byte[] numArray31 = buffer;
      int index31 = num92;
      int num95 = 1;
      int num96 = index31 + num95;
      int num97 = (int) (byte) ((uint) num94 & (uint) byte.MaxValue);
      numArray31[index31] = (byte) num97;
      byte[] numArray32 = buffer;
      int index32 = num96;
      int num98 = 1;
      int num99 = index32 + num98;
      int num100 = (int) (byte) (((int) num94 & 65280) >> 8);
      numArray32[index32] = (byte) num100;
      int count = this._CommentBytes == null ? 0 : this._CommentBytes.Length;
      if (count + num99 > buffer.Length)
        count = buffer.Length - num99;
      byte[] numArray33 = buffer;
      int index33 = num99;
      int num101 = 1;
      int num102 = index33 + num101;
      int num103 = (int) (byte) (count & (int) byte.MaxValue);
      numArray33[index33] = (byte) num103;
      byte[] numArray34 = buffer;
      int index34 = num102;
      int num104 = 1;
      int num105 = index34 + num104;
      int num106 = (int) (byte) ((count & 65280) >> 8);
      numArray34[index34] = (byte) num106;
      int num107;
      if (this._container.ZipFile != null && this._container.ZipFile.MaxOutputSegmentSize != 0)
      {
        byte[] numArray21 = buffer;
        int index21 = num105;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = (int) (byte) (this._diskNumber & (uint) byte.MaxValue);
        numArray21[index21] = (byte) num66;
        byte[] numArray22 = buffer;
        int index22 = num65;
        int num67 = 1;
        num107 = index22 + num67;
        int num68 = (int) (byte) ((this._diskNumber & 65280U) >> 8);
        numArray22[index22] = (byte) num68;
      }
      else
      {
        byte[] numArray21 = buffer;
        int index21 = num105;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = 0;
        numArray21[index21] = (byte) num66;
        byte[] numArray22 = buffer;
        int index22 = num65;
        int num67 = 1;
        num107 = index22 + num67;
        int num68 = 0;
        numArray22[index22] = (byte) num68;
      }
      byte[] numArray35 = buffer;
      int index35 = num107;
      int num108 = 1;
      int num109 = index35 + num108;
      int num110 = this._IsText ? 1 : 0;
      numArray35[index35] = (byte) num110;
      byte[] numArray36 = buffer;
      int index36 = num109;
      int num111 = 1;
      int num112 = index36 + num111;
      int num113 = 0;
      numArray36[index36] = (byte) num113;
      byte[] numArray37 = buffer;
      int index37 = num112;
      int num114 = 1;
      int num115 = index37 + num114;
      int num116 = (int) (byte) (this._ExternalFileAttrs & (int) byte.MaxValue);
      numArray37[index37] = (byte) num116;
      byte[] numArray38 = buffer;
      int index38 = num115;
      int num117 = 1;
      int num118 = index38 + num117;
      int num119 = (int) (byte) ((this._ExternalFileAttrs & 65280) >> 8);
      numArray38[index38] = (byte) num119;
      byte[] numArray39 = buffer;
      int index39 = num118;
      int num120 = 1;
      int num121 = index39 + num120;
      int num122 = (int) (byte) ((this._ExternalFileAttrs & 16711680) >> 16);
      numArray39[index39] = (byte) num122;
      byte[] numArray40 = buffer;
      int index40 = num121;
      int num123 = 1;
      int num124 = index40 + num123;
      int num125 = (int) (byte) (((long) this._ExternalFileAttrs & 4278190080L) >> 24);
      numArray40[index40] = (byte) num125;
      int dstOffset;
      if (this._RelativeOffsetOfLocalHeader > (long) uint.MaxValue)
      {
        byte[] numArray21 = buffer;
        int index21 = num124;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = (int) byte.MaxValue;
        numArray21[index21] = (byte) num66;
        byte[] numArray22 = buffer;
        int index22 = num65;
        int num67 = 1;
        int num68 = index22 + num67;
        int num69 = (int) byte.MaxValue;
        numArray22[index22] = (byte) num69;
        byte[] numArray23 = buffer;
        int index23 = num68;
        int num70 = 1;
        int num71 = index23 + num70;
        int num72 = (int) byte.MaxValue;
        numArray23[index23] = (byte) num72;
        byte[] numArray24 = buffer;
        int index24 = num71;
        int num73 = 1;
        dstOffset = index24 + num73;
        int num74 = (int) byte.MaxValue;
        numArray24[index24] = (byte) num74;
      }
      else
      {
        byte[] numArray21 = buffer;
        int index21 = num124;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = (int) (byte) ((ulong) this._RelativeOffsetOfLocalHeader & (ulong) byte.MaxValue);
        numArray21[index21] = (byte) num66;
        byte[] numArray22 = buffer;
        int index22 = num65;
        int num67 = 1;
        int num68 = index22 + num67;
        int num69 = (int) (byte) ((this._RelativeOffsetOfLocalHeader & 65280L) >> 8);
        numArray22[index22] = (byte) num69;
        byte[] numArray23 = buffer;
        int index23 = num68;
        int num70 = 1;
        int num71 = index23 + num70;
        int num72 = (int) (byte) ((this._RelativeOffsetOfLocalHeader & 16711680L) >> 16);
        numArray23[index23] = (byte) num72;
        byte[] numArray24 = buffer;
        int index24 = num71;
        int num73 = 1;
        dstOffset = index24 + num73;
        int num74 = (int) (byte) ((this._RelativeOffsetOfLocalHeader & 4278190080L) >> 24);
        numArray24[index24] = (byte) num74;
      }
      Buffer.BlockCopy((Array) encodedFileNameBytes, 0, (Array) buffer, dstOffset, (int) num87);
      int num126 = dstOffset + (int) num87;
      if (this._Extra != null)
      {
        Buffer.BlockCopy((Array) this._Extra, 0, (Array) buffer, num126, (int) num94);
        num126 += (int) num94;
      }
      if (count != 0)
      {
        Buffer.BlockCopy((Array) this._CommentBytes, 0, (Array) buffer, num126, count);
        num126 += count;
      }
      s.Write(buffer, 0, num126);
    }

    private byte[] ConstructExtraField(bool forCentralDirectory)
    {
      List<byte[]> list = new List<byte[]>();
      if (this._container.Zip64 == Zip64Option.Always || this._container.Zip64 == Zip64Option.AsNecessary && (!forCentralDirectory || this._entryRequiresZip64.Value))
      {
        int length = 4 + (forCentralDirectory ? 28 : 16);
        byte[] numArray1 = new byte[length];
        int num1 = 0;
        int num2;
        if (this._presumeZip64 || forCentralDirectory)
        {
          byte[] numArray2 = numArray1;
          int index1 = num1;
          int num3 = 1;
          int num4 = index1 + num3;
          int num5 = 1;
          numArray2[index1] = (byte) num5;
          byte[] numArray3 = numArray1;
          int index2 = num4;
          int num6 = 1;
          num2 = index2 + num6;
          int num7 = 0;
          numArray3[index2] = (byte) num7;
        }
        else
        {
          byte[] numArray2 = numArray1;
          int index1 = num1;
          int num3 = 1;
          int num4 = index1 + num3;
          int num5 = 153;
          numArray2[index1] = (byte) num5;
          byte[] numArray3 = numArray1;
          int index2 = num4;
          int num6 = 1;
          num2 = index2 + num6;
          int num7 = 153;
          numArray3[index2] = (byte) num7;
        }
        byte[] numArray4 = numArray1;
        int index3 = num2;
        int num8 = 1;
        int num9 = index3 + num8;
        int num10 = (int) (byte) (length - 4);
        numArray4[index3] = (byte) num10;
        byte[] numArray5 = numArray1;
        int index4 = num9;
        int num11 = 1;
        int destinationIndex1 = index4 + num11;
        int num12 = 0;
        numArray5[index4] = (byte) num12;
        Array.Copy((Array) BitConverter.GetBytes(this._UncompressedSize), 0, (Array) numArray1, destinationIndex1, 8);
        int destinationIndex2 = destinationIndex1 + 8;
        Array.Copy((Array) BitConverter.GetBytes(this._CompressedSize), 0, (Array) numArray1, destinationIndex2, 8);
        int destinationIndex3 = destinationIndex2 + 8;
        if (forCentralDirectory)
        {
          Array.Copy((Array) BitConverter.GetBytes(this._RelativeOffsetOfLocalHeader), 0, (Array) numArray1, destinationIndex3, 8);
          int destinationIndex4 = destinationIndex3 + 8;
          Array.Copy((Array) BitConverter.GetBytes(0), 0, (Array) numArray1, destinationIndex4, 4);
        }
        list.Add(numArray1);
      }
      if (this._ntfsTimesAreSet && this._emitNtfsTimes)
      {
        byte[] numArray1 = new byte[36];
        int num1 = 0;
        byte[] numArray2 = numArray1;
        int index1 = num1;
        int num2 = 1;
        int num3 = index1 + num2;
        int num4 = 10;
        numArray2[index1] = (byte) num4;
        byte[] numArray3 = numArray1;
        int index2 = num3;
        int num5 = 1;
        int num6 = index2 + num5;
        int num7 = 0;
        numArray3[index2] = (byte) num7;
        byte[] numArray4 = numArray1;
        int index3 = num6;
        int num8 = 1;
        int num9 = index3 + num8;
        int num10 = 32;
        numArray4[index3] = (byte) num10;
        byte[] numArray5 = numArray1;
        int index4 = num9;
        int num11 = 1;
        int num12 = index4 + num11;
        int num13 = 0;
        numArray5[index4] = (byte) num13;
        int num14 = num12 + 4;
        byte[] numArray6 = numArray1;
        int index5 = num14;
        int num15 = 1;
        int num16 = index5 + num15;
        int num17 = 1;
        numArray6[index5] = (byte) num17;
        byte[] numArray7 = numArray1;
        int index6 = num16;
        int num18 = 1;
        int num19 = index6 + num18;
        int num20 = 0;
        numArray7[index6] = (byte) num20;
        byte[] numArray8 = numArray1;
        int index7 = num19;
        int num21 = 1;
        int num22 = index7 + num21;
        int num23 = 24;
        numArray8[index7] = (byte) num23;
        byte[] numArray9 = numArray1;
        int index8 = num22;
        int num24 = 1;
        int destinationIndex1 = index8 + num24;
        int num25 = 0;
        numArray9[index8] = (byte) num25;
        Array.Copy((Array) BitConverter.GetBytes(this._Mtime.ToFileTime()), 0, (Array) numArray1, destinationIndex1, 8);
        int destinationIndex2 = destinationIndex1 + 8;
        Array.Copy((Array) BitConverter.GetBytes(this._Atime.ToFileTime()), 0, (Array) numArray1, destinationIndex2, 8);
        int destinationIndex3 = destinationIndex2 + 8;
        Array.Copy((Array) BitConverter.GetBytes(this._Ctime.ToFileTime()), 0, (Array) numArray1, destinationIndex3, 8);
        int num26 = destinationIndex3 + 8;
        list.Add(numArray1);
      }
      if (this._ntfsTimesAreSet && this._emitUnixTimes)
      {
        int length = 9;
        if (!forCentralDirectory)
          length += 8;
        byte[] numArray1 = new byte[length];
        int num1 = 0;
        byte[] numArray2 = numArray1;
        int index1 = num1;
        int num2 = 1;
        int num3 = index1 + num2;
        int num4 = 85;
        numArray2[index1] = (byte) num4;
        byte[] numArray3 = numArray1;
        int index2 = num3;
        int num5 = 1;
        int num6 = index2 + num5;
        int num7 = 84;
        numArray3[index2] = (byte) num7;
        byte[] numArray4 = numArray1;
        int index3 = num6;
        int num8 = 1;
        int num9 = index3 + num8;
        int num10 = (int) (byte) (length - 4);
        numArray4[index3] = (byte) num10;
        byte[] numArray5 = numArray1;
        int index4 = num9;
        int num11 = 1;
        int num12 = index4 + num11;
        int num13 = 0;
        numArray5[index4] = (byte) num13;
        byte[] numArray6 = numArray1;
        int index5 = num12;
        int num14 = 1;
        int destinationIndex1 = index5 + num14;
        int num15 = 7;
        numArray6[index5] = (byte) num15;
        Array.Copy((Array) BitConverter.GetBytes((int) (this._Mtime - ZipEntry._unixEpoch).TotalSeconds), 0, (Array) numArray1, destinationIndex1, 4);
        int destinationIndex2 = destinationIndex1 + 4;
        if (!forCentralDirectory)
        {
          Array.Copy((Array) BitConverter.GetBytes((int) (this._Atime - ZipEntry._unixEpoch).TotalSeconds), 0, (Array) numArray1, destinationIndex2, 4);
          int destinationIndex3 = destinationIndex2 + 4;
          Array.Copy((Array) BitConverter.GetBytes((int) (this._Ctime - ZipEntry._unixEpoch).TotalSeconds), 0, (Array) numArray1, destinationIndex3, 4);
          int num16 = destinationIndex3 + 4;
        }
        list.Add(numArray1);
      }
      byte[] numArray = (byte[]) null;
      if (list.Count > 0)
      {
        int length = 0;
        int destinationIndex = 0;
        for (int index = 0; index < list.Count; ++index)
          length += list[index].Length;
        numArray = new byte[length];
        for (int index = 0; index < list.Count; ++index)
        {
          Array.Copy((Array) list[index], 0, (Array) numArray, destinationIndex, list[index].Length);
          destinationIndex += list[index].Length;
        }
      }
      return numArray;
    }

    private string NormalizeFileName()
    {
      string str1 = this.FileName.Replace("\\", "/");
      string str2;
      if (this._TrimVolumeFromFullyQualifiedPaths && this.FileName.Length >= 3 && ((int) this.FileName[1] == 58 && (int) str1[2] == 47))
        str2 = str1.Substring(3);
      else if (this.FileName.Length >= 4 && (int) str1[0] == 47 && (int) str1[1] == 47)
      {
        int num = str1.IndexOf('/', 2);
        if (num == -1)
          throw new ArgumentException("The path for that entry appears to be badly formatted");
        str2 = str1.Substring(num + 1);
      }
      else
        str2 = this.FileName.Length < 3 || (int) str1[0] != 46 || (int) str1[1] != 47 ? str1 : str1.Substring(2);
      return str2;
    }

    private byte[] GetEncodedFileNameBytes()
    {
      string s = this.NormalizeFileName();
      switch (this.AlternateEncodingUsage)
      {
        case ZipOption.Default:
          if (this._Comment != null && this._Comment.Length != 0)
            this._CommentBytes = ZipEntry.ibm437.GetBytes(this._Comment);
          this._actualEncoding = ZipEntry.ibm437;
          return ZipEntry.ibm437.GetBytes(s);
        case ZipOption.Always:
          if (this._Comment != null && this._Comment.Length != 0)
            this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
          this._actualEncoding = this.AlternateEncoding;
          return this.AlternateEncoding.GetBytes(s);
        default:
          byte[] bytes1 = ZipEntry.ibm437.GetBytes(s);
          string @string = ZipEntry.ibm437.GetString(bytes1, 0, bytes1.Length);
          this._CommentBytes = (byte[]) null;
          if (@string != s)
          {
            byte[] bytes2 = this.AlternateEncoding.GetBytes(s);
            if (this._Comment != null && this._Comment.Length != 0)
              this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
            this._actualEncoding = this.AlternateEncoding;
            return bytes2;
          }
          this._actualEncoding = ZipEntry.ibm437;
          if (this._Comment == null || this._Comment.Length == 0)
            return bytes1;
          byte[] bytes3 = ZipEntry.ibm437.GetBytes(this._Comment);
          if (ZipEntry.ibm437.GetString(bytes3, 0, bytes3.Length) != this.Comment)
          {
            byte[] bytes2 = this.AlternateEncoding.GetBytes(s);
            this._CommentBytes = this.AlternateEncoding.GetBytes(this._Comment);
            this._actualEncoding = this.AlternateEncoding;
            return bytes2;
          }
          this._CommentBytes = bytes3;
          return bytes1;
      }
    }

    private bool WantReadAgain()
    {
      return this._UncompressedSize >= 16L && (int) this._CompressionMethod != 0 && (this.CompressionLevel != CompressionLevel.None && this._CompressedSize >= this._UncompressedSize) && ((this._Source != ZipEntrySource.Stream || this._sourceStream.CanSeek) && (this._zipCrypto_forWrite == null || this.CompressedSize - 12L > this.UncompressedSize));
    }

    private void MaybeUnsetCompressionMethodForWriting(int cycle)
    {
      if (cycle > 1)
        this._CompressionMethod = (short) 0;
      else if (this.IsDirectory)
      {
        this._CompressionMethod = (short) 0;
      }
      else
      {
        if (this._Source == ZipEntrySource.ZipFile)
          return;
        if (this._Source == ZipEntrySource.Stream)
        {
          if (this._sourceStream != null && this._sourceStream.CanSeek && this._sourceStream.Length == 0L)
          {
            this._CompressionMethod = (short) 0;
            return;
          }
        }
        else if (this._Source == ZipEntrySource.FileSystem && SharedUtilities.GetFileLength(this.LocalFileName) == 0L)
        {
          this._CompressionMethod = (short) 0;
          return;
        }
        if (this.SetCompression != null)
          this.CompressionLevel = this.SetCompression(this.LocalFileName, this._FileNameInArchive);
        if (this.CompressionLevel != CompressionLevel.None || this.CompressionMethod != CompressionMethod.Deflate)
          return;
        this._CompressionMethod = (short) 0;
      }
    }

    internal void WriteHeader(Stream s, int cycle)
    {
      CountingStream countingStream = s as CountingStream;
      this._future_ROLH = countingStream != null ? countingStream.ComputedPosition : s.Position;
      int num1 = 0;
      byte[] numArray1 = new byte[30];
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2;
      int num4 = 80;
      numArray2[index1] = (byte) num4;
      byte[] numArray3 = numArray1;
      int index2 = num3;
      int num5 = 1;
      int num6 = index2 + num5;
      int num7 = 75;
      numArray3[index2] = (byte) num7;
      byte[] numArray4 = numArray1;
      int index3 = num6;
      int num8 = 1;
      int num9 = index3 + num8;
      int num10 = 3;
      numArray4[index3] = (byte) num10;
      byte[] numArray5 = numArray1;
      int index4 = num9;
      int num11 = 1;
      int num12 = index4 + num11;
      int num13 = 4;
      numArray5[index4] = (byte) num13;
      this._presumeZip64 = this._container.Zip64 == Zip64Option.Always || this._container.Zip64 == Zip64Option.AsNecessary && !s.CanSeek;
      short num14 = this._presumeZip64 ? (short) 45 : (short) 20;
      byte[] numArray6 = numArray1;
      int index5 = num12;
      int num15 = 1;
      int num16 = index5 + num15;
      int num17 = (int) (byte) ((uint) num14 & (uint) byte.MaxValue);
      numArray6[index5] = (byte) num17;
      byte[] numArray7 = numArray1;
      int index6 = num16;
      int num18 = 1;
      int num19 = index6 + num18;
      int num20 = (int) (byte) (((int) num14 & 65280) >> 8);
      numArray7[index6] = (byte) num20;
      byte[] encodedFileNameBytes = this.GetEncodedFileNameBytes();
      short num21 = (short) encodedFileNameBytes.Length;
      if (this._Encryption == EncryptionAlgorithm.None)
        this._BitField &= (short) -2;
      else
        this._BitField |= (short) 1;
      if (this._actualEncoding.CodePage == Encoding.UTF8.CodePage)
        this._BitField |= (short) 2048;
      if (this.IsDirectory || cycle == 99)
      {
        this._BitField &= (short) -9;
        this._BitField &= (short) -2;
        this.Encryption = EncryptionAlgorithm.None;
        this.Password = (string) null;
      }
      else if (!s.CanSeek)
        this._BitField |= (short) 8;
      byte[] numArray8 = numArray1;
      int index7 = num19;
      int num22 = 1;
      int num23 = index7 + num22;
      int num24 = (int) (byte) ((uint) this._BitField & (uint) byte.MaxValue);
      numArray8[index7] = (byte) num24;
      byte[] numArray9 = numArray1;
      int index8 = num23;
      int num25 = 1;
      int num26 = index8 + num25;
      int num27 = (int) (byte) (((int) this._BitField & 65280) >> 8);
      numArray9[index8] = (byte) num27;
      if (this.__FileDataPosition == -1L)
      {
        this._CompressedSize = 0L;
        this._crcCalculated = false;
      }
      this.MaybeUnsetCompressionMethodForWriting(cycle);
      byte[] numArray10 = numArray1;
      int index9 = num26;
      int num28 = 1;
      int num29 = index9 + num28;
      int num30 = (int) (byte) ((uint) this._CompressionMethod & (uint) byte.MaxValue);
      numArray10[index9] = (byte) num30;
      byte[] numArray11 = numArray1;
      int index10 = num29;
      int num31 = 1;
      int num32 = index10 + num31;
      int num33 = (int) (byte) (((int) this._CompressionMethod & 65280) >> 8);
      numArray11[index10] = (byte) num33;
      if (cycle == 99)
        this.SetZip64Flags();
      this._TimeBlob = SharedUtilities.DateTimeToPacked(this.LastModified);
      byte[] numArray12 = numArray1;
      int index11 = num32;
      int num34 = 1;
      int num35 = index11 + num34;
      int num36 = (int) (byte) (this._TimeBlob & (int) byte.MaxValue);
      numArray12[index11] = (byte) num36;
      byte[] numArray13 = numArray1;
      int index12 = num35;
      int num37 = 1;
      int num38 = index12 + num37;
      int num39 = (int) (byte) ((this._TimeBlob & 65280) >> 8);
      numArray13[index12] = (byte) num39;
      byte[] numArray14 = numArray1;
      int index13 = num38;
      int num40 = 1;
      int num41 = index13 + num40;
      int num42 = (int) (byte) ((this._TimeBlob & 16711680) >> 16);
      numArray14[index13] = (byte) num42;
      byte[] numArray15 = numArray1;
      int index14 = num41;
      int num43 = 1;
      int num44 = index14 + num43;
      int num45 = (int) (byte) (((long) this._TimeBlob & 4278190080L) >> 24);
      numArray15[index14] = (byte) num45;
      byte[] numArray16 = numArray1;
      int index15 = num44;
      int num46 = 1;
      int num47 = index15 + num46;
      int num48 = (int) (byte) (this._Crc32 & (int) byte.MaxValue);
      numArray16[index15] = (byte) num48;
      byte[] numArray17 = numArray1;
      int index16 = num47;
      int num49 = 1;
      int num50 = index16 + num49;
      int num51 = (int) (byte) ((this._Crc32 & 65280) >> 8);
      numArray17[index16] = (byte) num51;
      byte[] numArray18 = numArray1;
      int index17 = num50;
      int num52 = 1;
      int num53 = index17 + num52;
      int num54 = (int) (byte) ((this._Crc32 & 16711680) >> 16);
      numArray18[index17] = (byte) num54;
      byte[] numArray19 = numArray1;
      int index18 = num53;
      int num55 = 1;
      int num56 = index18 + num55;
      int num57 = (int) (byte) (((long) this._Crc32 & 4278190080L) >> 24);
      numArray19[index18] = (byte) num57;
      if (this._presumeZip64)
      {
        for (int index19 = 0; index19 < 8; ++index19)
          numArray1[num56++] = byte.MaxValue;
      }
      else
      {
        byte[] numArray20 = numArray1;
        int index19 = num56;
        int num58 = 1;
        int num59 = index19 + num58;
        int num60 = (int) (byte) ((ulong) this._CompressedSize & (ulong) byte.MaxValue);
        numArray20[index19] = (byte) num60;
        byte[] numArray21 = numArray1;
        int index20 = num59;
        int num61 = 1;
        int num62 = index20 + num61;
        int num63 = (int) (byte) ((this._CompressedSize & 65280L) >> 8);
        numArray21[index20] = (byte) num63;
        byte[] numArray22 = numArray1;
        int index21 = num62;
        int num64 = 1;
        int num65 = index21 + num64;
        int num66 = (int) (byte) ((this._CompressedSize & 16711680L) >> 16);
        numArray22[index21] = (byte) num66;
        byte[] numArray23 = numArray1;
        int index22 = num65;
        int num67 = 1;
        int num68 = index22 + num67;
        int num69 = (int) (byte) ((this._CompressedSize & 4278190080L) >> 24);
        numArray23[index22] = (byte) num69;
        byte[] numArray24 = numArray1;
        int index23 = num68;
        int num70 = 1;
        int num71 = index23 + num70;
        int num72 = (int) (byte) ((ulong) this._UncompressedSize & (ulong) byte.MaxValue);
        numArray24[index23] = (byte) num72;
        byte[] numArray25 = numArray1;
        int index24 = num71;
        int num73 = 1;
        int num74 = index24 + num73;
        int num75 = (int) (byte) ((this._UncompressedSize & 65280L) >> 8);
        numArray25[index24] = (byte) num75;
        byte[] numArray26 = numArray1;
        int index25 = num74;
        int num76 = 1;
        int num77 = index25 + num76;
        int num78 = (int) (byte) ((this._UncompressedSize & 16711680L) >> 16);
        numArray26[index25] = (byte) num78;
        byte[] numArray27 = numArray1;
        int index26 = num77;
        int num79 = 1;
        num56 = index26 + num79;
        int num80 = (int) (byte) ((this._UncompressedSize & 4278190080L) >> 24);
        numArray27[index26] = (byte) num80;
      }
      byte[] numArray28 = numArray1;
      int index27 = num56;
      int num81 = 1;
      int num82 = index27 + num81;
      int num83 = (int) (byte) ((uint) num21 & (uint) byte.MaxValue);
      numArray28[index27] = (byte) num83;
      byte[] numArray29 = numArray1;
      int index28 = num82;
      int num84 = 1;
      int num85 = index28 + num84;
      int num86 = (int) (byte) (((int) num21 & 65280) >> 8);
      numArray29[index28] = (byte) num86;
      this._Extra = this.ConstructExtraField(false);
      short num87 = this._Extra == null ? (short) 0 : (short) this._Extra.Length;
      byte[] numArray30 = numArray1;
      int index29 = num85;
      int num88 = 1;
      int num89 = index29 + num88;
      int num90 = (int) (byte) ((uint) num87 & (uint) byte.MaxValue);
      numArray30[index29] = (byte) num90;
      byte[] numArray31 = numArray1;
      int index30 = num89;
      int num91 = 1;
      int num92 = index30 + num91;
      int num93 = (int) (byte) (((int) num87 & 65280) >> 8);
      numArray31[index30] = (byte) num93;
      byte[] buffer = new byte[num92 + (int) num21 + (int) num87];
      Buffer.BlockCopy((Array) numArray1, 0, (Array) buffer, 0, num92);
      Buffer.BlockCopy((Array) encodedFileNameBytes, 0, (Array) buffer, num92, encodedFileNameBytes.Length);
      int num94 = num92 + encodedFileNameBytes.Length;
      if (this._Extra != null)
      {
        Buffer.BlockCopy((Array) this._Extra, 0, (Array) buffer, num94, this._Extra.Length);
        num94 += this._Extra.Length;
      }
      this._LengthOfHeader = num94;
      ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
      if (zipSegmentedStream != null)
      {
        zipSegmentedStream.ContiguousWrite = true;
        uint segment = zipSegmentedStream.ComputeSegment(num94);
        this._future_ROLH = (int) segment == (int) zipSegmentedStream.CurrentSegment ? zipSegmentedStream.Position : 0L;
        this._diskNumber = segment;
      }
      if (this._container.Zip64 == Zip64Option.Default && (uint) this._RelativeOffsetOfLocalHeader >= uint.MaxValue)
        throw new ZipException("Offset within the zip archive exceeds 0xFFFFFFFF. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
      s.Write(buffer, 0, num94);
      if (zipSegmentedStream != null)
        zipSegmentedStream.ContiguousWrite = false;
      this._EntryHeader = buffer;
    }

    private int FigureCrc32()
    {
      if (!this._crcCalculated)
      {
        Stream input = (Stream) null;
        if (this._Source == ZipEntrySource.WriteDelegate)
        {
          CrcCalculatorStream calculatorStream = new CrcCalculatorStream(Stream.Null);
          this._WriteDelegate(this.FileName, (Stream) calculatorStream);
          this._Crc32 = calculatorStream.Crc;
        }
        else if (this._Source != ZipEntrySource.ZipFile)
        {
          if (this._Source == ZipEntrySource.Stream)
          {
            this.PrepSourceStream();
            input = this._sourceStream;
          }
          else if (this._Source == ZipEntrySource.JitStream)
          {
            if (this._sourceStream == null)
              this._sourceStream = this._OpenDelegate(this.FileName);
            this.PrepSourceStream();
            input = this._sourceStream;
          }
          else if (this._Source != ZipEntrySource.ZipOutputStream)
            input = (Stream) File.Open(this.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
          this._Crc32 = new CRC32().GetCrc32(input);
          if (this._sourceStream == null)
            input.Dispose();
        }
        this._crcCalculated = true;
      }
      return this._Crc32;
    }

    private void PrepSourceStream()
    {
      if (this._sourceStream == null)
        throw new ZipException(string.Format("The input stream is null for entry '{0}'.", (object) this.FileName));
      if (this._sourceStreamOriginalPosition.HasValue)
        this._sourceStream.Position = this._sourceStreamOriginalPosition.Value;
      else if (this._sourceStream.CanSeek)
        this._sourceStreamOriginalPosition = new long?(this._sourceStream.Position);
      else if (this.Encryption == EncryptionAlgorithm.PkzipWeak && this._Source != ZipEntrySource.ZipFile && ((int) this._BitField & 8) != 8)
        throw new ZipException("It is not possible to use PKZIP encryption on a non-seekable input stream");
    }

    internal void CopyMetaData(ZipEntry source)
    {
      this.__FileDataPosition = source.__FileDataPosition;
      this.CompressionMethod = source.CompressionMethod;
      this._CompressionMethod_FromZipFile = source._CompressionMethod_FromZipFile;
      this._CompressedFileDataSize = source._CompressedFileDataSize;
      this._UncompressedSize = source._UncompressedSize;
      this._BitField = source._BitField;
      this._Source = source._Source;
      this._LastModified = source._LastModified;
      this._Mtime = source._Mtime;
      this._Atime = source._Atime;
      this._Ctime = source._Ctime;
      this._ntfsTimesAreSet = source._ntfsTimesAreSet;
      this._emitUnixTimes = source._emitUnixTimes;
      this._emitNtfsTimes = source._emitNtfsTimes;
    }

    private void OnWriteBlock(long bytesXferred, long totalBytesToXfer)
    {
      if (this._container.ZipFile == null)
        return;
      this._ioOperationCanceled = this._container.ZipFile.OnSaveBlock(this, bytesXferred, totalBytesToXfer);
    }

    private void _WriteEntryData(Stream s)
    {
      Stream input = (Stream) null;
      long num1 = -1L;
      try
      {
        num1 = s.Position;
      }
      catch (Exception ex)
      {
      }
      try
      {
        long num2 = this.SetInputAndFigureFileLength(ref input);
        CountingStream entryCounter = new CountingStream(s);
        Stream stream1;
        Stream stream2;
        if (num2 != 0L)
        {
          stream1 = this.MaybeApplyEncryption((Stream) entryCounter);
          stream2 = this.MaybeApplyCompression(stream1, num2);
        }
        else
          stream1 = stream2 = (Stream) entryCounter;
        CrcCalculatorStream output = new CrcCalculatorStream(stream2, true);
        if (this._Source == ZipEntrySource.WriteDelegate)
        {
          this._WriteDelegate(this.FileName, (Stream) output);
        }
        else
        {
          byte[] buffer = new byte[this.BufferSize];
          int count;
          while ((count = SharedUtilities.ReadWithRetry(input, buffer, 0, buffer.Length, this.FileName)) != 0)
          {
            output.Write(buffer, 0, count);
            this.OnWriteBlock(output.TotalBytesSlurped, num2);
            if (this._ioOperationCanceled)
              break;
          }
        }
        this.FinishOutputStream(s, entryCounter, stream1, stream2, output);
      }
      finally
      {
        if (this._Source == ZipEntrySource.JitStream)
        {
          if (this._CloseDelegate != null)
            this._CloseDelegate(this.FileName, input);
        }
        else if (input is FileStream)
          input.Dispose();
      }
      if (this._ioOperationCanceled)
        return;
      this.__FileDataPosition = num1;
      this.PostProcessOutput(s);
    }

    private long SetInputAndFigureFileLength(ref Stream input)
    {
      long num = -1L;
      if (this._Source == ZipEntrySource.Stream)
      {
        this.PrepSourceStream();
        input = this._sourceStream;
        try
        {
          num = this._sourceStream.Length;
        }
        catch (NotSupportedException ex)
        {
        }
      }
      else if (this._Source == ZipEntrySource.ZipFile)
      {
        this._sourceStream = (Stream) this.InternalOpenReader(this._Encryption_FromZipFile == EncryptionAlgorithm.None ? (string) null : this._Password ?? this._container.Password);
        this.PrepSourceStream();
        input = this._sourceStream;
        num = this._sourceStream.Length;
      }
      else if (this._Source == ZipEntrySource.JitStream)
      {
        if (this._sourceStream == null)
          this._sourceStream = this._OpenDelegate(this.FileName);
        this.PrepSourceStream();
        input = this._sourceStream;
        try
        {
          num = this._sourceStream.Length;
        }
        catch (NotSupportedException ex)
        {
        }
      }
      else if (this._Source == ZipEntrySource.FileSystem)
      {
        FileShare share = (FileShare) (3 | 4);
        input = (Stream) File.Open(this.LocalFileName, FileMode.Open, FileAccess.Read, share);
        num = input.Length;
      }
      return num;
    }

    internal void FinishOutputStream(Stream s, CountingStream entryCounter, Stream encryptor, Stream compressor, CrcCalculatorStream output)
    {
      if (output == null)
        return;
      output.Close();
      if (compressor is DeflateStream)
        compressor.Close();
      else if (compressor is ParallelDeflateOutputStream)
        compressor.Close();
      encryptor.Flush();
      encryptor.Close();
      this._LengthOfTrailer = 0;
      this._UncompressedSize = output.TotalBytesSlurped;
      this._CompressedFileDataSize = entryCounter.BytesWritten;
      this._CompressedSize = this._CompressedFileDataSize;
      this._Crc32 = output.Crc;
      this.StoreRelativeOffset();
    }

    internal void PostProcessOutput(Stream s)
    {
      CountingStream countingStream = s as CountingStream;
      if (this._UncompressedSize == 0L && this._CompressedSize == 0L)
      {
        if (this._Source == ZipEntrySource.ZipOutputStream)
          return;
        if (this._Password != null)
        {
          int num1 = 0;
          if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
            num1 = 12;
          if (this._Source == ZipEntrySource.ZipOutputStream && !s.CanSeek)
            throw new ZipException("Zero bytes written, encryption in use, and non-seekable output.");
          if (this.Encryption != EncryptionAlgorithm.None)
          {
            s.Seek((long) (-1 * num1), SeekOrigin.Current);
            s.SetLength(s.Position);
            if (countingStream != null)
              countingStream.Adjust((long) num1);
            this._LengthOfHeader -= num1;
            this.__FileDataPosition -= (long) num1;
          }
          this._Password = (string) null;
          this._BitField &= (short) -2;
          int num2 = 6;
          byte[] numArray1 = this._EntryHeader;
          int index1 = num2;
          int num3 = 1;
          int num4 = index1 + num3;
          int num5 = (int) (byte) ((uint) this._BitField & (uint) byte.MaxValue);
          numArray1[index1] = (byte) num5;
          byte[] numArray2 = this._EntryHeader;
          int index2 = num4;
          int num6 = 1;
          int num7 = index2 + num6;
          int num8 = (int) (byte) (((int) this._BitField & 65280) >> 8);
          numArray2[index2] = (byte) num8;
        }
        this.CompressionMethod = CompressionMethod.None;
        this.Encryption = EncryptionAlgorithm.None;
      }
      else if (this._zipCrypto_forWrite != null && this.Encryption == EncryptionAlgorithm.PkzipWeak)
        this._CompressedSize += 12L;
      int num9 = 8;
      byte[] numArray3 = this._EntryHeader;
      int index3 = num9;
      int num10 = 1;
      int num11 = index3 + num10;
      int num12 = (int) (byte) ((uint) this._CompressionMethod & (uint) byte.MaxValue);
      numArray3[index3] = (byte) num12;
      byte[] numArray4 = this._EntryHeader;
      int index4 = num11;
      int num13 = 1;
      int num14 = index4 + num13;
      int num15 = (int) (byte) (((int) this._CompressionMethod & 65280) >> 8);
      numArray4[index4] = (byte) num15;
      int num16 = 14;
      byte[] numArray5 = this._EntryHeader;
      int index5 = num16;
      int num17 = 1;
      int num18 = index5 + num17;
      int num19 = (int) (byte) (this._Crc32 & (int) byte.MaxValue);
      numArray5[index5] = (byte) num19;
      byte[] numArray6 = this._EntryHeader;
      int index6 = num18;
      int num20 = 1;
      int num21 = index6 + num20;
      int num22 = (int) (byte) ((this._Crc32 & 65280) >> 8);
      numArray6[index6] = (byte) num22;
      byte[] numArray7 = this._EntryHeader;
      int index7 = num21;
      int num23 = 1;
      int num24 = index7 + num23;
      int num25 = (int) (byte) ((this._Crc32 & 16711680) >> 16);
      numArray7[index7] = (byte) num25;
      byte[] numArray8 = this._EntryHeader;
      int index8 = num24;
      int num26 = 1;
      int num27 = index8 + num26;
      int num28 = (int) (byte) (((long) this._Crc32 & 4278190080L) >> 24);
      numArray8[index8] = (byte) num28;
      this.SetZip64Flags();
      short num29 = (short) ((int) this._EntryHeader[26] + (int) this._EntryHeader[27] * 256);
      short num30 = (short) ((int) this._EntryHeader[28] + (int) this._EntryHeader[29] * 256);
      if (this._OutputUsesZip64.Value)
      {
        this._EntryHeader[4] = (byte) 45;
        this._EntryHeader[5] = (byte) 0;
        for (int index1 = 0; index1 < 8; ++index1)
          this._EntryHeader[num27++] = byte.MaxValue;
        int num1 = 30 + (int) num29;
        byte[] numArray1 = this._EntryHeader;
        int index2 = num1;
        int num2 = 1;
        int num3 = index2 + num2;
        int num4 = 1;
        numArray1[index2] = (byte) num4;
        byte[] numArray2 = this._EntryHeader;
        int index9 = num3;
        int num5 = 1;
        int num6 = index9 + num5;
        int num7 = 0;
        numArray2[index9] = (byte) num7;
        int destinationIndex = num6 + 2;
        Array.Copy((Array) BitConverter.GetBytes(this._UncompressedSize), 0, (Array) this._EntryHeader, destinationIndex, 8);
        Array.Copy((Array) BitConverter.GetBytes(this._CompressedSize), 0, (Array) this._EntryHeader, destinationIndex + 8, 8);
      }
      else
      {
        this._EntryHeader[4] = (byte) 20;
        this._EntryHeader[5] = (byte) 0;
        int num1 = 18;
        byte[] numArray1 = this._EntryHeader;
        int index1 = num1;
        int num2 = 1;
        int num3 = index1 + num2;
        int num4 = (int) (byte) ((ulong) this._CompressedSize & (ulong) byte.MaxValue);
        numArray1[index1] = (byte) num4;
        byte[] numArray2 = this._EntryHeader;
        int index2 = num3;
        int num5 = 1;
        int num6 = index2 + num5;
        int num7 = (int) (byte) ((this._CompressedSize & 65280L) >> 8);
        numArray2[index2] = (byte) num7;
        byte[] numArray9 = this._EntryHeader;
        int index9 = num6;
        int num8 = 1;
        int num31 = index9 + num8;
        int num32 = (int) (byte) ((this._CompressedSize & 16711680L) >> 16);
        numArray9[index9] = (byte) num32;
        byte[] numArray10 = this._EntryHeader;
        int index10 = num31;
        int num33 = 1;
        int num34 = index10 + num33;
        int num35 = (int) (byte) ((this._CompressedSize & 4278190080L) >> 24);
        numArray10[index10] = (byte) num35;
        byte[] numArray11 = this._EntryHeader;
        int index11 = num34;
        int num36 = 1;
        int num37 = index11 + num36;
        int num38 = (int) (byte) ((ulong) this._UncompressedSize & (ulong) byte.MaxValue);
        numArray11[index11] = (byte) num38;
        byte[] numArray12 = this._EntryHeader;
        int index12 = num37;
        int num39 = 1;
        int num40 = index12 + num39;
        int num41 = (int) (byte) ((this._UncompressedSize & 65280L) >> 8);
        numArray12[index12] = (byte) num41;
        byte[] numArray13 = this._EntryHeader;
        int index13 = num40;
        int num42 = 1;
        int num43 = index13 + num42;
        int num44 = (int) (byte) ((this._UncompressedSize & 16711680L) >> 16);
        numArray13[index13] = (byte) num44;
        byte[] numArray14 = this._EntryHeader;
        int index14 = num43;
        int num45 = 1;
        num14 = index14 + num45;
        int num46 = (int) (byte) ((this._UncompressedSize & 4278190080L) >> 24);
        numArray14[index14] = (byte) num46;
        if ((int) num30 != 0)
        {
          int num47 = 30 + (int) num29;
          if ((int) (short) ((int) this._EntryHeader[num47 + 2] + (int) this._EntryHeader[num47 + 3] * 256) == 16)
          {
            byte[] numArray15 = this._EntryHeader;
            int index15 = num47;
            int num48 = 1;
            int num49 = index15 + num48;
            int num50 = 153;
            numArray15[index15] = (byte) num50;
            byte[] numArray16 = this._EntryHeader;
            int index16 = num49;
            int num51 = 1;
            num14 = index16 + num51;
            int num52 = 153;
            numArray16[index16] = (byte) num52;
          }
        }
      }
      if (((int) this._BitField & 8) != 8 || this._Source == ZipEntrySource.ZipOutputStream && s.CanSeek)
      {
        ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
        if (zipSegmentedStream != null && (int) this._diskNumber != (int) zipSegmentedStream.CurrentSegment)
        {
          using (Stream stream = ZipSegmentedStream.ForUpdate(this._container.ZipFile.Name, this._diskNumber))
          {
            stream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
            stream.Write(this._EntryHeader, 0, this._EntryHeader.Length);
          }
        }
        else
        {
          s.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
          s.Write(this._EntryHeader, 0, this._EntryHeader.Length);
          if (countingStream != null)
            countingStream.Adjust((long) this._EntryHeader.Length);
          s.Seek(this._CompressedSize, SeekOrigin.Current);
        }
      }
      if (((int) this._BitField & 8) != 8 || this.IsDirectory)
        return;
      byte[] buffer = new byte[16 + (this._OutputUsesZip64.Value ? 8 : 0)];
      int destinationIndex1 = 0;
      Array.Copy((Array) BitConverter.GetBytes(134695760), 0, (Array) buffer, destinationIndex1, 4);
      int destinationIndex2 = destinationIndex1 + 4;
      Array.Copy((Array) BitConverter.GetBytes(this._Crc32), 0, (Array) buffer, destinationIndex2, 4);
      int destinationIndex3 = destinationIndex2 + 4;
      if (this._OutputUsesZip64.Value)
      {
        Array.Copy((Array) BitConverter.GetBytes(this._CompressedSize), 0, (Array) buffer, destinationIndex3, 8);
        int destinationIndex4 = destinationIndex3 + 8;
        Array.Copy((Array) BitConverter.GetBytes(this._UncompressedSize), 0, (Array) buffer, destinationIndex4, 8);
        num14 = destinationIndex4 + 8;
      }
      else
      {
        byte[] numArray1 = buffer;
        int index1 = destinationIndex3;
        int num1 = 1;
        int num2 = index1 + num1;
        int num3 = (int) (byte) ((ulong) this._CompressedSize & (ulong) byte.MaxValue);
        numArray1[index1] = (byte) num3;
        byte[] numArray2 = buffer;
        int index2 = num2;
        int num4 = 1;
        int num5 = index2 + num4;
        int num6 = (int) (byte) ((this._CompressedSize & 65280L) >> 8);
        numArray2[index2] = (byte) num6;
        byte[] numArray9 = buffer;
        int index9 = num5;
        int num7 = 1;
        int num8 = index9 + num7;
        int num31 = (int) (byte) ((this._CompressedSize & 16711680L) >> 16);
        numArray9[index9] = (byte) num31;
        byte[] numArray10 = buffer;
        int index10 = num8;
        int num32 = 1;
        int num33 = index10 + num32;
        int num34 = (int) (byte) ((this._CompressedSize & 4278190080L) >> 24);
        numArray10[index10] = (byte) num34;
        byte[] numArray11 = buffer;
        int index11 = num33;
        int num35 = 1;
        int num36 = index11 + num35;
        int num37 = (int) (byte) ((ulong) this._UncompressedSize & (ulong) byte.MaxValue);
        numArray11[index11] = (byte) num37;
        byte[] numArray12 = buffer;
        int index12 = num36;
        int num38 = 1;
        int num39 = index12 + num38;
        int num40 = (int) (byte) ((this._UncompressedSize & 65280L) >> 8);
        numArray12[index12] = (byte) num40;
        byte[] numArray13 = buffer;
        int index13 = num39;
        int num41 = 1;
        int num42 = index13 + num41;
        int num43 = (int) (byte) ((this._UncompressedSize & 16711680L) >> 16);
        numArray13[index13] = (byte) num43;
        byte[] numArray14 = buffer;
        int index14 = num42;
        int num44 = 1;
        num14 = index14 + num44;
        int num45 = (int) (byte) ((this._UncompressedSize & 4278190080L) >> 24);
        numArray14[index14] = (byte) num45;
      }
      s.Write(buffer, 0, buffer.Length);
      this._LengthOfTrailer += buffer.Length;
    }

    private void SetZip64Flags()
    {
      this._entryRequiresZip64 = new bool?(this._CompressedSize >= (long) uint.MaxValue || this._UncompressedSize >= (long) uint.MaxValue || this._RelativeOffsetOfLocalHeader >= (long) uint.MaxValue);
      if (this._container.Zip64 == Zip64Option.Default && this._entryRequiresZip64.Value)
        throw new ZipException("Compressed or Uncompressed size, or offset exceeds the maximum value. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
      this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
    }

    internal void PrepOutputStream(Stream s, long streamLength, out CountingStream outputCounter, out Stream encryptor, out Stream compressor, out CrcCalculatorStream output)
    {
      outputCounter = new CountingStream(s);
      if (streamLength != 0L)
      {
        encryptor = this.MaybeApplyEncryption((Stream) outputCounter);
        compressor = this.MaybeApplyCompression(encryptor, streamLength);
      }
      else
        encryptor = compressor = (Stream) outputCounter;
      output = new CrcCalculatorStream(compressor, true);
    }

    private Stream MaybeApplyCompression(Stream s, long streamLength)
    {
      if ((int) this._CompressionMethod != 8 || this.CompressionLevel == CompressionLevel.None)
        return s;
      if (this._container.ParallelDeflateThreshold == 0L || streamLength > this._container.ParallelDeflateThreshold && this._container.ParallelDeflateThreshold > 0L)
      {
        if (this._container.ParallelDeflater == null)
        {
          this._container.ParallelDeflater = new ParallelDeflateOutputStream(s, this.CompressionLevel, this._container.Strategy, true);
          if (this._container.CodecBufferSize > 0)
            this._container.ParallelDeflater.BufferSize = this._container.CodecBufferSize;
          if (this._container.ParallelDeflateMaxBufferPairs > 0)
            this._container.ParallelDeflater.MaxBufferPairs = this._container.ParallelDeflateMaxBufferPairs;
        }
        ParallelDeflateOutputStream parallelDeflater = this._container.ParallelDeflater;
        parallelDeflater.Reset(s);
        return (Stream) parallelDeflater;
      }
      DeflateStream deflateStream = new DeflateStream(s, CompressionMode.Compress, this.CompressionLevel, true);
      if (this._container.CodecBufferSize > 0)
        deflateStream.BufferSize = this._container.CodecBufferSize;
      deflateStream.Strategy = this._container.Strategy;
      return (Stream) deflateStream;
    }

    private Stream MaybeApplyEncryption(Stream s)
    {
      if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
        return (Stream) new ZipCipherStream(s, this._zipCrypto_forWrite, CryptoMode.Encrypt);
      return s;
    }

    private void OnZipErrorWhileSaving(Exception e)
    {
      if (this._container.ZipFile == null)
        return;
      this._ioOperationCanceled = this._container.ZipFile.OnZipErrorSaving(this, e);
    }

    internal void Write(Stream s)
    {
      CountingStream countingStream = s as CountingStream;
      ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
      bool flag1 = false;
      do
      {
        try
        {
          if (this._Source == ZipEntrySource.ZipFile && !this._restreamRequiredOnSave)
          {
            this.CopyThroughOneEntry(s);
            break;
          }
          if (this.IsDirectory)
          {
            this.WriteHeader(s, 1);
            this.StoreRelativeOffset();
            this._entryRequiresZip64 = new bool?(this._RelativeOffsetOfLocalHeader >= (long) uint.MaxValue);
            this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
            if (zipSegmentedStream == null)
              break;
            this._diskNumber = zipSegmentedStream.CurrentSegment;
            break;
          }
          int cycle = 0;
          bool flag2;
          do
          {
            ++cycle;
            this.WriteHeader(s, cycle);
            this.WriteSecurityMetadata(s);
            this._WriteEntryData(s);
            this._TotalEntrySize = (long) this._LengthOfHeader + this._CompressedFileDataSize + (long) this._LengthOfTrailer;
            flag2 = cycle <= 1 && (s.CanSeek && this.WantReadAgain());
            if (flag2)
            {
              if (zipSegmentedStream != null)
                zipSegmentedStream.TruncateBackward(this._diskNumber, this._RelativeOffsetOfLocalHeader);
              else
                s.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
              s.SetLength(s.Position);
              if (countingStream != null)
                countingStream.Adjust(this._TotalEntrySize);
            }
          }
          while (flag2);
          this._skippedDuringSave = false;
          flag1 = true;
        }
        catch (Exception ex)
        {
          ZipErrorAction zipErrorAction = this.ZipErrorAction;
          int num1 = 0;
          while (this.ZipErrorAction != ZipErrorAction.Throw)
          {
            if (this.ZipErrorAction == ZipErrorAction.Skip || this.ZipErrorAction == ZipErrorAction.Retry)
            {
              long num2 = countingStream != null ? countingStream.ComputedPosition : s.Position;
              long offset = num2 - this._future_ROLH;
              if (offset > 0L)
              {
                s.Seek(offset, SeekOrigin.Current);
                long position = s.Position;
                s.SetLength(s.Position);
                if (countingStream != null)
                  countingStream.Adjust(num2 - position);
              }
              if (this.ZipErrorAction == ZipErrorAction.Skip)
              {
                this.WriteStatus("Skipping file {0} (exception: {1})", (object) this.LocalFileName, (object) ex.ToString());
                this._skippedDuringSave = true;
                flag1 = true;
                goto label_32;
              }
              else
              {
                this.ZipErrorAction = zipErrorAction;
                goto label_32;
              }
            }
            else if (num1 > 0)
            {
              throw;
            }
            else
            {
              if (this.ZipErrorAction == ZipErrorAction.InvokeErrorEvent)
              {
                this.OnZipErrorWhileSaving(ex);
                if (this._ioOperationCanceled)
                {
                  flag1 = true;
                  goto label_32;
                }
              }
              ++num1;
            }
          }
          throw;
        }
label_32:;
      }
      while (!flag1);
    }

    internal void StoreRelativeOffset()
    {
      this._RelativeOffsetOfLocalHeader = this._future_ROLH;
    }

    internal void NotifySaveComplete()
    {
      this._Encryption_FromZipFile = this._Encryption;
      this._CompressionMethod_FromZipFile = this._CompressionMethod;
      this._restreamRequiredOnSave = false;
      this._metadataChanged = false;
      this._Source = ZipEntrySource.ZipFile;
    }

    internal void WriteSecurityMetadata(Stream outstream)
    {
      if (this.Encryption == EncryptionAlgorithm.None)
        return;
      string password = this._Password;
      if (this._Source == ZipEntrySource.ZipFile && password == null)
        password = this._container.Password;
      if (password == null)
      {
        this._zipCrypto_forWrite = (ZipCrypto) null;
      }
      else
      {
        if (this.Encryption != EncryptionAlgorithm.PkzipWeak)
          return;
        this._zipCrypto_forWrite = ZipCrypto.ForWrite(password);
        Random random = new Random();
        byte[] numArray = new byte[12];
        random.NextBytes(numArray);
        if (((int) this._BitField & 8) == 8)
        {
          this._TimeBlob = SharedUtilities.DateTimeToPacked(this.LastModified);
          numArray[11] = (byte) (this._TimeBlob >> 8 & (int) byte.MaxValue);
        }
        else
        {
          this.FigureCrc32();
          numArray[11] = (byte) (this._Crc32 >> 24 & (int) byte.MaxValue);
        }
        byte[] buffer = this._zipCrypto_forWrite.EncryptMessage(numArray, numArray.Length);
        outstream.Write(buffer, 0, buffer.Length);
        this._LengthOfHeader += buffer.Length;
      }
    }

    private void CopyThroughOneEntry(Stream outStream)
    {
      if (this.LengthOfHeader == 0)
        throw new BadStateException("Bad header length.");
      if (this._metadataChanged || this.ArchiveStream is ZipSegmentedStream || outStream is ZipSegmentedStream || this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Default || !this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Always)
        this.CopyThroughWithRecompute(outStream);
      else
        this.CopyThroughWithNoChange(outStream);
      this._entryRequiresZip64 = new bool?(this._CompressedSize >= (long) uint.MaxValue || this._UncompressedSize >= (long) uint.MaxValue || this._RelativeOffsetOfLocalHeader >= (long) uint.MaxValue);
      this._OutputUsesZip64 = new bool?(this._container.Zip64 == Zip64Option.Always || this._entryRequiresZip64.Value);
    }

    private void CopyThroughWithRecompute(Stream outstream)
    {
      byte[] buffer1 = new byte[this.BufferSize];
      CountingStream countingStream = new CountingStream(this.ArchiveStream);
      long num1 = this._RelativeOffsetOfLocalHeader;
      int lengthOfHeader = this.LengthOfHeader;
      this.WriteHeader(outstream, 0);
      this.StoreRelativeOffset();
      if (!this.FileName.EndsWith("/"))
      {
        long num2 = num1 + (long) lengthOfHeader;
        int cryptoHeaderBytes = ZipEntry.GetLengthOfCryptoHeaderBytes(this._Encryption_FromZipFile);
        long offset = num2 - (long) cryptoHeaderBytes;
        this._LengthOfHeader += cryptoHeaderBytes;
        countingStream.Seek(offset, SeekOrigin.Begin);
        long num3 = this._CompressedSize;
        while (num3 > 0L)
        {
          int count1 = num3 > (long) buffer1.Length ? buffer1.Length : (int) num3;
          int count2 = countingStream.Read(buffer1, 0, count1);
          outstream.Write(buffer1, 0, count2);
          num3 -= (long) count2;
          this.OnWriteBlock(countingStream.BytesRead, this._CompressedSize);
          if (this._ioOperationCanceled)
            break;
        }
        if (((int) this._BitField & 8) == 8)
        {
          int count = 16;
          if (this._InputUsesZip64)
            count += 8;
          byte[] buffer2 = new byte[count];
          countingStream.Read(buffer2, 0, count);
          if (this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Default)
          {
            outstream.Write(buffer2, 0, 8);
            if (this._CompressedSize > (long) uint.MaxValue)
              throw new InvalidOperationException("ZIP64 is required");
            outstream.Write(buffer2, 8, 4);
            if (this._UncompressedSize > (long) uint.MaxValue)
              throw new InvalidOperationException("ZIP64 is required");
            outstream.Write(buffer2, 16, 4);
            this._LengthOfTrailer -= 8;
          }
          else if (!this._InputUsesZip64 && this._container.UseZip64WhenSaving == Zip64Option.Always)
          {
            byte[] buffer3 = new byte[4];
            outstream.Write(buffer2, 0, 8);
            outstream.Write(buffer2, 8, 4);
            outstream.Write(buffer3, 0, 4);
            outstream.Write(buffer2, 12, 4);
            outstream.Write(buffer3, 0, 4);
            this._LengthOfTrailer += 8;
          }
          else
            outstream.Write(buffer2, 0, count);
        }
      }
      this._TotalEntrySize = (long) this._LengthOfHeader + this._CompressedFileDataSize + (long) this._LengthOfTrailer;
    }

    private void CopyThroughWithNoChange(Stream outstream)
    {
      byte[] buffer = new byte[this.BufferSize];
      CountingStream countingStream1 = new CountingStream(this.ArchiveStream);
      countingStream1.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
      if (this._TotalEntrySize == 0L)
        this._TotalEntrySize = (long) this._LengthOfHeader + this._CompressedFileDataSize + (long) this._LengthOfTrailer;
      CountingStream countingStream2 = outstream as CountingStream;
      this._RelativeOffsetOfLocalHeader = countingStream2 != null ? countingStream2.ComputedPosition : outstream.Position;
      long num = this._TotalEntrySize;
      while (num > 0L)
      {
        int count1 = num > (long) buffer.Length ? buffer.Length : (int) num;
        int count2 = countingStream1.Read(buffer, 0, count1);
        outstream.Write(buffer, 0, count2);
        num -= (long) count2;
        this.OnWriteBlock(countingStream1.BytesRead, this._TotalEntrySize);
        if (this._ioOperationCanceled)
          break;
      }
    }

    [Conditional("Trace")]
    private void TraceWriteLine(string format, params object[] varParams)
    {
      lock (this._outputLock)
      {
        int local_0 = Thread.CurrentThread.GetHashCode();
        Console.ForegroundColor = (ConsoleColor) (local_0 % 8 + 8);
        Console.Write("{0:000} ZipEntry.Write ", (object) local_0);
        Console.WriteLine(format, varParams);
        Console.ResetColor();
      }
    }

    private class CopyHelper
    {
      private static Regex re = new Regex(" \\(copy (\\d+)\\)$");
      private static int callCount = 0;

      internal static string AppendCopyToFileName(string f)
      {
        ++ZipEntry.CopyHelper.callCount;
        if (ZipEntry.CopyHelper.callCount > 25)
          throw new OverflowException("overflow while creating filename");
        int num1 = 1;
        int num2 = f.LastIndexOf(".");
        if (num2 == -1)
        {
          Match match = ZipEntry.CopyHelper.re.Match(f);
          if (match.Success)
          {
            string str = string.Format(" (copy {0})", (object) (int.Parse(match.Groups[1].Value) + 1));
            f = f.Substring(0, match.Index) + str;
          }
          else
          {
            string str = string.Format(" (copy {0})", (object) num1);
            f += str;
          }
        }
        else
        {
          Match match = ZipEntry.CopyHelper.re.Match(f.Substring(0, num2));
          if (match.Success)
          {
            string str = string.Format(" (copy {0})", (object) (int.Parse(match.Groups[1].Value) + 1));
            f = f.Substring(0, match.Index) + str + f.Substring(num2);
          }
          else
          {
            string str = string.Format(" (copy {0})", (object) num1);
            f = f.Substring(0, num2) + str + f.Substring(num2);
          }
        }
        return f;
      }
    }

    private delegate T Func<T>();
  }
}
