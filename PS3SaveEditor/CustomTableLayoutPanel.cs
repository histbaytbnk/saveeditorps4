
// Type: PS3SaveEditor.CustomTableLayoutPanel


// Hacked by SystemAce

using System.ComponentModel;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class CustomTableLayoutPanel : TableLayoutPanel
  {
    private IContainer components;

    public CustomTableLayoutPanel()
    {
      this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }
  }
}
