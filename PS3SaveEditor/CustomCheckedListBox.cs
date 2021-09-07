
// Type: PS3SaveEditor.CustomCheckedListBox


// Hacked by SystemAce

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PS3SaveEditor
{
  public class CustomCheckedListBox : CheckedListBox
  {
    private IContainer components;

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      e.DrawBackground();
      if (e.State == DrawItemState.Selected)
        e.Graphics.FillRectangle((Brush) new SolidBrush(Color.FromArgb(0, 175, (int) byte.MaxValue)), e.Bounds);
      string checkBoxText = this.Items[e.Index].ToString();
      CheckBoxState state = this.GetItemChecked(e.Index) ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
      Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
      CheckBoxRenderer.DrawCheckBox(e.Graphics, e.Bounds.Location, new Rectangle(new Point(e.Bounds.X + glyphSize.Width, e.Bounds.Y), new Size(e.Bounds.Width - glyphSize.Width, e.Bounds.Height)), checkBoxText, this.Font, false, state);
      e.DrawFocusRectangle();
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
