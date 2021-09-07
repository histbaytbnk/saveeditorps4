
// Type: Be.Windows.Forms.ByteProviderChanged


// Hacked by SystemAce

using System;

namespace Be.Windows.Forms
{
  public class ByteProviderChanged : EventArgs
  {
    public long Index { get; set; }

    public byte OldValue { get; set; }

    public byte NewValue { get; set; }

    public ChangeType ChangeType { get; set; }
  }
}
