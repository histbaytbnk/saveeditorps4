
// Type: ICSharpCode.SharpZipLib.Core.FileSystemScanner


// Hacked by SystemAce

using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
  public class FileSystemScanner
  {
    public ProcessDirectoryHandler ProcessDirectory;
    public ProcessFileHandler ProcessFile;
    public CompletedFileHandler CompletedFile;
    public DirectoryFailureHandler DirectoryFailure;
    public FileFailureHandler FileFailure;
    private IScanFilter fileFilter_;
    private IScanFilter directoryFilter_;
    private bool alive_;

    public FileSystemScanner(string filter)
    {
      this.fileFilter_ = (IScanFilter) new PathFilter(filter);
    }

    public FileSystemScanner(string fileFilter, string directoryFilter)
    {
      this.fileFilter_ = (IScanFilter) new PathFilter(fileFilter);
      this.directoryFilter_ = (IScanFilter) new PathFilter(directoryFilter);
    }

    public FileSystemScanner(IScanFilter fileFilter)
    {
      this.fileFilter_ = fileFilter;
    }

    public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
    {
      this.fileFilter_ = fileFilter;
      this.directoryFilter_ = directoryFilter;
    }

    private bool OnDirectoryFailure(string directory, Exception e)
    {
      DirectoryFailureHandler directoryFailureHandler = this.DirectoryFailure;
      bool flag = directoryFailureHandler != null;
      if (flag)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(directory, e);
        directoryFailureHandler((object) this, e1);
        this.alive_ = e1.ContinueRunning;
      }
      return flag;
    }

    private bool OnFileFailure(string file, Exception e)
    {
      bool flag = this.FileFailure != null;
      if (flag)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(file, e);
        this.FileFailure((object) this, e1);
        this.alive_ = e1.ContinueRunning;
      }
      return flag;
    }

    private void OnProcessFile(string file)
    {
      ProcessFileHandler processFileHandler = this.ProcessFile;
      if (processFileHandler == null)
        return;
      ScanEventArgs e = new ScanEventArgs(file);
      processFileHandler((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    private void OnCompleteFile(string file)
    {
      CompletedFileHandler completedFileHandler = this.CompletedFile;
      if (completedFileHandler == null)
        return;
      ScanEventArgs e = new ScanEventArgs(file);
      completedFileHandler((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    private void OnProcessDirectory(string directory, bool hasMatchingFiles)
    {
      ProcessDirectoryHandler directoryHandler = this.ProcessDirectory;
      if (directoryHandler == null)
        return;
      DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
      directoryHandler((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    public void Scan(string directory, bool recurse)
    {
      this.alive_ = true;
      this.ScanDir(directory, recurse);
    }

    private void ScanDir(string directory, bool recurse)
    {
      try
      {
        string[] files = Directory.GetFiles(directory);
        bool hasMatchingFiles = false;
        for (int index = 0; index < files.Length; ++index)
        {
          if (!this.fileFilter_.IsMatch(files[index]))
            files[index] = (string) null;
          else
            hasMatchingFiles = true;
        }
        this.OnProcessDirectory(directory, hasMatchingFiles);
        if (this.alive_)
        {
          if (hasMatchingFiles)
          {
            foreach (string file in files)
            {
              try
              {
                if (file != null)
                {
                  this.OnProcessFile(file);
                  if (!this.alive_)
                    break;
                }
              }
              catch (Exception ex)
              {
                if (!this.OnFileFailure(file, ex))
                  throw;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (!this.OnDirectoryFailure(directory, ex))
          throw;
      }
      if (!this.alive_)
        return;
      if (!recurse)
        return;
      try
      {
        foreach (string str in Directory.GetDirectories(directory))
        {
          if (this.directoryFilter_ == null || this.directoryFilter_.IsMatch(str))
          {
            this.ScanDir(str, true);
            if (!this.alive_)
              break;
          }
        }
      }
      catch (Exception ex)
      {
        if (this.OnDirectoryFailure(directory, ex))
          return;
        throw;
      }
    }
  }
}
