using Be.Windows.Forms;
using Microsoft.Win32;
using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class AdvancedEdit : Form
  {
    private Dictionary<string, Stack<ActionItem>> m_undoList = new Dictionary<string, Stack<ActionItem>>();
    private Dictionary<string, Stack<ActionItem>> m_redoList = new Dictionary<string, Stack<ActionItem>>();
    private DynamicByteProvider provider;
    private game m_game;
    private bool m_bTextMode;
    private byte[] DependentData;
    private Dictionary<string, byte[]> m_saveFilesData;
    private List<string> m_DirtyFiles;
    private string m_cursaveFile;
    private bool _resizeInProgress;
    private Search m_searchForm;
    private IContainer components;
    private HexBox hexBox1;
    private Label lblCheatCodes;
    private Label lblCheats;
    private Button btnApply;
    private Label lblOffset;
    private Label lblOffsetValue;
    private Panel panel1;
    private ListBox lstCheats;
    private ListBox lstValues;
    private Label lblGameName;
    private Button btnClose;
    private Label label1;
    private Button btnFindPrev;
    private Button btnFind;
    private Label lblAddress;
    private ComboBox cbProfile;
    private Label lblProfile;
    private RichTextBox txtSaveData;
    private ComboBox cbSaveFiles;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButtonSearch;
    private ToolStripButton toolStripButtonUndo;
    private ToolStripButton toolStripButtonRedo;
    private ToolStripButton toolStripButtonGoto;
    private ToolStripButton toolStripButtonExport;
    private Label lblCurrentFile;
    private Label lblLengthVal;
    private Label lblLength;

    public bool TextMode
    {
      get
      {
        return this.m_bTextMode;
      }
      set
      {
        this.m_searchForm.TextMode = value;
        this.m_bTextMode = value;
      }
    }

    public AdvancedEdit(game game, Dictionary<string, byte[]> data, byte[] dependentData)
    {
      this.InitializeComponent();
      this.KeyDown += new KeyEventHandler(this.AdvancedEdit_KeyDown);
      this.btnFindPrev.Click += new EventHandler(this.button1_Click);
      this.btnFind.Click += new EventHandler(this.btnFind_Click);
      this.hexBox1.KeyDown += new KeyEventHandler(this.hexBox1_KeyDown);
      this.hexBox1.SelectionBackColor = System.Drawing.Color.FromArgb(0, 175, (int) byte.MaxValue);
      this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(204, 240, (int) byte.MaxValue);
      this.lstCheats.DrawMode = DrawMode.OwnerDrawFixed;
      this.lstCheats.DrawItem += new DrawItemEventHandler(this.lstCheats_DrawItem);
      this.lstValues.DrawMode = DrawMode.OwnerDrawFixed;
      this.lstValues.DrawItem += new DrawItemEventHandler(this.lstValues_DrawItem);
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
      this.m_searchForm = new Search(this);
      this.TextMode = false;
      this.btnApply.BackColor = SystemColors.ButtonFace;
      this.btnApply.ForeColor = System.Drawing.Color.Black;
      this.btnClose.BackColor = SystemColors.ButtonFace;
      this.btnClose.ForeColor = System.Drawing.Color.Black;
      this.btnFind.BackColor = SystemColors.ButtonFace;
      this.btnFind.ForeColor = System.Drawing.Color.Black;
      this.btnFindPrev.BackColor = SystemColors.ButtonFace;
      this.btnFindPrev.ForeColor = System.Drawing.Color.Black;
      this.panel1.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.label1.BackColor = this.lblAddress.BackColor = this.lblCheatCodes.BackColor = this.lblCheats.BackColor = this.lblGameName.BackColor = this.lblOffset.BackColor = this.lblOffsetValue.BackColor = this.lblProfile.BackColor = System.Drawing.Color.Transparent;
      this.lblProfile.Visible = true;
      this.cbProfile.Visible = true;
      this.m_DirtyFiles = new List<string>();
      this.m_saveFilesData = data;
      this.DependentData = dependentData;
      this.SetLabels();
      this.FillProfiles();
      this.lblGameName.Text = game.name;
      this.m_game = game;
      this.CenterToScreen();
      this.btnApply.Enabled = false;
      this.lstValues.SelectedIndexChanged += new EventHandler(this.lstValues_SelectedIndexChanged);
      this.lstCheats.SelectedIndexChanged += new EventHandler(this.lstCheats_SelectedIndexChanged);
      this.cbSaveFiles.SelectedIndexChanged += new EventHandler(this.cbSaveFiles_SelectedIndexChanged);
      if (this.m_game.GetTargetGameFolder() != null)
      {
        this.cbSaveFiles.Sorted = false;
        foreach (object obj in data.Keys)
          this.cbSaveFiles.Items.Add(obj);
        if (this.cbSaveFiles.Items.Count > 0)
          this.cbSaveFiles.SelectedIndex = 0;
      }
      if (this.cbSaveFiles.Items.Count == 1)
        this.cbSaveFiles.Enabled = false;
      this.btnApply.Click += new EventHandler(this.btnApply_Click);
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      if (this.lstCheats.Items.Count > 0)
        this.lstCheats.SelectedIndex = 0;
      this.ResizeBegin += (EventHandler) ((s, e) =>
      {
        this.SuspendLayout();
        this.panel1.BackColor = System.Drawing.Color.FromArgb(0, 138, 213);
        this._resizeInProgress = true;
      });
      this.ResizeEnd += (EventHandler) ((s, e) =>
      {
        this.ResumeLayout(true);
        this._resizeInProgress = false;
        this.panel1.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
        this.Invalidate(true);
      });
      this.SizeChanged += (EventHandler) ((s, e) =>
      {
        if (this.WindowState != FormWindowState.Maximized)
          return;
        this._resizeInProgress = false;
        this.panel1.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
        this.Invalidate(true);
      });
      this.Disposed += new EventHandler(this.AdvancedEdit_Disposed);
      this.toolStripButtonExport.Click += new EventHandler(this.toolStripButtonExport_Click);
      this.toolStripButtonGoto.Click += new EventHandler(this.toolStripButtonGoto_Click);
      this.toolStripButtonUndo.Click += new EventHandler(this.toolStripButtonUndo_Click);
      this.toolStripButtonRedo.Click += new EventHandler(this.toolStripButtonRedo_Click);
      this.toolStripButtonSearch.Click += new EventHandler(this.toolStripButtonSearch_Click);
      this.cbSaveFiles.Width = Math.Min(200, this.ComboBoxWidth(this.cbSaveFiles));
    }

    private void AdvancedEdit_Disposed(object sender, EventArgs e)
    {
      if (this.m_searchForm.IsDisposed)
        return;
      this.m_searchForm.Dispose();
    }

    private void toolStripButtonSearch_Click(object sender, EventArgs e)
    {
      this.m_searchForm.Hide();
      this.m_searchForm.Show((IWin32Window) this);
    }

    private void toolStripButtonRedo_Click(object sender, EventArgs e)
    {
      if (this.m_redoList[this.m_cursaveFile].Count > 0)
      {
        ActionItem actionItem = this.m_redoList[this.m_cursaveFile].Pop();
        this.m_undoList[this.m_cursaveFile].Push(actionItem);
        this.hexBox1.ScrollByteIntoView(actionItem.Location);
        this.hexBox1.ByteProvider.WriteByte(actionItem.Location, actionItem.NewValue, true);
        this.hexBox1.Refresh();
      }
      if (this.m_undoList[this.m_cursaveFile].Count == 0)
        this.toolStripButtonUndo.Enabled = false;
      else
        this.toolStripButtonUndo.Enabled = true;
      if (this.m_redoList[this.m_cursaveFile].Count == 0)
        this.toolStripButtonRedo.Enabled = false;
      else
        this.toolStripButtonRedo.Enabled = true;
    }

    private void toolStripButtonUndo_Click(object sender, EventArgs e)
    {
      if (this.m_undoList[this.m_cursaveFile].Count > 0)
      {
        ActionItem actionItem = this.m_undoList[this.m_cursaveFile].Pop();
        this.m_redoList[this.m_cursaveFile].Push(actionItem);
        this.hexBox1.ScrollByteIntoView(actionItem.Location);
        this.hexBox1.ByteProvider.WriteByte(actionItem.Location, actionItem.Value, true);
        this.hexBox1.Refresh();
      }
      if (this.m_undoList[this.m_cursaveFile].Count == 0)
        this.toolStripButtonUndo.Enabled = false;
      else
        this.toolStripButtonUndo.Enabled = true;
      if (this.m_redoList[this.m_cursaveFile].Count == 0)
        this.toolStripButtonRedo.Enabled = false;
      else
        this.toolStripButtonRedo.Enabled = true;
    }

    private void toolStripButtonGoto_Click(object sender, EventArgs e)
    {
      this.DoGoTo();
    }

    private void toolStripButtonExport_Click(object sender, EventArgs e)
    {
      byte[] bytes = this.m_saveFilesData[this.m_cursaveFile];
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.FileName = this.m_cursaveFile;
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      File.WriteAllBytes(saveFileDialog.FileName, bytes);
    }

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 274 && m.WParam == new IntPtr(61488))
      {
        this.panel1.BackColor = System.Drawing.Color.FromArgb(0, 138, 213);
        this._resizeInProgress = true;
      }
      base.WndProc(ref m);
    }

    private int ComboBoxWidth(ComboBox myCombo)
    {
      int num = 0;
      foreach (object obj in myCombo.Items)
      {
        int width = TextRenderer.MeasureText(myCombo.GetItemText(obj), myCombo.Font).Width;
        if (width > num)
          num = width;
      }
      return num + SystemInformation.VerticalScrollBarWidth;
    }

    private void lstValues_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      e.DrawBackground();
      Graphics graphics = e.Graphics;
      if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
      {
        graphics.FillRectangle((Brush) new SolidBrush(System.Drawing.Color.FromArgb(72, 187, 97)), new Rectangle(e.Bounds.Left, e.Bounds.Top, this.lstValues.Width, e.Bounds.Height));
        e.Graphics.DrawString((string) this.lstValues.Items[e.Index], e.Font, (Brush) new SolidBrush(System.Drawing.Color.White), (RectangleF) e.Bounds, StringFormat.GenericDefault);
      }
      else
        e.Graphics.DrawString((string) this.lstValues.Items[e.Index], e.Font, (Brush) new SolidBrush(System.Drawing.Color.Black), (RectangleF) new Rectangle(e.Bounds.Left, e.Bounds.Top, this.lstValues.Width, e.Bounds.Height), StringFormat.GenericDefault);
      e.DrawFocusRectangle();
    }

    private void lstCheats_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      e.DrawBackground();
      Graphics graphics = e.Graphics;
      if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
      {
        graphics.FillRectangle((Brush) new SolidBrush(System.Drawing.Color.FromArgb(0, 175, (int) byte.MaxValue)), e.Bounds);
        e.Graphics.DrawString((string) this.lstCheats.Items[e.Index], e.Font, (Brush) new SolidBrush(System.Drawing.Color.White), (RectangleF) e.Bounds, StringFormat.GenericDefault);
      }
      else
        e.Graphics.DrawString((string) this.lstCheats.Items[e.Index], e.Font, (Brush) new SolidBrush(System.Drawing.Color.Black), (RectangleF) e.Bounds, StringFormat.GenericDefault);
      e.DrawFocusRectangle();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (this._resizeInProgress)
        return;
      base.OnPaint(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      if (this._resizeInProgress || this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
        return;
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, System.Drawing.Color.FromArgb(0, 138, 213), System.Drawing.Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void txtSaveData_TextChanged(object sender, EventArgs e)
    {
      this.btnApply.Enabled = true;
      if (this.m_DirtyFiles.IndexOf(this.m_cursaveFile) >= 0)
        return;
      this.m_DirtyFiles.Add(this.m_cursaveFile);
    }

    private void SetLabels()
    {
    }

    private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
    {
      this.lblOffsetValue.Text = "0x" + string.Format("{0:X}", (object) ((long) this.hexBox1.BytesPerLine * (this.hexBox1.CurrentLine - 1L) + (this.hexBox1.CurrentPositionInLine - 1L))).PadLeft(8, '0');
    }

    protected override void OnClosed(EventArgs e)
    {
      this.hexBox1.Dispose();
      base.OnClosed(e);
    }

    private void provider_LengthChanged(object sender, EventArgs e)
    {
    }

    private void provider_Changed(object sender, ByteProviderChanged e)
    {
      this.btnApply.Enabled = true;
      if (this.m_DirtyFiles.IndexOf(this.m_cursaveFile) < 0)
        this.m_DirtyFiles.Add(this.m_cursaveFile);
      this.m_undoList[this.m_cursaveFile].Push(new ActionItem()
      {
        Location = e.Index,
        Value = e.OldValue,
        NewValue = e.NewValue
      });
      this.toolStripButtonUndo.Enabled = true;
    }

    private void lstCheats_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.m_bTextMode)
        return;
      this.lstValues.Items.Clear();
      int selectedIndex = this.lstCheats.SelectedIndex;
      string a = this.cbSaveFiles.SelectedItem.ToString();
      if (selectedIndex < 0)
        return;
      container targetGameFolder = this.m_game.GetTargetGameFolder();
      if (targetGameFolder == null)
        return;
      foreach (file file in targetGameFolder.files._files)
      {
        List<string> saveFiles = this.m_game.GetSaveFiles();
        if (saveFiles != null)
        {
          foreach (string path in saveFiles)
          {
            if (Path.GetFileName(path) == a || Util.IsMatch(a, file.filename))
            {
              cheat cheat = file.GetCheat(this.lstCheats.Items[selectedIndex].ToString());
              if (cheat != null)
              {
                string code = cheat.code;
                if (!string.IsNullOrEmpty(code))
                {
                  string[] strArray = code.Trim().Split(new char[3]
                  {
                    ' ',
                    '\r',
                    '\n'
                  });
                  int index = 0;
                  while (index < strArray.Length - 1)
                  {
                    this.lstValues.Items.Add((object) (strArray[index] + " " + strArray[index + 1]));
                    index += 2;
                  }
                }
              }
            }
          }
        }
      }
    }

    private void lstValues_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.lstValues.SelectedIndex < 0 || this.m_bTextMode || (int) this.lstValues.Items[0].ToString()[0] == 70)
        return;
      this.hexBox1.SelectAddresses.Clear();
      int bitWriteCode;
      long memLocation = cheat.GetMemLocation(this.lstValues.Items[this.lstValues.SelectedIndex].ToString().Split(' ')[0], out bitWriteCode);
      if (this.provider.Length <= memLocation)
        return;
      this.hexBox1.SelectAddresses.Add(memLocation, cheat.GetBitCodeBytes(bitWriteCode));
      this.hexBox1.ScrollByteIntoView(memLocation);
      this.hexBox1.Invalidate();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
    
      if (!this.m_bTextMode)
      {
        this.provider.ApplyChanges();
        if (this.m_cursaveFile == null)
          this.m_cursaveFile = this.cbSaveFiles.SelectedItem.ToString();
        this.m_saveFilesData[this.m_cursaveFile] = this.provider.Bytes.ToArray();
      }
      else
      {
        file gameFile = this.m_game.GetGameFile(this.m_game.GetTargetGameFolder(), this.m_cursaveFile);
        this.m_saveFilesData[this.m_cursaveFile] = gameFile.TextMode != 1 ? (gameFile.TextMode != 3 ? Encoding.ASCII.GetBytes(this.txtSaveData.Text) : Encoding.Unicode.GetBytes(this.txtSaveData.Text)) : Encoding.UTF8.GetBytes(this.txtSaveData.Text);
      }
      if (this.m_game.GetTargetGameFolder() == null)
      {
      }
      else
      {
        this.m_game.GetTargetGameFolder();
        List<string> list = this.m_DirtyFiles;
        List<string> selectedSaveFiles = new List<string>();
        foreach (string path1 in list)
        {
          string path2 = Path.Combine(ZipUtil.GetPs3SeTempFolder(), "_file_" + Path.GetFileName(path1));
          File.WriteAllBytes(path2, this.m_saveFilesData[Path.GetFileName(path1)]);
          if (selectedSaveFiles.IndexOf(path2) < 0)
            selectedSaveFiles.Add(path2);
        }
        List<string> containerFiles = this.m_game.GetContainerFiles();
        string file = this.m_game.LocalSaveFolder.Substring(0, this.m_game.LocalSaveFolder.Length - 4);
        string hash = Util.GetHash(file);
        bool cache = Util.GetCache(hash);
        string contents = this.m_game.ToString(selectedSaveFiles, "encrypt");
        if (cache)
        {
          containerFiles.Remove(file);
          contents = contents.Replace("<pfs><name>" + Path.GetFileNameWithoutExtension(this.m_game.LocalSaveFolder) + "</name></pfs>", "<pfs><name>" + Path.GetFileNameWithoutExtension(this.m_game.LocalSaveFolder) + "</name><md5>" + hash + "</md5></pfs>");
        }
        selectedSaveFiles.AddRange((IEnumerable<string>) containerFiles);
        string path = Util.GetTempFolder() + "ps4_list.xml";
        File.WriteAllText(path, contents);
        selectedSaveFiles.Add(path);
        string profile = (string) this.cbProfile.SelectedItem;
        if (new AdvancedSaveUploaderForEncrypt(selectedSaveFiles.ToArray(), this.m_game, profile, "encrypt").ShowDialog() == DialogResult.OK)
        {
        }
        File.Delete(path);
        Directory.Delete(ZipUtil.GetPs3SeTempFolder(), true);
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void txtSearchValue_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    private void hexBox1_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.F3)
        return;
      this.Search(e.Shift, false, this.m_searchForm.GetSearchMode());
    }

    private void txtSearchValue_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return)
        this.Search(false, true, this.m_searchForm.GetSearchMode());
      if (this.m_bTextMode || e.KeyCode == Keys.Delete || (e.KeyCode == Keys.Back || e.Control) || (e.KeyCode == Keys.Home || e.KeyCode == Keys.End || (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && !e.Shift || e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 && !e.Shift) || (this.m_searchForm.SearchText.SelectionStart == 1 && e.KeyCode == Keys.X && (int) this.m_searchForm.SearchText.Text[0] == 48 || this.m_searchForm.SearchText.Text.StartsWith("0x") && e.KeyCode >= Keys.A && e.KeyCode <= Keys.F))
        return;
      e.SuppressKeyPress = true;
    }

    public void Search(bool bBackward, bool bStart, SearchMode mode)
    {
      if (this.m_bTextMode)
      {
        this.SerachText(bBackward, bStart);
      }
      else
      {
        MemoryStream memoryStream = new MemoryStream((this.hexBox1.ByteProvider as DynamicByteProvider).Bytes.GetBytes());
        BinaryReader reader = new BinaryReader((Stream) memoryStream);
        if (bStart)
        {
          reader.BaseStream.Position = 0L;
          this.hexBox1.SelectionStart = 0L;
          this.hexBox1.SelectionLength = 0L;
        }
        else if (this.hexBox1.SelectionStart >= 0L)
          reader.BaseStream.Position = this.hexBox1.SelectionStart + this.hexBox1.SelectionLength;
        long position = reader.BaseStream.Position;
        uint val1;
        uint val2;
        byte[] val3;
        int searchValues = this.GetSearchValues(mode, out val1, out val2, out val3);
        this.lblLengthVal.Text = "0x" + string.Format("{0:X}", (object) searchValues).PadLeft(8, '0');
        if (searchValues == 0)
        {
          this.m_searchForm.SearchText.Focus();
        }
        else if (searchValues < 0)
        {
          this.m_searchForm.SearchText.Focus();
        }
        else
        {
          for (; reader.BaseStream.Position >= 0L && reader.BaseStream.Position < reader.BaseStream.Length + (bBackward ? (long) searchValues : (long) (1 - searchValues)); reader.BaseStream.Position = position)
          {
            bool flag = true;
            uint num = 0U;
            if (mode == SearchMode.Text)
            {
              byte[] buffer = new byte[searchValues];
              reader.BaseStream.Read(buffer, 0, searchValues);
              for (int index = 0; index < searchValues; ++index)
              {
                if ((int) buffer[index] != (int) val3[index])
                {
                  flag = false;
                  break;
                }
              }
            }
            else
            {
              flag = false;
              num = this.ReadValue(reader, searchValues, bBackward);
            }
            if (mode != SearchMode.Text && ((int) num == (int) val1 || (int) num == (int) val2) || flag)
            {
              this.hexBox1.Select(reader.BaseStream.Position - (long) searchValues, (long) searchValues);
              this.hexBox1.ScrollByteIntoView(reader.BaseStream.Position);
              this.hexBox1.Focus();
              break;
            }
            if (bBackward)
            {
              --position;
              if (position < 0L)
                break;
            }
            else
            {
              ++position;
              if (position > reader.BaseStream.Length)
                break;
            }
          }
          reader.Close();
          memoryStream.Close();
          memoryStream.Dispose();
        }
      }
    }

    public int FindMyText(string text, int start, bool bReverse)
    {
      int num1 = -1;
      if (text.Length > 0 && start >= 0)
      {
        RichTextBoxFinds options = RichTextBoxFinds.None;
        int end = this.txtSaveData.Text.Length;
        if (bReverse)
        {
          options |= RichTextBoxFinds.Reverse;
          end = start - text.Length;
          start = 0;
          if (end < 0)
            end = this.txtSaveData.Text.Length - 1;
        }
        int num2 = this.txtSaveData.Find(text, start, end, options);
        if (num2 >= 0)
          num1 = num2;
      }
      return num1;
    }

    private void SerachText(bool bBackward, bool bStart)
    {
      int start = 0;
      if (!bStart)
        start = this.txtSaveData.SelectionStart + this.txtSaveData.SelectionLength;
      this.lblLengthVal.Text = string.Concat((object) this.m_searchForm.Text.Length);
      if (this.FindMyText(this.m_searchForm.SearchText.Text, start, bBackward) >= 0)
        return;
      this.txtSaveData.Select(0, 0);
    }

    private uint ReadValue(BinaryReader reader, int size, bool bBackward)
    {
      if (bBackward)
      {
        if (reader.BaseStream.Position < (long) (2 * size))
          reader.BaseStream.Position = reader.BaseStream.Length - 1L;
        reader.BaseStream.Position -= (long) (2 * size);
      }
      if (size == 1)
        return (uint) reader.ReadByte();
      if (size == 2)
        return (uint) reader.ReadUInt16();
      if (size == 3)
        return (uint) reader.ReadUInt16() << 8 | (uint) reader.ReadByte();
      return reader.ReadUInt32();
    }

    private int GetSearchValues(SearchMode mode, out uint val1, out uint val2, out byte[] val3)
    {
      uint num1 = 0U;
      int num2 = 0;
      val3 = Encoding.ASCII.GetBytes(this.m_searchForm.SearchText.Text);
      try
      {
        if (mode == SearchMode.Hex)
        {
          num1 = uint.Parse(this.m_searchForm.SearchText.Text, NumberStyles.HexNumber);
          num2 = this.m_searchForm.SearchText.Text.Length;
          switch (num2)
          {
            case 1:
            case 2:
            case 4:
            case 6:
            case 8:
              break;
          }
        }
        else if (mode == SearchMode.Decimal)
        {
          num1 = uint.Parse(this.m_searchForm.SearchText.Text);
          num2 = num1.ToString("X").Length;
        }
        else if (mode == SearchMode.Float)
        {
          num1 = BitConverter.ToUInt32(BitConverter.GetBytes(float.Parse(this.m_searchForm.SearchText.Text)), 0);
          num2 = 8;
        }
      }
      catch (Exception ex)
      {
        val1 = 0U;
        val2 = 0U;
        return -1;
      }
      int num3;
      switch (num2 - 1)
      {
        case 0:
        case 1:
          num3 = 1;
          break;
        case 2:
        case 3:
          num3 = 2;
          break;
        case 4:
        case 5:
          num3 = 3;
          break;
        case 6:
        case 7:
          num3 = 4;
          break;
        default:
          num3 = 4;
          break;
      }
      val1 = num1;
      switch (num3)
      {
        case 2:
          val2 = (uint) (((int) num1 & (int) byte.MaxValue) << 8) | (num1 & 65280U) >> 8;
          break;
        case 3:
          val2 = (uint) (((int) num1 & 65280) << 8 | (int) ((num1 & 16711680U) >> 8) | (int) num1 & (int) byte.MaxValue);
          break;
        case 4:
          val2 = (uint) (((int) num1 & (int) byte.MaxValue) << 24 | ((int) num1 & 65280) << 8) | (num1 & 16711680U) >> 8 | (num1 & 4278190080U) >> 24;
          break;
        default:
          val2 = num1;
          break;
      }
      if (mode == SearchMode.Text)
        num3 = val3.Length;
      return num3;
    }

    private void txtSearchValue_TextChanged(object sender, EventArgs e)
    {
      if (this.m_bTextMode)
        return;
      if (!this.m_searchForm.SearchText.Text.StartsWith("0x"))
      {
        try
        {
          int.Parse(this.m_searchForm.SearchText.Text);
        }
        catch (OverflowException ex)
        {
          this.m_searchForm.SearchText.Text = this.m_searchForm.SearchText.Text.Substring(0, this.m_searchForm.SearchText.Text.Length - 1);
          this.m_searchForm.SearchText.SelectionStart = this.m_searchForm.SearchText.Text.Length;
        }
        catch (Exception ex)
        {
          this.m_searchForm.SearchText.Text = "";
        }
      }
      if (this.m_searchForm.SearchText.Text.Length <= 0)
        return;
      this.btnFind.Enabled = true;
      this.btnFindPrev.Enabled = true;
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
      this.Search(false, false, this.m_searchForm.GetSearchMode());
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Search(true, false, this.m_searchForm.GetSearchMode());
    }

    private void AdvancedEdit_KeyDown(object sender, KeyEventArgs e)
    {
      if (this.m_bTextMode || e.KeyCode != Keys.G || e.Modifiers != Keys.Control)
        return;
      this.DoGoTo();
    }

    private void DoGoTo()
    {
      Goto @goto = new Goto(this.provider.Length);
      if (@goto.ShowDialog() != DialogResult.OK)
        return;
      if (@goto.AddressLocation < this.provider.Length)
      {
        this.hexBox1.ScrollByteIntoView(@goto.AddressLocation);
        this.hexBox1.Select(@goto.AddressLocation, 1L);
        this.hexBox1.Invalidate();
      }
      else
      {
      }
    }

    private void FillProfiles()
    {
      this.cbProfile.Items.Add((object) "None");
      using (RegistryKey registryKey = Registry.CurrentUser)
      {
        using (RegistryKey subKey = registryKey.CreateSubKey(Util.GetRegistryBase() + "\\Profiles"))
        {
          string str = (string) subKey.GetValue((string) null);
          foreach (string name in subKey.GetValueNames())
          {
            if (!string.IsNullOrEmpty(name))
            {
              int num = this.cbProfile.Items.Add((object) name);
              if ((string) subKey.GetValue(name) == str)
                this.cbProfile.SelectedIndex = num;
            }
          }
        }
      }
      if (this.cbProfile.SelectedIndex >= 0)
        return;
      this.cbProfile.SelectedIndex = 0;
    }

    private void cbSaveFiles_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this.m_bTextMode && this.provider != null && this.provider.Length > 0L)
        this.provider.ApplyChanges();
      container targetGameFolder = this.m_game.GetTargetGameFolder();
      if (!string.IsNullOrEmpty(this.m_cursaveFile) && this.m_saveFilesData.ContainsKey(this.m_cursaveFile))
      {
        file gameFile = this.m_game.GetGameFile(targetGameFolder, this.m_cursaveFile);
        this.m_saveFilesData[this.m_cursaveFile] = gameFile.TextMode != 0 ? (gameFile.TextMode != 2 ? (gameFile.TextMode != 3 ? Encoding.UTF8.GetBytes(this.txtSaveData.Text) : Encoding.Unicode.GetBytes(this.txtSaveData.Text)) : Encoding.ASCII.GetBytes(this.txtSaveData.Text)) : this.provider.Bytes.ToArray();
      }
      this.lstCheats.Items.Clear();
      this.lstValues.Items.Clear();
      this.m_cursaveFile = this.cbSaveFiles.SelectedItem.ToString();
      List<cheat> cheats = this.m_game.GetCheats(this.m_game.LocalSaveFolder.Substring(0, this.m_game.LocalSaveFolder.Length - 4), this.m_cursaveFile);
      if (cheats != null)
      {
        foreach (cheat cheat in cheats)
        {
          if (cheat.id == "-1")
            this.lstCheats.Items.Add((object) cheat.name);
        }
      }
      if (this.lstCheats.Items.Count > 0)
        this.lstCheats.SelectedIndex = 0;
      file gameFile1 = this.m_game.GetGameFile(targetGameFolder, this.m_cursaveFile);
      if (gameFile1 != null && gameFile1.TextMode > 0)
      {
        this.txtSaveData.Visible = true;
        this.hexBox1.Visible = false;
        if (gameFile1.TextMode == 1)
          this.txtSaveData.Text = Encoding.UTF8.GetString(this.m_saveFilesData[this.m_cursaveFile]);
        if (gameFile1.TextMode == 3)
          this.txtSaveData.Text = Encoding.Unicode.GetString(this.m_saveFilesData[this.m_cursaveFile]);
        else
          this.txtSaveData.Text = Encoding.ASCII.GetString(this.m_saveFilesData[this.m_cursaveFile]);
        this.TextMode = true;
        this.txtSaveData.TextChanged += new EventHandler(this.txtSaveData_TextChanged);
        this.lblAddress.Visible = false;
        this.lblOffset.Visible = false;
        this.txtSaveData.HideSelection = false;
      }
      else
      {
        this.hexBox1.Visible = true;
        this.lblAddress.Visible = true;
        this.lblOffset.Visible = true;
        this.txtSaveData.HideSelection = true;
        this.txtSaveData.Visible = false;
        this.provider = new DynamicByteProvider(this.m_saveFilesData[this.m_cursaveFile]);
        this.provider.Changed += new EventHandler<ByteProviderChanged>(this.provider_Changed);
        if (!this.m_undoList.ContainsKey(this.m_cursaveFile))
          this.m_undoList.Add(this.m_cursaveFile, new Stack<ActionItem>());
        if (!this.m_redoList.ContainsKey(this.m_cursaveFile))
          this.m_redoList.Add(this.m_cursaveFile, new Stack<ActionItem>());
        if (this.m_undoList[this.m_cursaveFile].Count == 0)
          this.toolStripButtonUndo.Enabled = false;
        else
          this.toolStripButtonUndo.Enabled = true;
        if (this.m_redoList[this.m_cursaveFile].Count == 0)
          this.toolStripButtonRedo.Enabled = false;
        else
          this.toolStripButtonRedo.Enabled = true;
        this.provider.LengthChanged += new EventHandler(this.provider_LengthChanged);
        this.hexBox1.ByteProvider = (IByteProvider) this.provider;
        this.hexBox1.BytesPerLine = 16;
        this.hexBox1.UseFixedBytesPerLine = true;
        this.hexBox1.VScrollBarVisible = true;
        this.hexBox1.LineInfoVisible = true;
        this.hexBox1.StringViewVisible = true;
        this.hexBox1.SelectionStartChanged += new EventHandler(this.hexBox1_SelectionStartChanged);
        this.hexBox1.SelectionLengthChanged += new EventHandler(this.hexBox1_SelectionLengthChanged);
      }
    }

    private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
    {
      this.lblLengthVal.Text = string.Concat((object) this.hexBox1.SelectionLength);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.lblCheatCodes = new System.Windows.Forms.Label();
            this.lblCheats = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblOffset = new System.Windows.Forms.Label();
            this.lblOffsetValue = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLengthVal = new System.Windows.Forms.Label();
            this.lblLength = new System.Windows.Forms.Label();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.cbSaveFiles = new System.Windows.Forms.ComboBox();
            this.txtSaveData = new System.Windows.Forms.RichTextBox();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cbProfile = new System.Windows.Forms.ComboBox();
            this.btnFindPrev = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.lblAddress = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstCheats = new System.Windows.Forms.ListBox();
            this.lstValues = new System.Windows.Forms.ListBox();
            this.lblGameName = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGoto = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCheatCodes
            // 
            this.lblCheatCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCheatCodes.AutoSize = true;
            this.lblCheatCodes.ForeColor = System.Drawing.Color.White;
            this.lblCheatCodes.Location = new System.Drawing.Point(1026, 225);
            this.lblCheatCodes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCheatCodes.Name = "lblCheatCodes";
            this.lblCheatCodes.Size = new System.Drawing.Size(106, 20);
            this.lblCheatCodes.TabIndex = 4;
            this.lblCheatCodes.Text = "Cheat Codes:";
            // 
            // lblCheats
            // 
            this.lblCheats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCheats.AutoSize = true;
            this.lblCheats.ForeColor = System.Drawing.Color.White;
            this.lblCheats.Location = new System.Drawing.Point(1026, 66);
            this.lblCheats.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCheats.Name = "lblCheats";
            this.lblCheats.Size = new System.Drawing.Size(64, 20);
            this.lblCheats.TabIndex = 5;
            this.lblCheats.Text = "Cheats:";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(128)))), ((int)(((byte)(31)))));
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(1088, 488);
            this.btnApply.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnApply.MinimumSize = new System.Drawing.Size(86, 35);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(86, 35);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply && Download";
            this.btnApply.UseVisualStyleBackColor = false;
            // 
            // lblOffset
            // 
            this.lblOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOffset.AutoSize = true;
            this.lblOffset.ForeColor = System.Drawing.Color.White;
            this.lblOffset.Location = new System.Drawing.Point(720, 495);
            this.lblOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(57, 20);
            this.lblOffset.TabIndex = 8;
            this.lblOffset.Text = "Offset:";
            // 
            // lblOffsetValue
            // 
            this.lblOffsetValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOffsetValue.AutoSize = true;
            this.lblOffsetValue.ForeColor = System.Drawing.Color.White;
            this.lblOffsetValue.Location = new System.Drawing.Point(777, 495);
            this.lblOffsetValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOffsetValue.Name = "lblOffsetValue";
            this.lblOffsetValue.Size = new System.Drawing.Size(0, 20);
            this.lblOffsetValue.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.panel1.Controls.Add(this.lblLengthVal);
            this.panel1.Controls.Add(this.lblLength);
            this.panel1.Controls.Add(this.lblCurrentFile);
            this.panel1.Controls.Add(this.cbSaveFiles);
            this.panel1.Controls.Add(this.txtSaveData);
            this.panel1.Controls.Add(this.lblProfile);
            this.panel1.Controls.Add(this.cbProfile);
            this.panel1.Controls.Add(this.btnFindPrev);
            this.panel1.Controls.Add(this.btnFind);
            this.panel1.Controls.Add(this.lblAddress);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lstCheats);
            this.panel1.Controls.Add(this.lstValues);
            this.panel1.Controls.Add(this.lblGameName);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.lblOffsetValue);
            this.panel1.Controls.Add(this.lblOffset);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.lblCheats);
            this.panel1.Controls.Add(this.lblCheatCodes);
            this.panel1.Controls.Add(this.hexBox1);
            this.panel1.Location = new System.Drawing.Point(15, 17);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1284, 535);
            this.panel1.TabIndex = 10;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lblLengthVal
            // 
            this.lblLengthVal.AutoSize = true;
            this.lblLengthVal.BackColor = System.Drawing.Color.Transparent;
            this.lblLengthVal.ForeColor = System.Drawing.Color.White;
            this.lblLengthVal.Location = new System.Drawing.Point(946, 495);
            this.lblLengthVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLengthVal.Name = "lblLengthVal";
            this.lblLengthVal.Size = new System.Drawing.Size(0, 20);
            this.lblLengthVal.TabIndex = 29;
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLength.AutoSize = true;
            this.lblLength.BackColor = System.Drawing.Color.Transparent;
            this.lblLength.ForeColor = System.Drawing.Color.White;
            this.lblLength.Location = new System.Drawing.Point(882, 495);
            this.lblLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(63, 20);
            this.lblLength.TabIndex = 27;
            this.lblLength.Text = "Length:";
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentFile.ForeColor = System.Drawing.Color.White;
            this.lblCurrentFile.Location = new System.Drawing.Point(20, 38);
            this.lblCurrentFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(90, 20);
            this.lblCurrentFile.TabIndex = 26;
            this.lblCurrentFile.Text = "Current file:";
            // 
            // cbSaveFiles
            // 
            this.cbSaveFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSaveFiles.FormattingEnabled = true;
            this.cbSaveFiles.Location = new System.Drawing.Point(141, 34);
            this.cbSaveFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbSaveFiles.Name = "cbSaveFiles";
            this.cbSaveFiles.Size = new System.Drawing.Size(180, 28);
            this.cbSaveFiles.Sorted = true;
            this.cbSaveFiles.TabIndex = 25;
            // 
            // txtSaveData
            // 
            this.txtSaveData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaveData.Location = new System.Drawing.Point(15, 97);
            this.txtSaveData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSaveData.Name = "txtSaveData";
            this.txtSaveData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtSaveData.Size = new System.Drawing.Size(997, 373);
            this.txtSaveData.TabIndex = 24;
            this.txtSaveData.Text = "";
            this.txtSaveData.Visible = false;
            // 
            // lblProfile
            // 
            this.lblProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProfile.AutoSize = true;
            this.lblProfile.ForeColor = System.Drawing.Color.White;
            this.lblProfile.Location = new System.Drawing.Point(460, 494);
            this.lblProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(57, 20);
            this.lblProfile.TabIndex = 23;
            this.lblProfile.Text = "Profile:";
            // 
            // cbProfile
            // 
            this.cbProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfile.FormattingEnabled = true;
            this.cbProfile.Location = new System.Drawing.Point(520, 488);
            this.cbProfile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbProfile.Name = "cbProfile";
            this.cbProfile.Size = new System.Drawing.Size(166, 28);
            this.cbProfile.TabIndex = 22;
            // 
            // btnFindPrev
            // 
            this.btnFindPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFindPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(128)))), ((int)(((byte)(31)))));
            this.btnFindPrev.ForeColor = System.Drawing.Color.White;
            this.btnFindPrev.Location = new System.Drawing.Point(332, 486);
            this.btnFindPrev.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFindPrev.Name = "btnFindPrev";
            this.btnFindPrev.Size = new System.Drawing.Size(122, 35);
            this.btnFindPrev.TabIndex = 21;
            this.btnFindPrev.Text = "Find Previous";
            this.btnFindPrev.UseVisualStyleBackColor = false;
            this.btnFindPrev.Visible = false;
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(128)))), ((int)(((byte)(31)))));
            this.btnFind.ForeColor = System.Drawing.Color.White;
            this.btnFind.Location = new System.Drawing.Point(228, 486);
            this.btnFind.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(94, 35);
            this.btnFind.TabIndex = 20;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Visible = false;
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress.ForeColor = System.Drawing.Color.White;
            this.lblAddress.Location = new System.Drawing.Point(21, 69);
            this.lblAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(962, 23);
            this.lblAddress.TabIndex = 17;
            this.lblAddress.Text = "Address      Data (Hex)                                                    Data (" +
    "ASCII)";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 494);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "Search";
            this.label1.Visible = false;
            // 
            // lstCheats
            // 
            this.lstCheats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCheats.FormattingEnabled = true;
            this.lstCheats.IntegralHeight = false;
            this.lstCheats.ItemHeight = 20;
            this.lstCheats.Location = new System.Drawing.Point(1026, 97);
            this.lstCheats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstCheats.Name = "lstCheats";
            this.lstCheats.Size = new System.Drawing.Size(238, 112);
            this.lstCheats.TabIndex = 14;
            // 
            // lstValues
            // 
            this.lstValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lstValues.FormattingEnabled = true;
            this.lstValues.ItemHeight = 20;
            this.lstValues.Location = new System.Drawing.Point(1026, 249);
            this.lstValues.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstValues.MultiColumn = true;
            this.lstValues.Name = "lstValues";
            this.lstValues.Size = new System.Drawing.Size(238, 224);
            this.lstValues.TabIndex = 13;
            // 
            // lblGameName
            // 
            this.lblGameName.AutoSize = true;
            this.lblGameName.ForeColor = System.Drawing.Color.White;
            this.lblGameName.Location = new System.Drawing.Point(20, 6);
            this.lblGameName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameName.Name = "lblGameName";
            this.lblGameName.Size = new System.Drawing.Size(40, 20);
            this.lblGameName.TabIndex = 12;
            this.lblGameName.Text = "Test";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(128)))), ((int)(((byte)(31)))));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1180, 488);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 35);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // hexBox1
            // 
            this.hexBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.HScrollBarVisible = false;
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.Location = new System.Drawing.Point(20, 97);
            this.hexBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(999, 375);
            this.hexBox1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSearch,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo,
            this.toolStripButtonGoto,
            this.toolStripButtonExport});
            this.toolStrip1.Location = new System.Drawing.Point(1059, 20);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.Size = new System.Drawing.Size(235, 28);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(34, 23);
            this.toolStripButtonSearch.Text = "Search";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click_1);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(34, 23);
            this.toolStripButtonUndo.Text = "Undo";
            // 
            // toolStripButtonRedo
            // 
            this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRedo.Name = "toolStripButtonRedo";
            this.toolStripButtonRedo.Size = new System.Drawing.Size(34, 23);
            this.toolStripButtonRedo.Text = "Redo";
            // 
            // toolStripButtonGoto
            // 
            this.toolStripButtonGoto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGoto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGoto.Name = "toolStripButtonGoto";
            this.toolStripButtonGoto.Size = new System.Drawing.Size(34, 23);
            this.toolStripButtonGoto.Text = "Go to location";
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(34, 23);
            this.toolStripButtonExport.Text = "Export to file";
            // 
            // AdvancedEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1314, 568);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1273, 527);
            this.Name = "AdvancedEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Edit";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButtonSearch_Click_1(object sender, EventArgs e)
        {

        }
    }
}
