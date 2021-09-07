
// Type: PS3SaveEditor.CustomGroupBox


// Hacked by SystemAce

using System.Drawing;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class CustomGroupBox : GroupBox
  {
    protected override void OnPaint(PaintEventArgs e)
    {
      e.Graphics.DrawRectangle(Pens.White, new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top + 4, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 6));
    }
  }
}
