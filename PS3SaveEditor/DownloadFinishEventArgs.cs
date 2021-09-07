
// Type: PS3SaveEditor.DownloadFinishEventArgs


// Hacked by SystemAce

using System;

namespace PS3SaveEditor
{
  public class DownloadFinishEventArgs : EventArgs
  {
    private bool m_status;
    private string m_error;

    public bool Status
    {
      get
      {
        return this.m_status;
      }
    }

    public string Error
    {
      get
      {
        return this.m_error;
      }
    }

    public DownloadFinishEventArgs(bool status, string error)
    {
      this.m_status = status;
      this.m_error = error;
    }
  }
}
