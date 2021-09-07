
// Type: Ionic.Zip.AddProgressEventArgs


// Hacked by SystemAce

namespace Ionic.Zip
{
  public class AddProgressEventArgs : ZipProgressEventArgs
  {
    internal AddProgressEventArgs()
    {
    }

    private AddProgressEventArgs(string archiveName, ZipProgressEventType flavor)
      : base(archiveName, flavor)
    {
    }

    internal static AddProgressEventArgs AfterEntry(string archiveName, ZipEntry entry, int entriesTotal)
    {
      AddProgressEventArgs progressEventArgs = new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_AfterAddEntry);
      progressEventArgs.EntriesTotal = entriesTotal;
      progressEventArgs.CurrentEntry = entry;
      return progressEventArgs;
    }

    internal static AddProgressEventArgs Started(string archiveName)
    {
      return new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Started);
    }

    internal static AddProgressEventArgs Completed(string archiveName)
    {
      return new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Completed);
    }
  }
}
