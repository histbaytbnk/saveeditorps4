
// Type: ICSharpCode.SharpZipLib.Core.ProgressEventArgs


// Hacked by SystemAce

using System;

namespace ICSharpCode.SharpZipLib.Core
{
  public class ProgressEventArgs : EventArgs
  {
    private bool continueRunning_ = true;
    private string name_;
    private long processed_;
    private long target_;

    public string Name
    {
      get
      {
        return this.name_;
      }
    }

    public bool ContinueRunning
    {
      get
      {
        return this.continueRunning_;
      }
      set
      {
        this.continueRunning_ = value;
      }
    }

    public float PercentComplete
    {
      get
      {
        return this.target_ > 0L ? (float) ((double) this.processed_ / (double) this.target_ * 100.0) : 0.0f;
      }
    }

    public long Processed
    {
      get
      {
        return this.processed_;
      }
    }

    public long Target
    {
      get
      {
        return this.target_;
      }
    }

    public ProgressEventArgs(string name, long processed, long target)
    {
      this.name_ = name;
      this.processed_ = processed;
      this.target_ = target;
    }
  }
}
