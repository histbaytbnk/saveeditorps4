
// Type: PS3SaveEditor.AddCode


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class AddCode : Form
  {
    private const int MAX_CHEAT_CODES = 128;
    private AddCode.Mode m_bMode;
    private string m_cheatFile;
    public string Code;
    private List<string> m_existingCodes;
    private IContainer components;
    private TextBox txtDescription;
    private DataGridView dataGridView1;
    private DataGridViewTextBoxColumn Location;
    private DataGridViewTextBoxColumn Value;
    private Button btnSave;
    private Button btnCancel;
    private TextBox txtCode;
    private Label lblCodes;
    private TextBox txtComment;
    private Label lblComment;
    private Panel panel1;
    private Label lblDescription;

    public string Description { get; set; }

    public string Comment { get; set; }

    public AddCode(List<string> existingCodes)
    {
      this.InitializeComponent();
      this.m_existingCodes = existingCodes;
      this.panel1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.lblCodes.BackColor = Color.Transparent;
      this.lblComment.BackColor = Color.Transparent;
      this.lblDescription.BackColor = Color.Transparent;
      this.CenterToScreen();
      this.dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
      this.dataGridView1.KeyDown += new KeyEventHandler(this.dataGridView1_KeyDown);
      this.m_bMode = AddCode.Mode.ADD_MODE;
    }

    public AddCode(cheat item, List<string> existingCodes)
    {
      this.m_bMode = AddCode.Mode.EDIT_MODE;
      this.m_existingCodes = existingCodes;
      this.InitializeComponent();

      this.CenterToScreen();
      this.dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
      this.dataGridView1.KeyDown += new KeyEventHandler(this.dataGridView1_KeyDown);
      this.txtCode.Text = item.ToEditableString();
      this.txtDescription.Text = item.name;
      this.txtComment.Text = item.note;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
        return;
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0, 138, 213), Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.F || e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 || (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)))
        return;
      e.SuppressKeyPress = true;
    }

    private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
        return;
      string s = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
      int result = 0;
      if (!int.TryParse(s, NumberStyles.HexNumber, (IFormatProvider) null, out result))
      {
        this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object) null;
      }
      else
        this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object) result.ToString("X8");
    }

    public static byte[] ConvertHexStringToByteArray(string hexString)
    {
      if (hexString.Length % 2 != 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", new object[1]
        {
          (object) hexString
        }));
      byte[] numArray = new byte[hexString.Length / 2];
      for (int index = 0; index < numArray.Length; ++index)
      {
        string s = hexString.Substring(index * 2, 2);
        numArray[index] = byte.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return numArray;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
            
      {
        foreach (string str in this.txtCode.Lines)
        {
          if (str.Trim().Length != 17 && str.Trim().Length != 0)
          {
            return;
          }
        }
        if ((int) this.txtCode.Lines[0][0] == 70)
        {
          if (this.txtCode.Lines.Length > 16)
          {
            return;
          }
          string str = this.txtCode.Text.Replace(" ", "").Replace("\r\n", "");
          if ((int) this.GetCRC(Encoding.ASCII.GetBytes(str.Substring(0, str.Length - 8))) != (int) uint.Parse(str.Substring(str.Length - 8, 8), NumberStyles.HexNumber))
          {
            return;
          }
        }
         this.Description = this.txtDescription.Text;
        this.Comment = this.txtComment.Text;
        this.Code = this.txtCode.Text.Replace("\r\n", " ").TrimEnd();
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void txtCheatCode_TextChanged(object sender, EventArgs e)
    {
      int selectionStart = this.txtCode.SelectionStart;
      int length = this.txtCode.Lines.Length;
      if (length > 1 && (this.txtCode.Lines[length - 2].Length < 17 || this.txtCode.Lines[length - 1].Length == 0))
        --length;
      if (length > 128)
      {
        string[] lines = new string[128];
        Array.Copy((Array) this.txtCode.Lines, (Array) lines, 128);
        this.SetLinesToCode(lines);
        this.txtCode.SelectionStart = this.txtCode.TextLength;
        this.txtCode.SelectionLength = 0;
      }
      else
      {
        if (length <= 0)
          return;
        this.SetLinesToCode(this.txtCode.Lines);
      }
    }

    private void txtCheatCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Delete)
        return;
      int num = this.txtCode.SelectionStart - this.txtCode.GetFirstCharIndexOfCurrentLine();
      int lineFromCharIndex = this.txtCode.GetLineFromCharIndex(this.txtCode.SelectionStart);
      string[] lines = this.txtCode.Lines;
      if (lines.Length > 0)
      {
        string str = lines[lineFromCharIndex];
        if (num > 0 && num >= str.Length)
        {
          e.SuppressKeyPress = true;
          return;
        }
      }
      if (num >= 17)
        e.SuppressKeyPress = true;
      if (num != 8)
        return;
      ++this.txtCode.SelectionStart;
    }

    private void SetLinesToCode(string[] lines)
    {
      string str1 = "";
      int index1 = 0;
      int index2 = this.txtCode.SelectionStart;
      for (int index3 = 0; index3 < lines.Length; ++index3)
      {
        if (index3 < lines.Length - 1 || lines[index3].Length > 0)
        {
          string str2 = lines[index1].Replace(" ", "");
          for (int startIndex = 0; startIndex < str2.Length; ++startIndex)
          {
            if (((int) str2[startIndex] < 48 || (int) str2[startIndex] > 57) && ((int) str2[startIndex] < 97 || (int) str2[startIndex] > 102) && ((int) str2[startIndex] < 65 || (int) str2[startIndex] > 70))
              str2 = str2.Remove(startIndex, 1);
          }
          string str3 = str2.Length <= 8 ? str2 + Environment.NewLine : str2.Substring(0, 8) + " " + str2.Substring(8, Math.Min(8, str2.Length - 8)) + Environment.NewLine;
          str1 += str3;
          ++index1;
        }
      }
      this.txtCode.GetLineFromCharIndex(index2);
      lines = this.txtCode.Lines;
      int num = 0;
      foreach (string str2 in lines)
      {
        if (str2.Length > 0 && str2.Length > 17)
          index2 = (num + 1) * 18;
        ++num;
      }
      this.txtCode.Text = str1;
      if (index2 <= 0)
        return;
      this.txtCode.SelectionStart = index2;
      this.txtCode.ScrollToCaret();
    }

    private void HandleCodeBackSpace(ref KeyPressEventArgs e)
    {
      int num = this.txtCode.SelectionStart - this.txtCode.GetFirstCharIndexOfCurrentLine();
      if (num < 0)
        num = this.txtCode.SelectionStart;
      string[] lines = this.txtCode.Lines;
      int lineFromCharIndex = this.txtCode.GetLineFromCharIndex(this.txtCode.SelectionStart);
      if (lines.Length == 0)
        return;
      if (num == 0 && this.txtCode.SelectionStart > 0 && lines[lineFromCharIndex].Length > 0)
      {
        e.Handled = true;
        this.txtCode.SelectionStart -= 2;
      }
      else
      {
        if (num != 9)
          return;
        --this.txtCode.SelectionStart;
      }
    }

    private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
    {
      if ((int) e.KeyChar == 8)
        this.HandleCodeBackSpace(ref e);
      else if ((int) e.KeyChar >= 48 && (int) e.KeyChar <= 57 || (int) e.KeyChar >= 97 && (int) e.KeyChar <= 102 || (int) e.KeyChar >= 65 && (int) e.KeyChar <= 70)
      {
        int length = this.txtCode.Lines.Length;
        if (length > 1 && this.txtCode.Lines[length - 2].Length < 17)
          --length;
        if (length > 128)
        {
          e.Handled = true;
        }
        else
        {
          int lineFromCharIndex = this.txtCode.GetLineFromCharIndex(this.txtCode.SelectionStart);
          string str = "";
          string[] lines = this.txtCode.Lines;
          if (this.txtCode.Lines.Length > 0)
            str = this.txtCode.Lines[lineFromCharIndex];
          else
            lines = new string[1];
          int index = this.txtCode.SelectionStart - this.txtCode.GetFirstCharIndexOfCurrentLine();
          if (index < 0)
            index = this.txtCode.SelectionStart;
          if (this.txtCode.TextLength > this.txtCode.SelectionStart && (int) this.txtCode.Text[this.txtCode.SelectionStart] == 10)
            --index;
          int selectionStart = this.txtCode.SelectionStart;
          if (index == 17)
          {
            this.txtCode.GetFirstCharIndexFromLine(lineFromCharIndex + 1);
            char[] chArray = lines[lineFromCharIndex + 1].ToCharArray();
            if (chArray.Length == 0)
              chArray = new char[1];
            chArray[0] = e.KeyChar;
            lines[lineFromCharIndex + 1] = new string(chArray);
            this.SetLinesToCode(lines);
            this.txtCode.SelectionStart = this.txtCode.GetFirstCharIndexFromLine(lineFromCharIndex + 1) + 1;
            if (this.txtCode.SelectionStart > 0)
              this.txtCode.ScrollToCaret();
            e.Handled = true;
          }
          else
          {
            char[] chArray1 = str.ToCharArray();
            if (chArray1.Length == 17)
            {
              if (index == 8)
              {
                chArray1[index + 1] = e.KeyChar;
                lines[lineFromCharIndex] = new string(chArray1);
                this.SetLinesToCode(lines);
                this.txtCode.SelectionStart += 2;
                e.Handled = true;
              }
              else
              {
                chArray1[index] = e.KeyChar;
                lines[lineFromCharIndex] = new string(chArray1);
                this.SetLinesToCode(lines);
                ++this.txtCode.SelectionStart;
                e.Handled = true;
              }
            }
            else if (index == 8 && chArray1.Length == 8)
            {
              char[] chArray2 = new char[chArray1.Length + 2];
              Array.Copy((Array) chArray1, (Array) chArray2, 8);
              chArray2[8] = ' ';
              chArray2[9] = e.KeyChar;
              lines[lineFromCharIndex] = new string(chArray2);
              this.SetLinesToCode(lines);
              this.txtCode.SelectionStart += 2;
              e.Handled = true;
            }
            else if (index == 8 && chArray1.Length > 8)
            {
              char[] chArray2 = new char[chArray1.Length + 1];
              Array.Copy((Array) chArray1, (Array) chArray2, 8);
              chArray2[8] = ' ';
              chArray2[9] = e.KeyChar;
              Array.Copy((Array) chArray1, 9, (Array) chArray2, 10, chArray1.Length - 9);
              lines[lineFromCharIndex] = new string(chArray2);
              this.SetLinesToCode(lines);
              this.txtCode.SelectionStart += 2;
              e.Handled = true;
            }
            else
            {
              if (index <= 8)
                return;
              char[] chArray2 = new char[chArray1.Length + 1];
              Array.Copy((Array) chArray1, (Array) chArray2, index);
              chArray2[index] = e.KeyChar;
              Array.Copy((Array) chArray1, index, (Array) chArray2, index + 1, chArray1.Length - index);
              lines[lineFromCharIndex] = new string(chArray2);
              this.SetLinesToCode(lines);
              ++this.txtCode.SelectionStart;
              e.Handled = true;
            }
          }
        }
      }
      else if ((int) e.KeyChar == 1)
      {
        this.txtCode.SelectAll();
      }
      else
      {
        if ((int) e.KeyChar == 3 || (int) e.KeyChar == 13 || ((int) e.KeyChar == 24 || (int) e.KeyChar == 22) || (int) e.KeyChar == 26)
          return;
        e.Handled = true;
      }
    }

    private uint GetCRC(byte[] data)
    {
      Crc32Net crc32Net = new Crc32Net();
      crc32Net.ComputeHash(data);
      return crc32Net.CrcValue;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      this.txtDescription = new TextBox();
      this.dataGridView1 = new DataGridView();
      this.Location = new DataGridViewTextBoxColumn();
      this.Value = new DataGridViewTextBoxColumn();
      this.btnSave = new Button();
      this.btnCancel = new Button();
      this.txtCode = new TextBox();
      this.lblCodes = new Label();
      this.txtComment = new TextBox();
      this.lblComment = new Label();
      this.panel1 = new Panel();
      this.lblDescription = new Label();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.txtDescription.Location = new Point(12, 28);
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.Size = new Size(181, 20);
      this.txtDescription.TabIndex = 0;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dataGridView1.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.Location, (DataGridViewColumn) this.Value);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dataGridView1.DefaultCellStyle = gridViewCellStyle2;
      this.dataGridView1.Location = new Point(265, 52);
      this.dataGridView1.Name = "dataGridView1";
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dataGridView1.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dataGridView1.Size = new Size(12, 10);
      this.dataGridView1.TabIndex = 2;
      this.dataGridView1.Visible = false;
      this.Location.HeaderText = "Location";
      this.Location.Name = "Location";
      this.Value.HeaderText = "Value";
      this.Value.Name = "Value";
      this.btnSave.Location = new Point(25, 282);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(75, 23);
      this.btnSave.TabIndex = 3;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = false;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.btnCancel.Location = new Point(104, 282);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(75, 23);
      this.btnCancel.TabIndex = 4;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.txtCode.CharacterCasing = CharacterCasing.Upper;
      this.txtCode.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.txtCode.Location = new Point(12, 112);
      this.txtCode.Multiline = true;
      this.txtCode.Name = "txtCode";
      this.txtCode.ScrollBars = ScrollBars.Vertical;
      this.txtCode.Size = new Size(181, 165);
      this.txtCode.TabIndex = 2;
      this.txtCode.TextChanged += new EventHandler(this.txtCheatCode_TextChanged);
      this.txtCode.KeyDown += new KeyEventHandler(this.txtCheatCode_KeyDown);
      this.txtCode.KeyPress += new KeyPressEventHandler(this.txtCode_KeyPress);
      this.lblCodes.BackColor = Color.Transparent;
      this.lblCodes.ForeColor = Color.White;
      this.lblCodes.Location = new Point(12, 97);
      this.lblCodes.Name = "lblCodes";
      this.lblCodes.Size = new Size(111, 13);
      this.lblCodes.TabIndex = 6;
      this.lblCodes.Text = "Cheat Codes:";
      this.txtComment.Location = new Point(12, 70);
      this.txtComment.Name = "txtComment";
      this.txtComment.Size = new Size(181, 20);
      this.txtComment.TabIndex = 1;
      this.lblComment.BackColor = Color.Transparent;
      this.lblComment.ForeColor = Color.White;
      this.lblComment.Location = new Point(12, 55);
      this.lblComment.Name = "lblComment";
      this.lblComment.Size = new Size(93, 13);
      this.lblComment.TabIndex = 7;
      this.lblComment.Text = "Comment:";
      this.panel1.AutoScroll = true;
      this.panel1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.panel1.Controls.Add((Control) this.lblDescription);
      this.panel1.Controls.Add((Control) this.txtComment);
      this.panel1.Controls.Add((Control) this.btnSave);
      this.panel1.Controls.Add((Control) this.lblComment);
      this.panel1.Controls.Add((Control) this.lblCodes);
      this.panel1.Controls.Add((Control) this.txtCode);
      this.panel1.Controls.Add((Control) this.btnCancel);
      this.panel1.Controls.Add((Control) this.txtDescription);
      this.panel1.Location = new Point(10, 11);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(205, 315);
      this.panel1.TabIndex = 8;
      this.lblDescription.BackColor = Color.Transparent;
      this.lblDescription.ForeColor = Color.White;
      this.lblDescription.Location = new Point(12, 13);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new Size(93, 13);
      this.lblDescription.TabIndex = 8;
      this.lblDescription.Text = "Description:";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Black;
      this.ClientSize = new Size(225, 337);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.dataGridView1);
      this.FormBorderStyle = FormBorderStyle.Fixed3D;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddCode";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Add Cheat";
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }

    private enum Mode
    {
      ADD_MODE,
      EDIT_MODE,
    }
  }
}
