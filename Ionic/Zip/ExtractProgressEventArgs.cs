
// Type: Ionic.Zip.ExtractProgressEventArgs


// Hacked by SystemAce

namespace Ionic.Zip
{
  public class ExtractProgressEventArgs : ZipProgressEventArgs
  {
    private int _entriesExtracted;
    private string _target;

    public int EntriesExtracted
    {
      get
      {
        return this._entriesExtracted;
      }
    }

    public string ExtractLocation
    {
      get
      {
        return this._target;
      }
    }

    internal ExtractProgressEventArgs(string archiveName, bool before, int entriesTotal, int entriesExtracted, ZipEntry entry, string extractLocation)
      : base(archiveName, before ? ZipProgressEventType.Extracting_BeforeExtractEntry : ZipProgressEventType.Extracting_AfterExtractEntry)
    {
      this.EntriesTotal = entriesTotal;
      this.CurrentEntry = entry;
      this._entriesExtracted = entriesExtracted;
      this._target = extractLocation;
    }

    internal ExtractProgressEventArgs(string archiveName, ZipProgressEventType flavor)
      : base(archiveName, flavor)
    {
    }

    internal ExtractProgressEventArgs()
    {
    }

    internal static ExtractProgressEventArgs BeforeExtractEntry(string archiveName, ZipEntry entry, string extractLocation)
    {
      ExtractProgressEventArgs progressEventArgs = new ExtractProgressEventArgs();
      progressEventArgs.ArchiveName = archiveName;
      progressEventArgs.EventType = ZipProgressEventType.Extracting_BeforeExtractEntry;
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs._target = extractLocation;
      return progressEventArgs;
    }

    internal static ExtractProgressEventArgs ExtractExisting(string archiveName, ZipEntry entry, string extractLocation)
    {
      ExtractProgressEventArgs progressEventArgs = new ExtractProgressEventArgs();
      progressEventArgs.ArchiveName = archiveName;
      progressEventArgs.EventType = ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite;
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs._target = extractLocation;
      return progressEventArgs;
    }

    internal static ExtractProgressEventArgs AfterExtractEntry(string archiveName, ZipEntry entry, string extractLocation)
    {
      ExtractProgressEventArgs progressEventArgs = new ExtractProgressEventArgs();
      progressEventArgs.ArchiveName = archiveName;
      progressEventArgs.EventType = ZipProgressEventType.Extracting_AfterExtractEntry;
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs._target = extractLocation;
      return progressEventArgs;
    }

    internal static ExtractProgressEventArgs ExtractAllStarted(string archiveName, string extractLocation)
    {
      return new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_BeforeExtractAll)
      {
        _target = extractLocation
      };
    }

    internal static ExtractProgressEventArgs ExtractAllCompleted(string archiveName, string extractLocation)
    {
      return new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_AfterExtractAll)
      {
        _target = extractLocation
      };
    }

    internal static ExtractProgressEventArgs ByteUpdate(string archiveName, ZipEntry entry, long bytesWritten, long totalBytes)
    {
      ExtractProgressEventArgs progressEventArgs = new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_EntryBytesWritten);
      progressEventArgs.ArchiveName = archiveName;
      progressEventArgs.CurrentEntry = entry;
      progressEventArgs.BytesTransferred = bytesWritten;
      progressEventArgs.TotalBytesToTransfer = totalBytes;
      return progressEventArgs;
    }
  }
}
