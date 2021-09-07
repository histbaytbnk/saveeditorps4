
// Type: CSUST.Data.CustomDataGridView


// Hacked by SystemAce

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CSUST.Data
{
  public class CustomDataGridView : DataGridView
  {
    private Pen borderPen;
    private Brush brSelection;

    [Description("Set cell background color, Colindex -1 denotes any col.")]
    public event EventHandler<CellBackColorEventArgs> SetCellBackColor;

    public CustomDataGridView()
    {
      this.brSelection = (Brush) new SolidBrush(Color.FromArgb(0, 175, (int) byte.MaxValue));
      this.borderPen = new Pen(Color.FromArgb(168, 173, 179), 1f);
    }

    private void DrawCellBackColor(DataGridViewCellPaintingEventArgs e)
    {
      if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
        base.OnCellPainting(e);
      else if (this.SetCellBackColor == null)
      {
        base.OnCellPainting(e);
      }
      else
      {
        CellBackColorEventArgs e1 = new CellBackColorEventArgs(e.RowIndex, e.ColumnIndex);
        this.SetCellBackColor((object) this, e1);
        if (e1.BackColor == Color.Empty)
        {
          base.OnCellPainting(e);
        }
        else
        {
          using (SolidBrush solidBrush = new SolidBrush(e1.BackColor))
          {
            using (Pen pen = new Pen(this.GridColor))
            {
              Rectangle rect1 = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
              Rectangle rect2 = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
              --rect1.X;
              --rect1.Y;
              --rect2.Width;
              --rect2.Height;
              e.Graphics.DrawRectangle(pen, rect1);
              e.Graphics.FillRectangle((Brush) solidBrush, rect2);
            }
          }
          e.PaintContent(e.CellBounds);
          e.Handled = true;
        }
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      e.Graphics.DrawRectangle(this.borderPen, 0, 0, this.Width - 1, this.Height - 1);
    }

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
      {
        if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null && (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "GameFile" || this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "CheatGroup" || this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "NoCheats"))
        {
          if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
          {
            e.Graphics.FillRectangle(this.brSelection, e.CellBounds);
            e.Graphics.DrawLine(Pens.Gray, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
            e.Graphics.DrawLine(Pens.Gray, e.CellBounds.Left, e.CellBounds.Bottom, e.CellBounds.Right, e.CellBounds.Bottom);
            e.Handled = true;
          }
          else
          {
            if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "NoCheats")
            {
              e.Graphics.DrawRectangle(Pens.White, new Rectangle(e.CellBounds.Left, e.CellBounds.Top + 1, e.CellBounds.Width, e.CellBounds.Height - 2));
              e.Graphics.FillRectangle(Brushes.White, new Rectangle(e.CellBounds.Left, e.CellBounds.Top + 1, e.CellBounds.Width, e.CellBounds.Height - 2));
            }
            else if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "CheatGroup")
            {
              e.Graphics.FillRectangle(Brushes.White, e.CellBounds.Left, e.CellBounds.Top + 1, e.CellBounds.Width, e.CellBounds.Height - 1);
            }
            else
            {
              e.Graphics.DrawRectangle(Pens.Gray, e.CellBounds);
              e.Graphics.FillRectangle(Brushes.Gray, e.CellBounds);
            }
            e.Handled = true;
          }
        }
        else
        {
          if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
          {
            e.Graphics.FillRectangle(this.brSelection, e.CellBounds);
          }
          else
          {
            Brush brush = (Brush) new SolidBrush(e.CellStyle.BackColor);
            e.Graphics.FillRectangle(brush, e.CellBounds);
            brush.Dispose();
          }
          e.Graphics.DrawLine(Pens.Gray, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
          e.Graphics.DrawLine(Pens.Gray, e.CellBounds.Left, e.CellBounds.Bottom, e.CellBounds.Right, e.CellBounds.Bottom);
          e.PaintContent(e.CellBounds);
          e.Handled = true;
        }
      }
      else
        base.OnCellPainting(e);
    }
  }
}
