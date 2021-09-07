
// Type: ICSharpCode.SharpZipLib.Zip.FastZipEvents


// Hacked by SystemAce

using ICSharpCode.SharpZipLib.Core;
using System;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class FastZipEvents
  {
    private TimeSpan progressInterval_ = TimeSpan.FromSeconds(3.0);
    public ProcessDirectoryHandler ProcessDirectory;
    public ProcessFileHandler ProcessFile;
    public ProgressHandler Progress;
    public CompletedFileHandler CompletedFile;
    public DirectoryFailureHandler DirectoryFailure;
    public FileFailureHandler FileFailure;

    public TimeSpan ProgressInterval
    {
      get
      {
        return this.progressInterval_;
      }
      set
      {
        this.progressInterval_ = value;
      }
    }

    public bool OnDirectoryFailure(string directory, Exception e)
    {
      bool flag = false;
      DirectoryFailureHandler directoryFailureHandler = this.DirectoryFailure;
      if (directoryFailureHandler != null)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(directory, e);
        directoryFailureHandler((object) this, e1);
        flag = e1.ContinueRunning;
      }
      return flag;
    }

    public bool OnFileFailure(string file, Exception e)
    {
      FileFailureHandler fileFailureHandler = this.FileFailure;
      bool flag = fileFailureHandler != null;
      if (flag)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(file, e);
        fileFailureHandler((object) this, e1);
        flag = e1.ContinueRunning;
      }
      return flag;
    }

    public bool OnProcessFile(string file)
    {
      bool flag = true;
      ProcessFileHandler processFileHandler = this.ProcessFile;
      if (processFileHandler != null)
      {
        ScanEventArgs e = new ScanEventArgs(file);
        processFileHandler((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }

    public bool OnCompletedFile(string file)
    {
      bool flag = true;
      CompletedFileHandler completedFileHandler = this.CompletedFile;
      if (completedFileHandler != null)
      {
        ScanEventArgs e = new ScanEventArgs(file);
        completedFileHandler((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }

    public bool OnProcessDirectory(string directory, bool hasMatchingFiles)
    {
      bool flag = true;
      ProcessDirectoryHandler directoryHandler = this.ProcessDirectory;
      if (directoryHandler != null)
      {
        DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
        directoryHandler((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }
  }
}
