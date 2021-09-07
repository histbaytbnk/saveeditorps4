﻿
// Type: ICSharpCode.SharpZipLib.Zip.DiskArchiveStorage


// Hacked by SystemAce

using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class DiskArchiveStorage : BaseArchiveStorage
  {
    private Stream temporaryStream_;
    private string fileName_;
    private string temporaryName_;

    public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode)
      : base(updateMode)
    {
      if (file.Name == null)
        throw new ZipException("Cant handle non file archives");
      this.fileName_ = file.Name;
    }

    public DiskArchiveStorage(ZipFile file)
      : this(file, FileUpdateMode.Safe)
    {
    }

    public override Stream GetTemporaryOutput()
    {
      if (this.temporaryName_ != null)
      {
        this.temporaryName_ = DiskArchiveStorage.GetTempFileName(this.temporaryName_, true);
        this.temporaryStream_ = (Stream) File.Open(this.temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
      }
      else
      {
        this.temporaryName_ = Path.GetTempFileName();
        this.temporaryStream_ = (Stream) File.Open(this.temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
      }
      return this.temporaryStream_;
    }

    public override Stream ConvertTemporaryToFinal()
    {
      if (this.temporaryStream_ == null)
        throw new ZipException("No temporary stream has been created");
      Stream stream = (Stream) null;
      string tempFileName = DiskArchiveStorage.GetTempFileName(this.fileName_, false);
      bool flag = false;
      try
      {
        this.temporaryStream_.Close();
        File.Move(this.fileName_, tempFileName);
        File.Move(this.temporaryName_, this.fileName_);
        flag = true;
        File.Delete(tempFileName);
        return (Stream) File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
      }
      catch (Exception ex)
      {
        stream = (Stream) null;
        if (!flag)
        {
          File.Move(tempFileName, this.fileName_);
          File.Delete(this.temporaryName_);
        }
        throw;
      }
    }

    public override Stream MakeTemporaryCopy(Stream stream)
    {
      stream.Close();
      this.temporaryName_ = DiskArchiveStorage.GetTempFileName(this.fileName_, true);
      File.Copy(this.fileName_, this.temporaryName_, true);
      this.temporaryStream_ = (Stream) new FileStream(this.temporaryName_, FileMode.Open, FileAccess.ReadWrite);
      return this.temporaryStream_;
    }

    public override Stream OpenForDirectUpdate(Stream stream)
    {
      Stream stream1;
      if (stream == null || !stream.CanWrite)
      {
        if (stream != null)
          stream.Close();
        stream1 = (Stream) new FileStream(this.fileName_, FileMode.Open, FileAccess.ReadWrite);
      }
      else
        stream1 = stream;
      return stream1;
    }

    public override void Dispose()
    {
      if (this.temporaryStream_ == null)
        return;
      this.temporaryStream_.Close();
    }

    private static string GetTempFileName(string original, bool makeTempFile)
    {
      string str = (string) null;
      if (original == null)
      {
        str = Path.GetTempFileName();
      }
      else
      {
        int num = 0;
        int second = DateTime.Now.Second;
        while (str == null)
        {
          ++num;
          string path = string.Format("{0}.{1}{2}.tmp", (object) original, (object) second, (object) num);
          if (!File.Exists(path))
          {
            if (makeTempFile)
            {
              try
              {
                using (File.Create(path))
                  ;
                str = path;
              }
              catch
              {
                second = DateTime.Now.Second;
              }
            }
            else
              str = path;
          }
        }
      }
      return str;
    }
  }
}
