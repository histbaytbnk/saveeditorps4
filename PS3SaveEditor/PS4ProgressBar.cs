
// Type: PS3SaveEditor.PS4ProgressBar


// Hacked by SystemAce

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class PS4ProgressBar : ProgressBar
  {
    private IContainer components;

    public PS4ProgressBar()
    {
      this.InitializeComponent();
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0, 181, (int) byte.MaxValue), Color.FromArgb(0, 62, 207), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, 0.0f, 0.0f, (float) this.ClientRectangle.Width * (float) this.Value / (float) this.Maximum, (float) this.ClientRectangle.Height);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
    }
  }
}
