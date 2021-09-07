
// Type: Ionic.Zip.ZipErrorEventArgs


// Hacked by SystemAce

using System;

namespace Ionic.Zip
{
  public class ZipErrorEventArgs : ZipProgressEventArgs
  {
    private Exception _exc;

    public Exception Exception
    {
      get
      {
        return this._exc;
      }
    }

    public string FileName
    {
      get
      {
        return this.CurrentEntry.LocalFileName;
      }
    }

    private ZipErrorEventArgs()
    {
    }

    internal static ZipErrorEventArgs Saving(string archiveName, ZipEntry entry, Exception exception)
    {
      ZipErrorEventArgs zipErrorEventArgs = new ZipErrorEventArgs();
      zipErrorEventArgs.EventType = ZipProgressEventType.Error_Saving;
      zipErrorEventArgs.ArchiveName = archiveName;
      zipErrorEventArgs.CurrentEntry = entry;
      zipErrorEventArgs._exc = exception;
      return zipErrorEventArgs;
    }
  }
}
