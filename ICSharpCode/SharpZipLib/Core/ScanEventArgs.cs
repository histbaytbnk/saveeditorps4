
// Type: ICSharpCode.SharpZipLib.Core.ScanEventArgs


// Hacked by SystemAce

using System;

namespace ICSharpCode.SharpZipLib.Core
{
  public class ScanEventArgs : EventArgs
  {
    private bool continueRunning_ = true;
    private string name_;

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

    public ScanEventArgs(string name)
    {
      this.name_ = name;
    }
  }
}
