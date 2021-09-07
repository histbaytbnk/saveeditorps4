
// Type: CSUST.Data.CellBackColorEventArgs


// Hacked by SystemAce

using System;
using System.Drawing;

namespace CSUST.Data
{
  public class CellBackColorEventArgs : EventArgs
  {
    private Color m_BackColor = Color.Empty;
    private int m_RowIndex;
    private int m_ColIndex;

    public int RowIndex
    {
      get
      {
        return this.m_RowIndex;
      }
    }

    public int ColIndex
    {
      get
      {
        return this.m_ColIndex;
      }
    }

    public Color BackColor
    {
      get
      {
        return this.m_BackColor;
      }
      set
      {
        this.m_BackColor = value;
      }
    }

    public CellBackColorEventArgs(int row, int col)
    {
      this.m_RowIndex = row;
      this.m_ColIndex = col;
    }
  }
}
