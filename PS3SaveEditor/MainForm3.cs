using CSUST.Data;
using CustomControls;
using Ionic.Zip;
using Microsoft.Win32;
using PS3SaveEditor.Resources;
using Rss;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class MainForm3 : Form
  {
    public static string USER_CHEATS_FILE = "swusercheats.xml";
    private Dictionary<string, List<game>> m_dictLocalSaves = new Dictionary<string, List<game>>();
    private Dictionary<string, List<game>> m_dictAllLocalSaves = new Dictionary<string, List<game>>();
    internal const int WM_DEVICECHANGE = 537;
    public const int WM_SYSCOMMAND = 274;
    public const int MF_SEPARATOR = 2048;
    public const int MF_BYPOSITION = 1024;
    public const int MF_STRING = 0;
    public const int IDM_ABOUT = 1000;
    private const string UNREGISTER_UUID = "{{\"action\":\"UNREGISTER_UUID\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}";
    private const string DESTROY_SESSION = "{{\"action\":\"DESTROY_SESSION\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}";
    private const string SESSION_INIT_URL = "{{\"action\":\"START_SESSION\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}";
    private const string PSNID_INFO = "{{\"action\":\"PSNID_INFO\",\"userid\":\"{0}\"}}";
    private const string SESSION_CLOSAL = "{0}/?q=software_auth2/sessionclose&sessionid={1}";
    private const int INTERNAL_VERION_MAJOR = 1;
    private const int INTERNAL_VERION_MINOR = 0;
    private string m_expandedGame;
    private string m_expandedGameResign;
    private Dictionary<int, string> RegionMap;
    private string m_hash;
    private CustomVScrollbar verticalScroller;
    private CustomHScrollbar horizontalScroller;
    private MainForm3.ClearDrivesDelegate ClearDrivesFunc;
    private MainForm3.AddItemDelegate AddItemFunc;
    private MainForm3.GetTrafficDelegate GetTrafficFunc;
    private List<game> m_games;
    private rblsit m_rblist;
    private bool m_bSerialChecked;
    private bool m_sessionInited;
    private AutoResetEvent evt;
    private AutoResetEvent evt2;
    private Dictionary<string, object> m_psnIDs;
    private int m_psn_quota;
    private int m_psn_remaining;
    private IContainer components;
    private DataGridViewTextBoxColumn _Name;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem simpleToolStripMenuItem;
    private ToolStripMenuItem advancedToolStripMenuItem;
    private Button btnHome;
    private Button btnHelp;
    private Button btnOptions;
    private Panel pnlHome;
    private ToolStripMenuItem restoreFromBackupToolStripMenuItem;
    private Panel panel1;
    private ComboBox cbDrives;
    private ToolStripMenuItem deleteSaveToolStripMenuItem;
    private Button btnGamesInServer;
    private Panel pnlNoSaves;
    private Label lblNoSaves;
    private Button btnOpenFolder;
    private Label lblBackup;
    private Button btnBrowse;
    private TextBox txtBackupLocation;
    private CheckBox chkBackup;
    private Button btnApply;
    private Label lblRSSSection;
    private Button btnRss;
    private Label lblDeactivate;
    private Button btnDeactivate;
    private Panel pnlBackup;
    private DataGridViewTextBoxColumn Multi;
    private Label lblManageProfiles;
    private Button btnManageProfiles;
    private ToolStripMenuItem registerPSNIDToolStripMenuItem;
    private ToolStripMenuItem resignToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
    private CustomGroupBox gbBackupLocation;
    private CustomGroupBox groupBox1;
    private CustomGroupBox groupBox2;
    private CustomGroupBox gbManageProfile;
    private CustomGroupBox gbProfiles;
    private ComboBox cbLanguage;
    private Label lblLanguage;
    private Panel tabPageGames;
    private CheckBox chkShowAll;
    private CustomDataGridView dgServerGames;
    private DataGridViewTextBoxColumn Choose;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private Panel tabPageResign;
    private CustomDataGridView dgResign;
    private Button btnResign;
    private Button btnCheats;
    private Button btnImport;
    private DataGridViewTextBoxColumn _Head;
    private DataGridViewTextBoxColumn GameID;
    private DataGridViewTextBoxColumn PSNID;
    private DataGridViewTextBoxColumn SysVer;
    private DataGridViewTextBoxColumn Addded;
    private ContextMenuStrip contextMenuStrip2;
        private PictureBox pictureBox1;
        private DataGridViewTextBoxColumn JuegoID;
        private DataGridViewTextBoxColumn Añadido;
        private ToolStripMenuItem resignToolStripMenuItem1;

    public MainForm3()
    {
      this.m_games = new List<game>();
      this.InitializeComponent();
      this.RegionMap = new Dictionary<int, string>();
      this.ClearDrivesFunc = new MainForm3.ClearDrivesDelegate(this.ClearDrives);
      this.AddItemFunc = new MainForm3.AddItemDelegate(this.AddItem);
      this.chkShowAll.CheckedChanged += new EventHandler(this.chkShowAll_CheckedChanged);
      this.chkShowAll.EnabledChanged += new EventHandler(this.chkShowAll_EnabledChanged);
      this.ResizeBegin += (EventHandler) ((s, e) => this.SuspendLayout());
      this.ResizeEnd += (EventHandler) ((s, e) =>
      {
        this.ResumeLayout(true);
        this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
        this.Invalidate(true);
      });
      this.SizeChanged += (EventHandler) ((s, e) =>
      {
        if (this.WindowState != FormWindowState.Maximized)
          return;
        this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
        this.Invalidate(true);
      });
      this.txtBackupLocation.ReadOnly = true;
      this.dgServerGames.Columns[0].ReadOnly = true;
      this.MinimumSize = this.Size;
      this.dgServerGames.CellClick += new DataGridViewCellEventHandler(this.dgServerGames_CellClick);
      this.dgServerGames.SelectionChanged += new EventHandler(this.dgServerGames_SelectionChanged);
      this.dgServerGames.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.btnGamesInServer.Visible = false;
      this.btnRss.BackColor = SystemColors.ButtonFace;
      this.btnOpenFolder.BackColor = SystemColors.ButtonFace;
      this.btnBrowse.BackColor = SystemColors.ButtonFace;
      this.btnDeactivate.BackColor = SystemColors.ButtonFace;
      this.btnManageProfiles.BackColor = SystemColors.ButtonFace;
      this.btnApply.BackColor = SystemColors.ButtonFace;
      this.btnRss.ForeColor = System.Drawing.Color.Black;
      this.btnOpenFolder.ForeColor = System.Drawing.Color.Black;
      this.btnBrowse.ForeColor = System.Drawing.Color.Black;
      this.btnDeactivate.ForeColor = System.Drawing.Color.Black;
      this.btnManageProfiles.ForeColor = System.Drawing.Color.Black;
      this.btnApply.ForeColor = System.Drawing.Color.Black;
      this.btnApply.ForeColor = System.Drawing.Color.Black;
      this.tabPageGames.BackColor = this.tabPageGames.BackColor = this.tabPageResign.BackColor = this.pnlBackup.BackColor = this.pnlHome.BackColor = this.pnlHome.BackColor = this.pnlNoSaves.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.gbBackupLocation.BackColor = this.gbManageProfile.BackColor = this.groupBox1.BackColor = this.groupBox2.BackColor = System.Drawing.Color.Transparent;
      this.chkShowAll.BackColor = System.Drawing.Color.FromArgb(0, 204, 204, 204);
      this.chkShowAll.ForeColor = System.Drawing.Color.White;
      this.registerPSNIDToolStripMenuItem.Visible = false;
      this.resignToolStripMenuItem.Visible = false;
      this.toolStripSeparator1.Visible = false;
      this.CenterToScreen();
      this.SetLabels();
      Util.SetForegroundWindow(this.Handle);
      this.cbDrives.SelectedIndexChanged += new EventHandler(this.cbDrives_SelectedIndexChanged);
      this.dgServerGames.CellMouseDown += new DataGridViewCellMouseEventHandler(this.dgServerGames_CellMouseDown);
      this.dgServerGames.CellDoubleClick += new DataGridViewCellEventHandler(this.dgServerGames_CellDoubleClick);
      this.dgServerGames.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(this.dgServerGames_ColumnHeaderMouseClick);
      this.dgServerGames.ShowCellToolTips = true;
      string[] directories = Directory.GetDirectories(Path.GetDirectoryName(Application.ExecutablePath));
      string registryValue = Util.GetRegistryValue("Language");
      this.cbLanguage.DisplayMember = "NativeName";
      this.cbLanguage.ValueMember = "Name";
      List<CultureInfo> list = new List<CultureInfo>();
      list.Add(CultureInfo.CreateSpecificCulture("en"));
      this.cbLanguage.SelectedValueChanged += new EventHandler(this.cbLanguage_SelectedIndexChanged);
     foreach (string path in directories)
      {
        try
        {
          CultureInfo specificCulture = CultureInfo.CreateSpecificCulture(Path.GetFileNameWithoutExtension(path));
          list.Add(specificCulture);
        }
        catch
        {
        }
      }
      this.cbLanguage.DataSource = (object) list;
      if (registryValue != null)
        this.cbLanguage.SelectedValue = (object) registryValue;
      else
        this.cbLanguage.SelectedIndex = 0;
      this.lblLanguage.Visible = false;
      this.cbLanguage.Visible = false;
      this.dgResign.CellDoubleClick += new DataGridViewCellEventHandler(this.dgResign_CellDoubleClick);
      this.cbDrives.DrawMode = DrawMode.OwnerDrawFixed;
     this.cbDrives.DrawItem += new DrawItemEventHandler(this.cbDrives_DrawItem);
      this.FillDrives();
      this.Load += new EventHandler(this.MainForm_Load);
      this.btnHome.ChangeUICues += new UICuesEventHandler(this.btnHome_ChangeUICues);
      this.dgServerGames.BackgroundColor = System.Drawing.Color.White;
      this.dgResign.BackgroundColor = System.Drawing.Color.White;
      this.dgServerGames.ScrollBars = ScrollBars.Both;
      this.dgResign.ScrollBars = ScrollBars.Both;
      this.dgResign.SortCompare += new DataGridViewSortCompareEventHandler(this.dgResign_SortCompare);
      this.dgResign.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dgResign.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.btnCheats.Click += new EventHandler(this.btnCheats_Click);
      this.btnResign.Click += new EventHandler(this.btnResign_Click);
      this.btnImport.Visible = this.tabPageResign.Visible = false;
      this.tabPageGames.BackColor = this.tabPageResign.BackColor = this.pnlHome.BackColor = System.Drawing.Color.Transparent;
      this.tabPageGames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tabPageResign.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.dgResign.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.btnImport.Click += new EventHandler(this.btnImport_Click);
      this.dgServerGames.Columns[7].SortMode = DataGridViewColumnSortMode.Automatic;
      this.dgServerGames.Columns[7].Visible = false;
      this.resignToolStripMenuItem1.Click += new EventHandler(this.resignToolStripMenuItem1_Click);
      this.contextMenuStrip2.Opening += new CancelEventHandler(this.contextMenuStrip2_Opening);
      this.dgServerGames.ColumnWidthChanged += new DataGridViewColumnEventHandler(this.dgServerGames_ColumnWidthChanged);
    }

    private void dgServerGames_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
    }

    private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
    {
      if (this.dgResign.SelectedRows.Count != 1)
      {
        e.Cancel = true;
      }
      else
      {
        if (this.dgResign.SelectedRows[0].Tag is game && this.dgResign.SelectedRows[0].Cells[1].Tag != "U")
          return;
        e.Cancel = true;
      }
    }

    private void resignToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      if (this.dgResign.SelectedRows.Count != 1)
        return;
      this.DoResign(this.dgResign.SelectedRows[0].Index);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Zip Files|*.zip";
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      try
      {
        ZipFile zipFile = new ZipFile(openFileDialog.FileName);
        IEnumerator<ZipEntry> enumerator1 = zipFile.GetEnumerator();
        Dictionary<ZipEntry, ZipEntry> entries = new Dictionary<ZipEntry, ZipEntry>();
        while (enumerator1.MoveNext())
        {
          ZipEntry current = enumerator1.Current;
          string[] strArray = current.FileName.Split('/');
          if (!current.IsDirectory && current.UncompressedSize > 2048L && (strArray.Length > 1 && strArray[strArray.Length - 2].StartsWith("CUSA")) && zipFile.EntryFileNames.Contains(current.FileName + ".bin"))
          {
            IEnumerator<ZipEntry> enumerator2 = zipFile.SelectEntries(Path.GetFileName(current.FileName) + ".bin").GetEnumerator();
            if (enumerator2 != null && enumerator2.MoveNext())
            {
              string str = strArray[strArray.Length - 2];
              if (this.IsValidForResign(new game()
              {
                id = str,
                containers = new containers()
                {
                  _containers = new List<container>()
                  {
                    new container()
                    {
                      pfs = strArray[strArray.Length - 1]
                    }
                  }
                }
              }))
                entries.Add(current, enumerator2.Current);
            }
          }
        }
        if (entries.Count > 0)
        {
          if (new Import(this.m_games, entries, zipFile, this.m_psnIDs, this.cbDrives.SelectedItem as string).ShowDialog((IWin32Window) this) != DialogResult.OK)
            return;
          this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void btnResign_Click(object sender, EventArgs e)
    {
      if (this.tabPageResign.Visible)
        return;
      this.chkShowAll.Visible = this.tabPageGames.Visible = false;
      this.btnImport.Visible = this.tabPageResign.Visible = true;
      this.btnResign.BackColor = System.Drawing.Color.White;
      this.btnCheats.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
    }

    private void btnCheats_Click(object sender, EventArgs e)
    {
      if (this.tabPageGames.Visible)
        return;
      this.chkShowAll.Visible = this.tabPageGames.Visible = true;
      this.btnImport.Visible = this.tabPageResign.Visible = false;
      this.btnCheats.BackColor = System.Drawing.Color.White;
      this.btnResign.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
    }

    private void dgResign_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
    {
      if (e.Column.Index == 1)
      {
        string str1 = e.CellValue1 as string;
        string str2 = e.CellValue2 as string;
        if (str1.IndexOf("    ") >= 0)
        {
          game game = this.dgResign.Rows[e.RowIndex1].Tag as game;
          str1 = string.IsNullOrEmpty(game.name) ? game.id : game.name + " (" + game.id + ")";
        }
        if (str2.IndexOf("    ") >= 0)
        {
          game game = this.dgResign.Rows[e.RowIndex2].Tag as game;
          str2 = string.IsNullOrEmpty(game.name) ? game.id : game.name + " (" + game.id + ")";
        }
        string[] strArray1 = str1.Split(new string[1]
        {
          " ("
        }, StringSplitOptions.None);
        string[] strArray2 = str2.Split(new string[1]
        {
          " ("
        }, StringSplitOptions.None);
        if (str1 == str2)
        {
          if ((e.CellValue1 as string).IndexOf("    ") >= 0 && (e.CellValue2 as string).IndexOf("    ") >= 0)
          {
            e.SortResult = (e.CellValue1 as string).CompareTo(e.CellValue2 as string);
          }
          else
          {
            if ((e.CellValue1 as string).IndexOf("    ") >= 0)
              e.SortResult = this.dgResign.Columns[1].HeaderCell.SortGlyphDirection == SortOrder.Ascending ? 1 : -1;
            if ((e.CellValue2 as string).IndexOf("    ") >= 0)
              e.SortResult = this.dgResign.Columns[1].HeaderCell.SortGlyphDirection == SortOrder.Ascending ? -1 : 1;
          }
          e.Handled = true;
        }
        else
        {
          if (strArray1.Length >= 2 && strArray2.Length >= 2)
            e.SortResult = !(strArray1[0] == strArray2[0]) ? strArray1[0].CompareTo(strArray2[0]) : strArray1[1].CompareTo(strArray2[1]);
          else if (strArray1.Length >= 2 && strArray2.Length == 1)
            e.SortResult = strArray1[0].CompareTo("ZZZZ");
          else if (strArray2.Length >= 2 && strArray1.Length == 1)
            e.SortResult = "ZZZZ".CompareTo(strArray2[0]);
          else if (strArray1.Length == 1 && strArray2.Length == 1)
            e.SortResult = strArray1[0].CompareTo(strArray2[0]);
          e.Handled = true;
        }
      }
      else
        e.Handled = false;
    }

    private void dgResign_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || this.dgResign.SelectedCells.Count == 0 || this.dgResign.SelectedCells[0].RowIndex < 0)
        return;
      int rowIndex = this.dgResign.SelectedCells[0].RowIndex;
      object obj = this.dgResign.Rows[rowIndex].Cells[1].Value;
      string toolTipText = this.dgResign.Rows[rowIndex].Cells[1].ToolTipText;
      if (!(this.dgResign.Rows[rowIndex].Tag is game))
      {
        if (!(this.dgResign.Rows[this.dgResign.SelectedCells[0].RowIndex].Tag is List<game>))
        {
          int num = (int) MessageBox.Show(toolTipText);
        }
        else
        {
          int scrollingRowIndex = this.dgResign.FirstDisplayedScrollingRowIndex;
          bool bSortedAsc = this.dgResign.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending;
          int num = e.RowIndex;
          this.FillResignSaves(this.dgResign.Rows[this.dgResign.SelectedCells[0].RowIndex].Cells[1].Value as string, bSortedAsc);
          if (this.m_expandedGameResign != null)
          {
            foreach (DataGridViewRow dataGridViewRow in (IEnumerable) this.dgResign.Rows)
            {
              if (dataGridViewRow.Cells[1].Value as string == this.m_expandedGameResign)
              {
                num = dataGridViewRow.Index;
                break;
              }
            }
          }
          if (this.dgResign.Rows.Count > e.RowIndex + 1)
          {
            this.dgResign.Rows[num + 1].Selected = true;
            this.dgResign.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
          }
          else
          {
            try
            {
              this.dgResign.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      else
      {
        int scrollingRowIndex = this.dgResign.FirstDisplayedScrollingRowIndex;
        if (this.dgResign.Rows[rowIndex].Cells[1].Tag == "U")
          return;
        this.DoResign(rowIndex);
        try
        {
          this.dgResign.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
        }
        catch (Exception ex)
        {
        }
      }
    }

    private void DoResign(int index)
    {
      game game = this.dgResign.Rows[index].Tag as game;
      if (Util.GetRegistryValue("NoResignMessage") == null)
      {
        int num1 = (int) new ResignInfo().ShowDialog((IWin32Window) this);
      }
      System.IO.File.ReadAllBytes(game.LocalSaveFolder);
      {
        ChooseProfile chooseProfile = new ChooseProfile(this.m_psnIDs, game.PSN_ID);
        if (chooseProfile.ShowDialog((IWin32Window) this) == DialogResult.OK)
        {
          if (System.IO.File.Exists(game.LocalSaveFolder.Replace(game.PSN_ID, chooseProfile.SelectedAccount)))
            return;
          if (new ResignFilesUplaoder(game, Path.GetDirectoryName(game.LocalSaveFolder), chooseProfile.SelectedAccount, new List<string>()).ShowDialog((IWin32Window) this) == DialogResult.OK)
          {
            ResignMessage resignMessage = new ResignMessage();
            int num3 = (int) resignMessage.ShowDialog((IWin32Window) this);
            if (resignMessage.DeleteExisting)
            {
              System.IO.File.Delete(game.LocalSaveFolder);
              System.IO.File.Delete(game.LocalSaveFolder.Substring(0, game.LocalSaveFolder.Length - 4));
              string directoryName = Path.GetDirectoryName(game.LocalSaveFolder);
              if (Directory.GetFiles(directoryName).Length == 0)
                Directory.Delete(directoryName);
            }
            this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
          }
        }
        this.m_expandedGameResign = (string) null;
      }
    }

    private void btnHome_ChangeUICues(object sender, UICuesEventArgs e)
    {
      if (!e.ChangeFocus)
        return;
      this.btnHome.Focus();
    }

    private void chkShowAll_EnabledChanged(object sender, EventArgs e)
    {
      if (this.chkShowAll.Enabled)
      {
        this.chkShowAll.ForeColor = System.Drawing.Color.White;
        this.chkShowAll.FlatStyle = FlatStyle.Standard;
      }
      else
      {
        this.chkShowAll.ForeColor = System.Drawing.Color.FromArgb(190, 190, 190);
        this.chkShowAll.FlatStyle = FlatStyle.Flat;
      }
    }

    private void cbDrives_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (this.cbDrives.SelectedIndex < 0)
        return;
      if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
      {
        e.Graphics.FillRectangle((Brush) new SolidBrush(System.Drawing.Color.FromArgb(0, 175, (int) byte.MaxValue)), e.Bounds);
        e.Graphics.DrawString(this.cbDrives.Items[e.Index].ToString(), e.Font, Brushes.White, (PointF) new Point(e.Bounds.X, e.Bounds.Y));
      }
      else
      {
        e.Graphics.FillRectangle(Brushes.White, e.Bounds);
        e.Graphics.DrawString(this.cbDrives.Items[e.Index].ToString(), e.Font, Brushes.Black, (PointF) new Point(e.Bounds.X, e.Bounds.Y));
      }
    }

    private void dgServerGames_SelectionChanged(object sender, EventArgs e)
    {
      if (this.dgServerGames.SelectedRows == null || this.dgServerGames.SelectedRows.Count <= 0)
        return;
      int index = this.dgServerGames.SelectedRows[0].Index;
      if (this.chkShowAll.Checked)
        this.dgServerGames.CurrentCell = this.dgServerGames.SelectedRows[0].Cells[1];
      else
        this.dgServerGames.CurrentCell = this.dgServerGames.SelectedRows[0].Cells[0];
    }

    private void MainForm_Resize(object sender, EventArgs e)
    {
      this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
        return;
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, System.Drawing.Color.FromArgb(0, 138, 213), System.Drawing.Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void dgServerGames_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
    {
      if (e.Column.Index == 1)
      {
        if (this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection == SortOrder.Descending)
        {
          e.SortResult = this.dgServerGames.Rows[e.RowIndex1].Cells[0].Tag.ToString().CompareTo(this.dgServerGames.Rows[e.RowIndex2].Cells[0].Tag.ToString());
          if (e.SortResult == 0)
          {
            if (this.dgServerGames.Rows[e.RowIndex1].Cells[1].Value.ToString().StartsWith("    "))
              e.SortResult = -1;
            if (this.dgServerGames.Rows[e.RowIndex2].Cells[1].Value.ToString().StartsWith("    "))
              e.SortResult = 1;
          }
        }
        else
        {
          e.SortResult = this.dgServerGames.Rows[e.RowIndex1].Cells[0].Tag.ToString().CompareTo(this.dgServerGames.Rows[e.RowIndex2].Cells[0].Tag.ToString());
          e.SortResult = this.dgServerGames.Rows[e.RowIndex1].Cells[0].Tag.ToString().CompareTo(this.dgServerGames.Rows[e.RowIndex2].Cells[0].Tag.ToString());
          if (e.SortResult == 0)
          {
            if (this.dgServerGames.Rows[e.RowIndex1].Cells[1].Value.ToString().StartsWith("    "))
              e.SortResult = 1;
            if (this.dgServerGames.Rows[e.RowIndex2].Cells[1].Value.ToString().StartsWith("    "))
              e.SortResult = -1;
          }
        }
        e.Handled = true;
      }
      else
        e.Handled = false;
    }

    private void dgServerGames_CellClick(object sender, DataGridViewCellEventArgs e)
    {

        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkShowAll.Checked)
      {
        this.pnlNoSaves.Visible = false;
        this.pnlNoSaves.SendToBack();
        this.dgServerGames.Columns[3].Visible = false;
        this.dgServerGames.Columns[7].Visible = true;
        this.m_games.Sort((Comparison<game>) ((item1, item2) => item2.acts.CompareTo(item1.acts)));
        this.ShowAllGames();
      }
      else
      {
        this.dgServerGames.Columns[0].Visible = true;
        this.dgServerGames.Columns[3].Visible = true;
        this.dgServerGames.Columns[7].Visible = false;
        this.m_games.Sort((Comparison<game>) ((item1, item2) => (item1.name + item1.LocalSaveFolder).CompareTo(item2.name + item1.LocalSaveFolder)));
        this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
      }
    }

    private void ShowAllGames()
    {
      this.dgServerGames.Rows.Clear();
      this.dgServerGames.Columns[4].Visible = false;
      int width = this.dgServerGames.Width;
      if (width == 0)
        return;
      this.dgServerGames.Columns[3].Visible = false;
      this.dgServerGames.Columns[0].Visible = false;
      this.dgServerGames.Columns[1].Width = (int) ((double) width * 0.600000023841858);
      this.dgServerGames.Columns[2].Width = (int) ((double) width * 0.170000001788139);
      this.dgServerGames.Columns[4].Width = (int) ((double) width * 0.170000001788139);
      foreach (game game in this.m_games)
      {
        foreach (alias alias1 in game.GetAllAliases(true))
        {
          if (!(game.name == alias1.name) || !(game.id != alias1.id))
          {
            int index = this.dgServerGames.Rows.Add();
            this.dgServerGames.Rows[index].Tag = (object) game;
            this.dgServerGames.Rows[index].Cells[1].Value = (object) alias1.name;
            this.dgServerGames.Rows[index].Cells[2].Value = (object) game.GetCheatCount();
            this.dgServerGames.Rows[index].Cells[7].Value = game.acts != 0 ? (object) new DateTime(1970, 1, 1).AddSeconds((double) game.acts).ToString("yyyy-MM-dd") : (object) "";
            string exregions = "";
            string region1 = Util.GetRegion(this.RegionMap, game.region, exregions);
            List<string> list = new List<string>();
            list.Add(game.id);
            if (game.aliases != null && game.aliases._aliases.Count > 0)
            {
              foreach (alias alias2 in game.aliases._aliases)
              {
                string region2 = Util.GetRegion(this.RegionMap, alias2.region, region1);
                if (region1.IndexOf(region2) < 0)
                  region1 += region2;
                list.Add(alias2.id);
              }
            }
            list.Sort();
            this.dgServerGames.Rows[index].Cells[3].Value = (object) region1;
            this.dgServerGames.Rows[index].Cells[1].ToolTipText = "Supported List: " + string.Join(",", list.ToArray());
          }
        }
      }
    }

    private void dgServerGames_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      int columnIndex = e.ColumnIndex;
      if (this.chkShowAll.Checked && e.ColumnIndex == 2)
        return;
      this.SortGames(e.ColumnIndex, this.dgServerGames.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending);
      if (this.chkShowAll.Checked)
        this.ShowAllGames();
      else
        this.FillLocalSaves((string) null, this.dgServerGames.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending);
    }

    private void dgServerGames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || this.dgServerGames.SelectedCells.Count == 0 || this.dgServerGames.SelectedCells[0].RowIndex < 0)
        return;
      object obj = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].Value;
      string toolTipText = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].ToolTipText;
      if (!(this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag is game))
      {
        if (!(this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag is List<game>))
        {
          int num = (int) MessageBox.Show(toolTipText);
        }
        else
        {
          int scrollingRowIndex = this.dgServerGames.FirstDisplayedScrollingRowIndex;
          this.FillLocalSaves(this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].Value as string, this.dgServerGames.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending);
          if (this.dgServerGames.Rows.Count > e.RowIndex + 1)
          {
            this.dgServerGames.Rows[e.RowIndex + 1].Selected = true;
            this.dgServerGames.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
          }
          else
          {
            this.dgServerGames.Rows[Math.Min(e.RowIndex, this.dgServerGames.Rows.Count - 1)].Selected = true;
            try
            {
              this.dgServerGames.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      else
        this.simpleToolStripMenuItem_Click((object) null, (EventArgs) null);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, int wIDNewItem, string lpNewItem);

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 274)
      {
        if (m.WParam.ToInt32() == 1000)
        {
          int num = (int) new AboutBox1().ShowDialog();
          return;
        }
      }
      else if (m.Msg == 537 && this.m_bSerialChecked)
      {
        if (m.WParam.ToInt32() == 32768)
        {
          if (m.LParam != IntPtr.Zero && (int) ((MainForm3.DEV_BROADCAST_HDR) Marshal.PtrToStructure(m.LParam, typeof (MainForm3.DEV_BROADCAST_HDR))).dbch_DeviceType == 2)
          {
            MainForm3.DEV_BROADCAST_VOLUME devBroadcastVolume = (MainForm3.DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(m.LParam, typeof (MainForm3.DEV_BROADCAST_VOLUME));
            for (int index = 0; index < 26; ++index)
            {
              if (((int) (devBroadcastVolume.dbcv_unitmask >> index) & 1) == 1)
                new Thread(new ParameterizedThreadStart(this.HandleDrive)).Start((object) string.Format("{0}:\\", (object) (char) (65 + index)));
            }
          }
        }
        else if (m.WParam.ToInt32() == 32772 && m.LParam != IntPtr.Zero && (int) ((MainForm3.DEV_BROADCAST_HDR) Marshal.PtrToStructure(m.LParam, typeof (MainForm3.DEV_BROADCAST_HDR))).dbch_DeviceType == 2)
        {
          MainForm3.DEV_BROADCAST_VOLUME devBroadcastVolume = (MainForm3.DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(m.LParam, typeof (MainForm3.DEV_BROADCAST_VOLUME));
          for (int index1 = 0; index1 < 26; ++index1)
          {
            if (((int) (devBroadcastVolume.dbcv_unitmask >> index1) & 1) == 1)
            {
              for (int index2 = 0; index2 < this.cbDrives.Items.Count; ++index2)
              {
                if (this.cbDrives.Items[index2].ToString() == string.Format("{0}:\\", (object) (char) (65 + index1)))
                  this.cbDrives.Items.RemoveAt(index2);
              }
            }
          }
          if (this.cbDrives.Items.Count == 0)
          {
            this.dgResign.Rows.Clear();
            this.chkShowAll.Checked = true;
            this.btnResign.Enabled = this.btnImport.Enabled = this.chkShowAll.Enabled = true;
          }
          else
            this.cbDrives.SelectedIndex = 0;
        }
      }
      if (m.Msg == 274 && (int) m.WParam == 61728)
        this.Invalidate(true);
      base.WndProc(ref m);
    }

    private void HandleDrive(object drive)
    {
      this.cbDrives.Invoke((Delegate) this.AddItemFunc, (object) (string) drive);
    }

    private int InitSession()
    {
            /*  try
              {
                WebClientEx webClientEx = new WebClientEx();
                webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
                string uid = Util.GetUID(false, false);
                if (string.IsNullOrEmpty(uid))
                {
                  RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Util.GetRegistryBase(), false);
                  try
                  {
                    registryKey.DeleteValue("Hash");
                  }
                  catch
                  {
                  }
                  this.Close();
                  return 0;
                }
                Dictionary<string, object> dictionary1 = new JavaScriptSerializer().Deserialize(Encoding.ASCII.GetString(webClientEx.UploadData(Util.GetBaseUrl() + "/ps4auth", Encoding.ASCII.GetBytes(string.Format("{{\"action\":\"START_SESSION\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}", (object) Util.GetUserId(), (object) uid)))), typeof (Dictionary<string, object>)) as Dictionary<string, object>;
                if (dictionary1.ContainsKey("update"))
                {
                  Dictionary<string, object> dictionary2 = dictionary1["update"] as Dictionary<string, object>;
                  foreach (string index in dictionary2.Keys)
                  {
                    string url = (string) dictionary2[index];
                    if (url.IndexOf("msi", StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                      int num = (int) new UpgradeDownloader(url).ShowDialog();
                      this.Close();
                      return 0;
                    }
                  }
                }
                if (dictionary1.ContainsKey("token"))
                {
                  Util.SetAuthToken(dictionary1["token"] as string);
                  new Thread(new ParameterizedThreadStart(this.Pinger)).Start((object) (Convert.ToInt32(dictionary1["expiry_ts"]) - Convert.ToInt32(dictionary1["current_ts"])));
                  new Thread(new ParameterizedThreadStart(this.TrafficPoller)).Start();
                  this.GetPSNIDInfo();
                  this.m_sessionInited = true;
                  return 1;
                }
                if (dictionary1.ContainsKey("code") && (dictionary1["code"].ToString() == "10009" || dictionary1["code"].ToString() == "4071"))
                  return -1;
                Util.DeleteRegistryValue("User");
                this.Close();
                return 0;
              }
              catch (Exception ex)*/
            {
                //if (ex is WebException)
                return 1;
      }
      return 1;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      IntPtr systemMenu = MainForm3.GetSystemMenu(this.Handle, false);
      MainForm3.InsertMenu(systemMenu, 5, 3072, 0, string.Empty);
      MainForm3.InsertMenu(systemMenu, 6, 1024, 1000, "About Save Wizard for PS4 MAX...");
     
      if (!this.CheckSerial())
      {
        //this.Close();
      }
      else
      {
        //this.m_bSerialChecked = true;
        int num1 = this.InitSession();
        if (num1 < 0)
        {
          Util.ChangeServer();
          num1 = this.InitSession();
        }
        if (num1 == 0)
          return;
        if (num1 < 0)
        {
          this.Close();
        }
        else
        {
          GameListDownloader gameListDownloader = new GameListDownloader();
         //if (gameListDownloader.ShowDialog() == DialogResult.OK)
          {
           // int count = this.m_psnIDs.Count;
            try
            {
              this.FillSavesList(gameListDownloader.GameListXml);
            }
            catch (Exception ex)
            {
              this.Close();
              return;
            }
            if (this.cbDrives.Items.Count == 0)
            {
              this.chkShowAll.Checked = false;
              this.btnResign.Enabled = this.btnImport.Enabled = this.chkShowAll.Enabled = true;
            }
            else
            {
              this.PrepareLocalSavesMap();
              this.FillLocalSaves((string) null, true);
              this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            }
            this.btnHome_Click((object) null, (EventArgs) null);
          }
          /*else
            this.Close();*/
        }
      }
    }

    private void TrafficPoller(object ob)
    {
      this.evt2 = new AutoResetEvent(false);
    }

    private void Pinger(object tim)
    {
      int num = (int) tim;
      this.evt = new AutoResetEvent(false);
      string format = "{{\"action\":\"SESSION_REFRESH\",\"userid\":\"{0}\",\"token\":\"{1}\"}}";
      WebClientEx webClientEx = new WebClientEx();
      webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
label_7:
      while (!this.evt.WaitOne((num - 10) * 1000))
      {
        while (true)
        {
          try
          {
            string @string = Encoding.ASCII.GetString(webClientEx.UploadData(Util.GetBaseUrl() + "/ps4auth", Encoding.ASCII.GetBytes(string.Format(format, (object) Util.GetUserId(), (object) Util.GetAuthToken()))));
            if (@string.Contains("ERROR"))
              return;
            Dictionary<string, object> dictionary = scriptSerializer.Deserialize(@string, typeof (Dictionary<string, object>)) as Dictionary<string, object>;
            if (dictionary.ContainsKey("token"))
            {
              Util.SetAuthToken(dictionary["token"] as string);
              goto label_7;
            }
            else
              goto label_7;
          }
          catch (Exception ex)
          {
            Thread.Sleep(3000);
          }
        }
      }
    }

    private void PrepareLocalSavesMap()
    {
      this.m_dictLocalSaves.Clear();
      this.m_dictAllLocalSaves.Clear();
      if (this.cbDrives.SelectedItem == null)
        return;
      string str1 = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str1 + "PS4\\SAVEDATA"))
        return;
      string[] directories = Directory.GetDirectories(str1 + "PS4\\SAVEDATA");
      List<string> list1 = new List<string>();
      foreach (string path1 in directories)
      {
        long result;
        if (long.TryParse(Path.GetFileName(path1), NumberStyles.HexNumber, (IFormatProvider) null, out result))
        {
          foreach (string path2 in Directory.GetDirectories(path1))
            list1.AddRange((IEnumerable<string>) Directory.GetFiles(path2, "*.bin"));
        }
      }
      string[] array = list1.ToArray();
      Array.Sort<string>(array);
      foreach (string str2 in array)
      {
        string saveId;
        int onlineSaveIndex = MainForm3.GetOnlineSaveIndex(this.m_games, str2, out saveId);
        if (onlineSaveIndex >= 0)
        {
          this.dgServerGames.Rows.Add();
          game game = game.Copy(this.m_games[onlineSaveIndex]);
          game.id = saveId;
          game.LocalCheatExists = true;
          game.LocalSaveFolder = str2;
          if (game.GetTargetGameFolder() == null)
            game.LocalCheatExists = false;
          if (!this.m_dictLocalSaves.ContainsKey(game.id))
            this.m_dictLocalSaves.Add(game.id, new List<game>()
            {
              game
            });
          else
            this.m_dictLocalSaves[game.id].Add(game);
          if (!this.m_dictAllLocalSaves.ContainsKey(game.id))
          {
            List<game> list2 = new List<game>();
            this.m_dictAllLocalSaves.Add(game.id, list2);
          }
          this.m_dictAllLocalSaves[game.id].Add(game);
        }
        else
        {
          string path = str2.Substring(0, str2.Length - 4);
          if (System.IO.File.Exists(path))
          {
            string fileName = Path.GetFileName(Path.GetDirectoryName(str2));
            game game = new game()
            {
              name = "",
              id = fileName,
              containers = new containers()
              {
                _containers = new List<container>()
                {
                  new container()
                  {
                    pfs = Path.GetFileName(path).Substring(0, Path.GetFileName(path).Length)
                  }
                }
              },
              LocalSaveFolder = str2
            };
            if (!this.m_dictAllLocalSaves.ContainsKey(game.id))
            {
              List<game> list2 = new List<game>();
              this.m_dictAllLocalSaves.Add(game.id, list2);
            }
            this.m_dictAllLocalSaves[game.id].Add(game);
          }
        }
      }
    }

    private void FillResignSaves(string expandGame, bool bSortedAsc)
    {
      if (this.m_expandedGameResign == expandGame)
      {
        expandGame = (string) null;
        this.m_expandedGameResign = (string) null;
      }
      this.dgResign.Rows.Clear();
      List<string> list1 = new List<string>();
      foreach (string str1 in this.m_dictAllLocalSaves.Keys)
      {
        string id = str1;
        game game1 = this.m_games.Find((Predicate<game>) (a => a.id == id)) ?? this.m_dictAllLocalSaves[id][0];
        foreach (alias alias in game1.GetAllAliases(bSortedAsc))
        {
          string str2 = alias.name + " (" + alias.id + ")";
          string index1 = alias.id;
          if (this.m_dictAllLocalSaves.ContainsKey(alias.id))
          {
            List<game> list2 = this.m_dictAllLocalSaves[index1];
            if (list1.IndexOf(index1) < 0)
            {
              list1.Add(index1);
              int index2 = this.dgResign.Rows.Add();
              this.dgResign.Rows[index2].Cells[1].Value = (object) alias.name;
              if (list2.Count == 0)
              {
                game game2 = list2[0];
                this.dgResign.Rows[index2].Tag = (object) game2;
                container targetGameFolder = game2.GetTargetGameFolder();
                if (targetGameFolder != null)
                  this.dgResign.Rows[index2].Cells[2].Value = (object) targetGameFolder.GetCheatsCount();
                else
                  this.dgResign.Rows[index2].Cells[2].Value = (object) "N/A";
                this.dgResign.Rows[index2].Cells[0].ToolTipText = "";
                this.dgResign.Rows[index2].Cells[0].Tag = (object) index1;
                if (!this.IsValidForResign(game2))
                {
                  this.dgResign.Rows[index2].DefaultCellStyle = new DataGridViewCellStyle()
                  {
                    ForeColor = System.Drawing.Color.Gray
                  };
                  this.dgResign.Rows[index2].Cells[1].Tag = (object) "U";
                }
              }
              else
              {
                DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                this.dgResign.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                {
                  Font = new Font("Arial", 7f)
                });
                this.dgResign.Rows[index2].Cells[0].Value = (object) "►";
                string str3 = this.dgResign.Rows[index2].Cells[1].Value as string;
                this.dgResign.Rows[index2].Cells[1].Value = string.IsNullOrEmpty(str3) ? (object) alias.id : (object) (str3 + " (" + alias.id + ")");
                dataGridViewCellStyle.BackColor = System.Drawing.Color.White;
                this.dgResign.Rows[index2].Cells[0].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgResign.Rows[index2].Cells[1].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgResign.Rows[index2].Cells[2].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgResign.Rows[index2].Tag = (object) list2;
                if (!this.IsValidForResign(game1))
                {
                  this.dgResign.Rows[index2].DefaultCellStyle = new DataGridViewCellStyle()
                  {
                    ForeColor = System.Drawing.Color.Gray
                  };
                  this.dgResign.Rows[index2].Cells[1].Tag = (object) "U";
                }
                if (str2 == expandGame || alias.id == expandGame)
                {
                  this.dgResign.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                  {
                    Font = new Font("Arial", 7f)
                  });
                  this.dgResign.Rows[index2].Cells[0].Value = (object) "▼";
                  this.dgResign.Rows[index2].Cells[0].ToolTipText = "";
                  this.dgResign.Rows[index2].Cells[1].Value = string.IsNullOrEmpty(str3) ? (object) alias.id : (object) (str3 + " (" + alias.id + ")");
                  this.dgResign.Rows[index2].Cells[0].Tag = (object) index1;
                  foreach (game game2 in list2)
                  {
                    container targetGameFolder = game2.GetTargetGameFolder();
                    if (targetGameFolder != null)
                    {
                      int index3 = this.dgResign.Rows.Add();
                      Match match = Regex.Match(Path.GetFileNameWithoutExtension(game2.LocalSaveFolder), targetGameFolder.pfs);
                      if (targetGameFolder.name != null && match.Groups != null && match.Groups.Count > 1)
                        this.dgResign.Rows[index3].Cells[1].Value = (object) ("    " + targetGameFolder.name.Replace("${1}", match.Groups[1].Value));
                      else
                        this.dgResign.Rows[index3].Cells[1].Value = (object) ("    " + (targetGameFolder.name ?? Path.GetFileNameWithoutExtension(game2.LocalSaveFolder)));
                      this.dgResign.Rows[index3].Cells[0].Tag = (object) index1;
                      game2.name = alias.name;
                      this.dgResign.Rows[index3].Tag = (object) game2;
                      this.dgResign.Rows[index3].Cells[1].ToolTipText = Path.GetFileNameWithoutExtension(game2.LocalSaveFolder);
                      string sysVer = MainForm3.GetSysVer(game2.LocalSaveFolder);
                      this.dgResign.Rows[index3].Cells[2].Value = (object) sysVer;
                      if (!this.IsValidForResign(game2))
                      {
                        this.dgResign.Rows[index3].DefaultCellStyle = new DataGridViewCellStyle()
                        {
                          ForeColor = System.Drawing.Color.Gray
                        };
                        this.dgResign.Rows[index3].Cells[1].Tag = (object) "U";
                      }
                    }
                  }
                  this.m_expandedGameResign = expandGame;
                }
              }
            }
          }
        }
      }
      this.dgResign.Sort(this.dgResign.Columns[1], !bSortedAsc ? ListSortDirection.Descending : ListSortDirection.Ascending);
      this.dgResign.ClearSelection();
    }

    internal static string GetSysVer(string binFile)
    {
      return MainForm3.GetSysVer(System.IO.File.ReadAllBytes(binFile));
    }

    internal static string GetSysVer(byte[] buf)
    {
      if (buf.Length <= 8)
        return "?";
      switch (buf[8])
      {
        case (byte) 1:
          return "All";
        case (byte) 2:
          return "4.50+";
        case (byte) 3:
          return "8.00+";
        case (byte) 4:
          return "4.70+";
        default:
          return "";
      }
    }

    private bool IsValidForResign(game item)
    {
      if (this.m_rblist == null)
        return false;
      foreach (rbgame rbgame in this.m_rblist._rbgames)
      {
        if (rbgame.gamecode == item.id)
        {
          if (rbgame.containers == null || rbgame.containers.container == null || rbgame.containers.container.Count == 0)
            return false;
          if (item.LocalSaveFolder != null)
          {
            foreach (string pattern in rbgame.containers.container)
            {
              if (Util.IsMatch(Path.GetFileNameWithoutExtension(item.LocalSaveFolder), pattern))
                return false;
            }
          }
        }
      }
      return true;
    }

    private void FillLocalSaves(string expandGame, bool bSortedAsc)
    {
      if (this.m_expandedGame == expandGame)
      {
        expandGame = (string) null;
        this.m_expandedGame = (string) null;
      }
      this.dgServerGames.Rows.Clear();
      List<string> list1 = new List<string>();
      foreach (game game1 in this.m_games)
      {
        foreach (alias alias in game1.GetAllAliases(bSortedAsc))
        {
          string str = alias.name + " (" + alias.id + ")";
          string index1 = alias.id;
          if (this.m_dictLocalSaves.ContainsKey(alias.id))
          {
            List<game> list2 = this.m_dictLocalSaves[index1];
            if (list1.IndexOf(index1) < 0)
            {
              list1.Add(index1);
              int index2 = this.dgServerGames.Rows.Add();
              this.dgServerGames.Rows[index2].Cells[1].Value = (object) alias.name;
              if (list2.Count == 0)
              {
                game game2 = list2[0];
                this.dgServerGames.Rows[index2].Tag = (object) game2;
                container targetGameFolder = game2.GetTargetGameFolder();
                if (targetGameFolder != null)
                  this.dgServerGames.Rows[index2].Cells[2].Value = (object) targetGameFolder.GetCheatsCount();
                else
                  this.dgServerGames.Rows[index2].Cells[2].Value = (object) "N/A";
                this.dgServerGames.Rows[index2].Cells[0].ToolTipText = "";
                this.dgServerGames.Rows[index2].Cells[0].Tag = (object) index1;
                this.dgServerGames.Rows[index2].Cells[1].ToolTipText = Path.GetFileNameWithoutExtension(game2.LocalSaveFolder);
                this.dgServerGames.Rows[index2].Cells[3].Value = (object) index1;
                this.dgServerGames.Rows[index2].Cells[5].Value = (object) true;
                if (!this.IsValidPSNID(game2.PSN_ID))
                {
                  this.dgServerGames.Rows[index2].DefaultCellStyle = new DataGridViewCellStyle()
                  {
                    ForeColor = System.Drawing.Color.Gray
                  };
                  this.dgServerGames.Rows[index2].Cells[1].Tag = (object) "U";
                }
              }
              else
              {
                DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                this.dgServerGames.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                {
                  Font = new Font("Arial", 7f)
                });
                this.dgServerGames.Rows[index2].Cells[0].Value = (object) "►";
                this.dgServerGames.Rows[index2].Cells[1].Value = (object) string.Concat(new object[4]
                {
                  this.dgServerGames.Rows[index2].Cells[1].Value,
                  (object) " (",
                  (object) alias.id,
                  (object) ")"
                });
                dataGridViewCellStyle.BackColor = System.Drawing.Color.White;
                this.dgServerGames.Rows[index2].Cells[0].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Cells[1].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Cells[2].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Cells[3].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Cells[4].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Cells[7].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgServerGames.Rows[index2].Tag = (object) list2;
                this.dgServerGames.Rows[index2].Cells[5].Value = (object) false;
                if (str == expandGame)
                {
                  this.dgServerGames.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                  {
                    Font = new Font("Arial", 7f)
                  });
                  this.dgServerGames.Rows[index2].Cells[0].Value = (object) "▼";
                  this.dgServerGames.Rows[index2].Cells[0].ToolTipText = "";
                  this.dgServerGames.Rows[index2].Cells[1].Value = (object) (alias.name + " (" + alias.id + ")");
                  this.dgServerGames.Rows[index2].Cells[0].Tag = (object) index1;
                  foreach (game game2 in list2)
                  {
                    container targetGameFolder = game2.GetTargetGameFolder();
                    if (targetGameFolder != null)
                    {
                      int index3 = this.dgServerGames.Rows.Add();
                      Match match = Regex.Match(Path.GetFileNameWithoutExtension(game2.LocalSaveFolder), targetGameFolder.pfs);
                      if (targetGameFolder.name != null && match.Groups != null && match.Groups.Count > 1)
                        this.dgServerGames.Rows[index3].Cells[1].Value = (object) ("    " + targetGameFolder.name.Replace("${1}", match.Groups[1].Value));
                      else
                        this.dgServerGames.Rows[index3].Cells[1].Value = (object) ("    " + (targetGameFolder.name ?? Path.GetFileNameWithoutExtension(game2.LocalSaveFolder)));
                      this.dgServerGames.Rows[index3].Cells[0].Tag = (object) index1;
                      this.dgServerGames.Rows[index3].Tag = (object) game2;
                      if (targetGameFolder != null)
                        this.dgServerGames.Rows[index3].Cells[2].Value = (object) targetGameFolder.GetCheatsCount();
                      else
                        this.dgServerGames.Rows[index3].Cells[2].Value = (object) "N/A";
                      this.dgServerGames.Rows[index3].Cells[1].ToolTipText = Path.GetFileNameWithoutExtension(game2.LocalSaveFolder);
                      this.dgServerGames.Rows[index3].Cells[3].Value = (object) index1;
                      this.dgServerGames.Rows[index3].Cells[5].Value = (object) true;
                      if (!this.IsValidPSNID(game2.PSN_ID))
                      {
                        this.dgServerGames.Rows[index3].DefaultCellStyle = new DataGridViewCellStyle()
                        {
                          ForeColor = System.Drawing.Color.Gray
                        };
                        this.dgServerGames.Rows[index3].Cells[1].Tag = (object) "U";
                      }
                    }
                  }
                  this.m_expandedGame = expandGame;
                }
              }
            }
          }
        }
      }
      this.FillUnavailableGames();
      this.dgServerGames.ClearSelection();
    }
        

    private string GetProfileKey(string sfoPath, Dictionary<string, string> mapProfiles)
    {
      if (System.IO.File.Exists(sfoPath))
      {
        int profileId;
        string str = Convert.ToBase64String(MainForm3.GetParamInfo(sfoPath, out profileId));
        string key1 = profileId.ToString() + ":" + str + ":" + Convert.ToBase64String(Util.GetPSNId(Path.GetDirectoryName(sfoPath)));
        if (mapProfiles.ContainsKey(key1))
          return mapProfiles[key1];
        string key2 = profileId.ToString() + ":" + str;
        if (mapProfiles.ContainsKey(key2))
          return mapProfiles[key2];
      }
      return "";
    }

    private bool CheckSerial()
    {
      if (Util.GetRegistryValue("User") == null)
      {
        if (new SerialValidateGG().ShowDialog((IWin32Window) this) != DialogResult.OK)
          return false;
      }
      else
        this.m_hash = Util.GetRegistryValue("User").ToString();
      return true;
    }

    private void SetLabels()
    {
      this.dgServerGames.Columns[3].Visible = false;
      this.Text = Util.PRODUCT_NAME;
      MainForm3 mainForm3 = this;
      string str = mainForm3.Text;
      mainForm3.Text = "PS4 Chetos Inc Save Special";
    }

    private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      CultureInfo cultureInfo = this.cbLanguage.SelectedItem as CultureInfo;
      if (cultureInfo == null)
        return;
      Util.SetRegistryValue("Languaje", cultureInfo.Name);
      Thread.CurrentThread.CurrentUICulture = cultureInfo;
      this.SetLabels();
    }

    public static void FillLocalCheats(ref game item)
    {
      string str = Util.GetBackupLocation() + (object) Path.DirectorySeparatorChar + MainForm3.USER_CHEATS_FILE;
      if (!System.IO.File.Exists(str))
        return;
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(str);
      for (int index1 = 0; index1 < xmlDocument["usercheats"].ChildNodes.Count; ++index1)
      {
        container targetGameFolder = item.GetTargetGameFolder();
        if (targetGameFolder != null && item.id + targetGameFolder.key == xmlDocument["usercheats"].ChildNodes[index1].Attributes["id"].Value && xmlDocument["usercheats"].ChildNodes[index1].ChildNodes.Count > 0)
        {
          for (int index2 = 0; index2 < xmlDocument["usercheats"].ChildNodes[index1].ChildNodes.Count; ++index2)
          {
            XmlNode xmlNode1 = xmlDocument["usercheats"].ChildNodes[index1].ChildNodes[index2];
            if ((xmlNode1 as XmlElement).Name == "file")
            {
              XmlElement xmlElement = xmlNode1 as XmlElement;
              string attribute = xmlElement.GetAttribute("name");
              file gameFile = item.GetGameFile(targetGameFolder, attribute);
              if (gameFile != null)
                gameFile.ucfilename = attribute;
              for (int index3 = 0; index3 < xmlElement.ChildNodes.Count; ++index3)
              {
                XmlNode xmlNode2 = xmlElement.ChildNodes[index3];
                cheat cheat = new cheat("-1", xmlNode2.Attributes["desc"].Value, xmlNode2.Attributes["comment"].Value);
                for (int index4 = 0; index4 < xmlNode2.ChildNodes.Count; ++index4)
                {
                  string innerText = xmlNode2.ChildNodes[index4].InnerText;
                  if (innerText.Split(' ').Length % 2 == 0)
                    cheat.code = innerText;
                }
                if (gameFile != null)
                  gameFile.Cheats.Add(cheat);
              }
            }
            else
            {
              cheat cheat = new cheat("-1", xmlDocument["usercheats"].ChildNodes[index1].ChildNodes[index2].Attributes["desc"].Value, xmlDocument["usercheats"].ChildNodes[index1].ChildNodes[index2].Attributes["comment"].Value);
              for (int index3 = 0; index3 < xmlDocument["usercheats"].ChildNodes[index1].ChildNodes[index2].ChildNodes.Count; ++index3)
              {
                string innerText = xmlDocument["usercheats"].ChildNodes[index1].ChildNodes[index2].ChildNodes[index3].InnerText;
                if (innerText.Split(' ').Length == 2)
                  cheat.code = innerText;
              }
              if (!string.IsNullOrEmpty(cheat.code) && targetGameFolder != null)
                targetGameFolder.files._files[0].Cheats.Add(cheat);
            }
          }
        }
      }
    }

    private void FillServerGamesList()
    {
      this.dgServerGames.Rows.Clear();
      foreach (game game in this.m_games)
      {
        int index = this.dgServerGames.Rows.Add(new DataGridViewRow());
        this.dgServerGames.Rows[index].Cells[1].Value = (object) game.name;
        this.dgServerGames.Rows[index].Cells[2].Value = (object) game.GetCheatCount();
        this.dgServerGames.Rows[index].Cells[3].Value = (object) game.id;
      }
    }

    private void FillUnavailableGames()
    {
      if (this.cbDrives.SelectedItem == null)
        return;
      string str1 = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str1 + "PS4\\SAVEDATA"))
        return;
      foreach (string str2 in Directory.GetDirectories(str1 + "PS4\\SAVEDATA"))
      {
        string saveId;
        if (MainForm3.GetOnlineSaveIndex(this.m_games, str2, out saveId) == -1)
        {
          string str3 = str2 + (object) Path.DirectorySeparatorChar + "PARAM.SFO";
          if (System.IO.File.Exists(str3))
          {
            int index = this.dgServerGames.Rows.Add();
            System.Drawing.Color lightSlateGray = System.Drawing.Color.LightSlateGray;
            this.dgServerGames.Rows[index].Cells[0].Style.BackColor = lightSlateGray;
            this.dgServerGames.Rows[index].Cells[1].Style.BackColor = lightSlateGray;
            this.dgServerGames.Rows[index].Cells[2].Style.BackColor = lightSlateGray;
            this.dgServerGames.Rows[index].Cells[3].Style.BackColor = lightSlateGray;
            this.dgServerGames.Rows[index].Cells[4].Style.BackColor = lightSlateGray;
            this.dgServerGames.Rows[index].Cells[1].Value = (object) this.GetSaveTitle(str3);
            this.dgServerGames.Rows[index].Cells[3].Value = (object) Path.GetFileName(str2).Substring(0, 9);
            this.dgServerGames.Rows[index].Cells[0].Tag = this.dgServerGames.Rows[index].Cells[3].Value;
            this.dgServerGames.Rows[index].Cells[4].Value = (object) "";
            this.dgServerGames.Rows[index].Tag = (object) str2;
          }
        }
      }
    }

    private void dgServerGames_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.RowIndex < 0 || e.Button != MouseButtons.Right)
        return;
      this.dgServerGames.ClearSelection();
      this.dgServerGames.Rows[e.RowIndex].Selected = true;
    }

    private void SortGames(int sortCol, bool bDesc)
    {
      this.m_games.Sort((Comparison<game>) ((item1, item2) =>
      {
        switch (sortCol)
        {
          case 2:
            return item1.GetCheatCount().CompareTo(item2.GetCheatCount());
          case 3:
            return item1.id.CompareTo(item2.id);
          case 7:
            return item1.acts.CompareTo(item2.acts);
          default:
            return (item1.name + item1.id).CompareTo(item2.name + item2.id);
        }
      }));
      if (!bDesc)
        return;
      this.m_games.Reverse();
    }

    private void ClearDrives()
    {
      this.cbDrives.Items.Clear();
      if (this.cbDrives.Items.Count > 0)
      {
        this.cbDrives.SelectedIndex = 0;
      }
      else
      {
        if (this.m_games.Count <= 0)
          return;
        this.chkShowAll.Checked = true;
        this.chkShowAll.Enabled = false;
      }
    }

    private void AddItem(string item)
    {
      if (item != null)
      {
        int num = this.cbDrives.Items.Add((object) item);
        if (Directory.Exists(item + "PS4\\SAVEDATA") && Directory.GetDirectories(item + "PS4\\SAVEDATA").Length > 0)
        {
          this.pnlNoSaves.Visible = false;
          this.pnlNoSaves.SendToBack();
          if (this.cbDrives.SelectedIndex < 0)
          {
            this.cbDrives.SelectedIndex = num;
          }
          else
          {
            string str = this.cbDrives.SelectedItem as string;
            if (!string.IsNullOrEmpty(str) && (!Directory.Exists(str + "PS4\\SAVEDATA") || Directory.GetDirectories(str + "PS4\\SAVEDATA").Length <= 0))
              this.cbDrives.SelectedIndex = num;
          }
        }
        else if (this.cbDrives.SelectedIndex < 0)
        {
          this.pnlNoSaves.Visible = true;
          this.pnlNoSaves.BringToFront();
          this.cbDrives.SelectedIndex = num;
        }
        if (this.chkShowAll.Enabled)
          return;
        this.btnResign.Enabled = this.btnImport.Enabled = this.chkShowAll.Enabled = true;
        this.chkShowAll.Checked = false;
      }
      else
      {
        if (this.m_games.Count <= 0)
          return;
        this.chkShowAll.Checked = true;
        this.btnResign.Enabled = this.btnImport.Enabled = this.chkShowAll.Enabled = true;
      }
    }

    private void FillDrives()
    {
      this.cbDrives.Invoke((Delegate) this.ClearDrivesFunc);
      foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
      {
        if (driveInfo.IsReady && driveInfo.DriveType == DriveType.Removable)
        {
          if (driveInfo != null)
            this.cbDrives.Invoke((Delegate) this.AddItemFunc, (object) driveInfo.RootDirectory.FullName);
          else
            this.cbDrives.Invoke((Delegate) this.AddItemFunc, new object[1]);
        }
      }
    }

    private void FillSavesList(string xml)
    {
      this.m_games = new List<game>();
      new XmlDocument().PreserveWhitespace = true;
      try
      {
        using (StringReader stringReader = new StringReader(xml))
        {
          games games = (games) new XmlSerializer(typeof (games)).Deserialize((TextReader) stringReader);
          this.m_games = games._games;
          this.m_rblist = games.rblist;
        }
      }
      catch (Exception ex1)
      {
        try
        {
          xml = xml.Replace("&", "&amp;");
          using (StringReader stringReader = new StringReader(xml))
            this.m_games = ((games) new XmlSerializer(typeof (games)).Deserialize((TextReader) stringReader))._games;
        }
        catch (Exception ex2)
        {
          return;
        }
      }
      this.m_games.Sort((Comparison<game>) ((item1, item2) => (item1.name + item1.LocalSaveFolder).CompareTo(item2.name + item1.LocalSaveFolder)));
    }

    private int GetPSNIDInfo()
    {
      WebClientEx webClientEx = new WebClientEx();
      webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
      webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
      Dictionary<string, object> dictionary = new JavaScriptSerializer().Deserialize(Encoding.UTF8.GetString(webClientEx.UploadData(Util.GetBaseUrl() + "/ps4auth", Encoding.UTF8.GetBytes(string.Format("{{\"action\":\"PSNID_INFO\",\"userid\":\"{0}\"}}", (object) Util.GetUserId())))), typeof (Dictionary<string, object>)) as Dictionary<string, object>;
      if (!dictionary.ContainsKey("status") || !((string) dictionary["status"] == "OK"))
        return 0;
      this.m_psnIDs = !dictionary.ContainsKey("psnid") ? new Dictionary<string, object>() : dictionary["psnid"] as Dictionary<string, object>;
      this.m_psn_quota = Convert.ToInt32(dictionary["psnid_quota"]);
      this.m_psn_remaining = Convert.ToInt32(dictionary["psnid_remaining"]);
      this.gbProfiles.Controls.Clear();
      this.gbProfiles.Width = this.m_psn_quota * 18 + 35;
      for (int index = 0; index < this.m_psn_quota; ++index)
      {
        PictureBox pictureBox = new PictureBox();
        pictureBox.Left = 8 + index * 18;
        pictureBox.Top = 8;
        pictureBox.Width = 18;
        this.gbProfiles.Controls.Add((Control) pictureBox);
      }
      TextBox textBox = new TextBox();
      textBox.Text = string.Format("{0}/{1}", (object) (this.m_psn_quota - this.m_psn_remaining), (object) this.m_psn_quota);
      textBox.Left = this.m_psn_quota * 18 + 8;
      textBox.Top = 9;
      textBox.Width = 26;
      textBox.ForeColor = System.Drawing.Color.White;
      textBox.BorderStyle = BorderStyle.None;
      textBox.BackColor = System.Drawing.Color.FromArgb(102, 132, 162);
      this.gbProfiles.Controls.Add((Control) textBox);
      return this.m_psn_quota;
    }

    public bool IsValidPSNID(string psnId)
    {
      return this.m_psnIDs != null && this.m_psnIDs.ContainsKey(psnId);
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      if (this.chkShowAll.Checked)
        e.Cancel = true;
      else if (this.dgServerGames.SelectedCells.Count == 0 || this.cbDrives.Items.Count == 0)
      {
        e.Cancel = true;
      }
      else
      {
        this.simpleToolStripMenuItem.Visible = true;
        this.advancedToolStripMenuItem.Visible = true;
        int rowIndex = this.dgServerGames.SelectedCells[1].RowIndex;
        if (!(bool) this.dgServerGames.Rows[rowIndex].Cells[5].Value)
          e.Cancel = true;
        if (this.dgServerGames.Rows[rowIndex].Cells[1].Tag == "U")
        {
          this.registerPSNIDToolStripMenuItem.Visible = true;
          this.registerPSNIDToolStripMenuItem.Enabled = true;
          this.simpleToolStripMenuItem.Enabled = false;
          this.advancedToolStripMenuItem.Enabled = false;
          this.restoreFromBackupToolStripMenuItem.Enabled = false;
        }
        else
        {
          this.registerPSNIDToolStripMenuItem.Visible = false;
          this.registerPSNIDToolStripMenuItem.Enabled = false;
          this.restoreFromBackupToolStripMenuItem.Enabled = true;
          game game = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as game;
          if (game == null)
          {
            e.Cancel = true;
          }
          else
          {
            container targetGameFolder = game.GetTargetGameFolder();
            if (targetGameFolder != null)
            {
              int? quickmode = targetGameFolder.quickmode;
              if ((quickmode.GetValueOrDefault() <= 0 ? 0 : (quickmode.HasValue ? 1 : 0)) != 0)
                this.advancedToolStripMenuItem.Enabled = false;
              else
                this.advancedToolStripMenuItem.Enabled = true;
              this.simpleToolStripMenuItem.Enabled = true;
            }
            else
            {
              this.simpleToolStripMenuItem.Enabled = false;
              this.advancedToolStripMenuItem.Enabled = false;
            }
            this.deleteSaveToolStripMenuItem.Visible = true;
            this.restoreFromBackupToolStripMenuItem.Visible = true;
          }
        }
      }
    }

    private void simpleToolStripMenuItem_Click(object sender, EventArgs e)
    {
      object obj1 = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].Value;
      string toolTipText = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].ToolTipText;
      game gameItem = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as game;
      if (gameItem == null || gameItem.PSN_ID != null && !this.IsValidPSNID(gameItem.PSN_ID) || this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[2].Value as string == "N/A")
        return;
      List<string> list = (List<string>) null;
      if (!this.chkShowAll.Checked)
      {
        list = gameItem.GetContainerFiles();
        if (list == null || list.Count < 2)
        {
          return;
        }
      }
      container targetGameFolder = gameItem.GetTargetGameFolder();
      if (targetGameFolder != null)
      {
        int? locked = targetGameFolder.locked;
        if ((locked.GetValueOrDefault() <= 0 ? 0 : (locked.HasValue ? 1 : 0)) != 0 )
          return;
      }
      int rowIndex = this.dgServerGames.SelectedCells[0].RowIndex;
      List<string> files = new List<string>();
      if (!this.chkShowAll.Checked)
      {
        string str = gameItem.LocalSaveFolder.Substring(0, gameItem.LocalSaveFolder.Length - 4);
        gameItem.ToString(new List<string>(), "decrypt");
        Util.GetTempFolder();
        if (!string.IsNullOrEmpty(gameItem.notes))
        {
          int num1 = (int) new Notes(gameItem.notes).ShowDialog((IWin32Window) this);
        }
        if (targetGameFolder.preprocess == 1)
        {
          list.Remove(str);
          AdvancedSaveUploaderForEncrypt uploaderForEncrypt = new AdvancedSaveUploaderForEncrypt(list.ToArray(), gameItem, (string) null, "list");
          if (uploaderForEncrypt.ShowDialog((IWin32Window) this) != DialogResult.Abort && !string.IsNullOrEmpty(uploaderForEncrypt.ListResult))
          {
            foreach (object obj2 in new JavaScriptSerializer().Deserialize(uploaderForEncrypt.ListResult, typeof (ArrayList)) as ArrayList)
              files.Add((string) obj2);
          }
          else
          {
            return;
          }
        }
      }
      SimpleTreeEdit simpleTreeEdit = new SimpleTreeEdit(gameItem, this.chkShowAll.Checked, files);
      if (simpleTreeEdit.ShowDialog() == DialogResult.OK)
      {
        this.dgServerGames.Rows[rowIndex].Tag = (object) simpleTreeEdit.GameItem;
        this.dgServerGames.Rows[rowIndex].Cells[2].Value = (object) simpleTreeEdit.GameItem.GetCheatCount();
        this.PrepareLocalSavesMap();
        string expandGame = this.m_expandedGame;
        this.m_expandedGame = (string) null;
        int scrollingRowIndex = this.dgServerGames.FirstDisplayedScrollingRowIndex;
        this.FillLocalSaves(expandGame, this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection == SortOrder.Ascending);
        this.dgServerGames.Rows[Math.Min(rowIndex, this.dgServerGames.Rows.Count - 1)].Selected = true;
        try
        {
          this.dgServerGames.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
        }
        catch (Exception ex)
        {
        }
      }
      else
      {
        int scrollingRowIndex = this.dgServerGames.FirstDisplayedScrollingRowIndex;
        this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
        this.dgServerGames.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
      }
    }

    private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.dgServerGames.SelectedCells.Count == 0)
        return;
      Util.ClearTemp();
      object obj = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].Value;
      string toolTipText = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].ToolTipText;
      game game = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as game;
      List<string> containerFiles = game.GetContainerFiles();
      if (containerFiles.Count < 2)
      {
      }
      else
      {
        int? locked = game.GetTargetGameFolder().locked;
        if ((locked.GetValueOrDefault() <= 0 ? 0 : (locked.HasValue ? 1 : 0)) != 0 )
          return;
        string str = game.LocalSaveFolder.Substring(0, game.LocalSaveFolder.Length - 4);
        game.ToString(new List<string>(), "decrypt");
        containerFiles.Remove(str);
        if (!string.IsNullOrEmpty(game.notes))
        {
          int num2 = (int) new Notes(game.notes).ShowDialog((IWin32Window) this);
        }
        AdvancedSaveUploaderForEncrypt uploaderForEncrypt = new AdvancedSaveUploaderForEncrypt(containerFiles.ToArray(), game, (string) null, "decrypt");
        if (uploaderForEncrypt.ShowDialog() == DialogResult.Abort || uploaderForEncrypt.DecryptedSaveData == null || (uploaderForEncrypt.DecryptedSaveData.Count <= 0 || new AdvancedEdit(game, uploaderForEncrypt.DecryptedSaveData, uploaderForEncrypt.DependentSaveData).ShowDialog((IWin32Window) this) != DialogResult.OK))
          return;
        this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
      }
    }

    private void cbDrives_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.dgResign.Rows.Clear();
      if (this.cbDrives.SelectedItem == null)
        return;
      this.dgServerGames.Columns[0].Width = 25;
      int width = this.dgServerGames.Width;
      this.dgServerGames.Columns[1].Width = (int) ((double) (width - 25) * 0.649999976158142);
      this.dgServerGames.Columns[2].Width = (int) ((double) (width - 25) * 0.150000005960464);
      this.dgServerGames.Columns[3].Visible = false;
      this.dgServerGames.Columns[4].Width = (int) ((double) (width - 25) * 0.200000002980232);
      this.dgServerGames.Columns[4].Visible = true;
      this.dgResign.Columns[0].Width = 25;
      this.dgResign.Columns[1].Width = (int) ((double) (this.dgResign.Width - 25) * 0.649999976158142);
      this.dgResign.Columns[2].Width = (int) ((double) (this.dgResign.Width - 25) * 0.140000000596046);
      this.dgResign.Columns[3].Width = (int) ((double) (this.dgResign.Width - 25) * 0.180000007152557);
      string str = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str + "PS4//SAVEDATA") || Directory.GetDirectories(str + "PS4//SAVEDATA").Length == 0)
      {
        if (!this.chkShowAll.Enabled)
        {
          this.btnResign.Enabled = this.btnImport.Enabled = this.chkShowAll.Enabled = true;
          this.chkShowAll.Checked = false;
        }
        if (this.chkShowAll.Checked)
          return;
        this.pnlNoSaves.Visible = true;
        this.pnlNoSaves.BringToFront();
      }
      else
      {
        if (!this.chkShowAll.Checked)
        {
          this.pnlNoSaves.Visible = false;
          this.pnlNoSaves.SendToBack();
          this.PrepareLocalSavesMap();
          this.FillLocalSaves((string) null, true);
          this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
        }
        else
          this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
        this.FillResignSaves((string) null, true);
      }
    }

    private void FillResignSaves()
    {
      this.dgResign.Rows.Clear();
      if (this.cbDrives.SelectedItem == null)
        return;
      string str1 = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str1 + "PS4\\SAVEDATA"))
        return;
      string[] directories = Directory.GetDirectories(str1 + "PS4\\SAVEDATA");
      foreach (string str2 in this.m_dictAllLocalSaves.Keys)
        ;
      foreach (string path1 in directories)
      {
        if (this.IsValidPSNID(Path.GetFileName(path1)))
        {
          foreach (string path2 in Directory.GetDirectories(path1))
          {
            foreach (string fileName1 in Directory.GetFiles(path2, "*.bin"))
            {
              if (new FileInfo(fileName1).Length < 2048L)
              {
                string str2 = fileName1.Substring(0, fileName1.Length - 4);
                if (System.IO.File.Exists(str2))
                {
                  string fileName2 = Path.GetFileName(path2);
                  game game = new game()
                  {
                    id = fileName2,
                    containers = new containers()
                    {
                      _containers = new List<container>()
                      {
                        new container()
                        {
                          pfs = Path.GetFileName(str2)
                        }
                      }
                    },
                    LocalSaveFolder = fileName1
                  };
                  string saveId;
                  int onlineSaveIndex = MainForm3.GetOnlineSaveIndex(this.m_games, str2, out saveId);
                  int index = this.dgResign.Rows.Add();
                  this.dgResign.Rows[index].Tag = (object) path2;
                  this.dgResign.Rows[index].Cells[0].Tag = (object) game;
                  if (onlineSaveIndex >= 0)
                    this.dgResign.Rows[index].Cells[0].Value = (object) this.m_games[onlineSaveIndex].name;
                  else
                    this.dgResign.Rows[index].Cells[0].Value = (object) fileName2;
                }
              }
            }
          }
        }
      }
    }

    public static int GetOnlineSaveIndex(List<game> games, string save, out string saveId)
    {
      string fileName = Path.GetFileName(Path.GetDirectoryName(save));
      string withoutExtension = Path.GetFileNameWithoutExtension(save);
      for (int index1 = 0; index1 < games.Count; ++index1)
      {
        saveId = games[index1].id;
        if (fileName.Equals(games[index1].id) || games[index1].IsAlias(fileName, out saveId))
        {
          for (int index2 = 0; index2 < games[index1].containers._containers.Count; ++index2)
          {
            if (withoutExtension == games[index1].containers._containers[index2].pfs || Util.IsMatch(withoutExtension, games[index1].containers._containers[index2].pfs))
              return index1;
          }
        }
      }
      saveId = (string) null;
      return -1;
    }

    private int GetOnlineSaveIndexByGameName(string gameName)
    {
      for (int index = 0; index < this.m_games.Count; ++index)
      {
        if (gameName.Equals(this.m_games[index].name))
          return index;
      }
      return -1;
    }

    public static string GetParamInfo(string sfoFile, string item)
    {
      if (!System.IO.File.Exists(sfoFile))
        return "";
      byte[] bytes = System.IO.File.ReadAllBytes(sfoFile);
      int num1 = BitConverter.ToInt32(bytes, 8);
      int num2 = BitConverter.ToInt32(bytes, 12);
      int num3 = BitConverter.ToInt32(bytes, 16);
      int num4 = 16;
      for (int index = 0; index < num3; ++index)
      {
        short num5 = BitConverter.ToInt16(bytes, index * num4 + 20);
        int num6 = BitConverter.ToInt32(bytes, index * num4 + 12 + 20);
        if (Encoding.UTF8.GetString(bytes, num1 + (int) num5, item.Length) == item)
        {
          int count = 0;
          while (count < bytes.Length && (int) bytes[num2 + num6 + count] != 0)
            ++count;
          return Encoding.UTF8.GetString(bytes, num2 + num6, count);
        }
      }
      return "";
    }

    public static byte[] GetParamInfo(string sfoFile, out int profileId)
    {
      profileId = 0;
      if (!System.IO.File.Exists(sfoFile))
        return (byte[]) null;
      byte[] bytes = System.IO.File.ReadAllBytes(sfoFile);
      int num1 = BitConverter.ToInt32(bytes, 8);
      int num2 = BitConverter.ToInt32(bytes, 12);
      int num3 = BitConverter.ToInt32(bytes, 16);
      int num4 = 16;
      for (int index = 0; index < num3; ++index)
      {
        short num5 = BitConverter.ToInt16(bytes, index * num4 + 20);
        int num6 = BitConverter.ToInt32(bytes, index * num4 + 12 + 20);
        if (Encoding.UTF8.GetString(bytes, num1 + (int) num5, 5) == "PARAM")
        {
          byte[] numArray = new byte[16];
          Array.Copy((Array) bytes, num2 + num6 + 28, (Array) numArray, 0, 16);
          profileId = BitConverter.ToInt32(bytes, num2 + num6 + 28 + 16);
          return numArray;
        }
      }
      return (byte[]) null;
    }

    private string GetSaveDescription(string sfoFile)
    {
      return MainForm3.GetParamInfo(sfoFile, "SUB_TITLE") + "\r\n" + MainForm3.GetParamInfo(sfoFile, "DETAIL");
    }

    private string GetSaveTitle(string sfoFile)
    {
      return MainForm3.GetParamInfo(sfoFile, "TITLE");
    }

    private void btnHome_Click(object sender, EventArgs e)
    {
      this.pnlHome.Visible = true;
      this.pnlBackup.Visible = false;
      this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
            btnOptions.Image = PS4SaveEditor.Properties.Resources.setting0;
            btnHome.Image = PS4SaveEditor.Properties.Resources.Game1;
        }

    private void btnSaves_Click(object sender, EventArgs e)
    {
      this.pnlHome.Visible = false;
      this.pnlBackup.Visible = false;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      switch (folderBrowserDialog.ShowDialog())
      {
        case DialogResult.OK:
        case DialogResult.Yes:
          this.txtBackupLocation.Text = folderBrowserDialog.SelectedPath;
          this.btnApply_Click((object) null, (EventArgs) null);
          break;
      }
    }

    private void chkBackup_CheckedChanged(object sender, EventArgs e)
    {
      this.txtBackupLocation.Enabled = this.chkBackup.Checked;
      this.btnBrowse.Enabled = this.chkBackup.Checked;
      Util.SetRegistryValue("BackupSaves", this.chkBackup.Checked ? "true" : "false");
    }

    private void btnBackup_Click(object sender, EventArgs e)
    {
      this.pnlBackup.Visible = true;
      this.pnlHome.Visible = false;
      this.chkBackup.Checked = Util.GetRegistryValue("BackupSaves") != "false";
      this.txtBackupLocation.Text = Util.GetBackupLocation();
      btnOptions.Image = PS4SaveEditor.Properties.Resources.setting1;
      btnHome.Image = PS4SaveEditor.Properties.Resources.Game0;
   }

    private void btnApply_Click(object sender, EventArgs e)
    {
      Util.SetRegistryValue("Location", this.txtBackupLocation.Text);
      Util.SetRegistryValue("BackupSaves", this.chkBackup.Checked ? "true" : "false");
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      try
      {
        if (this.evt == null)
          return;
        this.evt.Set();
        this.evt2.Set();
        Directory.Delete(Util.GetTempFolder(), true);
        if (!this.m_sessionInited)
          return;
        try
        {
          WebClientEx webClientEx = new WebClientEx();
          webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
          webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
          webClientEx.UploadData(Util.GetBaseUrl() + "/ps4auth?token=" + Util.GetAuthToken(), Encoding.ASCII.GetBytes(string.Format("{{\"action\":\"DESTROY_SESSION\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}", (object) Util.GetUserId(), (object) Util.GetUID(false, false))));
        }
        catch (Exception ex)
        {
        }
      }
      catch
      {
      }
    }

    private void SaveUserCheats()
    {
      string str = "<usercheats>";
      foreach (DataGridViewRow dataGridViewRow in (IEnumerable) this.dgServerGames.Rows)
      {
        if (dataGridViewRow.Tag != null)
        {
          game game = dataGridViewRow.Tag as game;
          if (game != null && game.GetTargetGameFolder() != null)
          {
            str += string.Format("<game id=\"{0}\">", (object) Path.GetFileName(game.LocalSaveFolder));
            foreach (cheat cheat in game.GetTargetGameFolder().files._files[0].Cheats)
            {
              if (cheat.id == "-1")
              {
                str = str + "<cheat desc=\"" + cheat.name + "\" comment=\"" + cheat.note + "\">";
                str += cheat.ToString(false);
                str += "</cheat>";
              }
            }
            str += "</game>";
          }
        }
      }
      string contents = str + "</usercheats>";
      System.IO.File.WriteAllText(Util.GetBackupLocation() + (object) Path.DirectorySeparatorChar + MainForm3.USER_CHEATS_FILE, contents);
    }

    private bool CheckForVersion()
    {
      return true;
    }

    private void btnRss_Click(object sender, EventArgs e)
    {
            MessageBox.Show("Version Cracker ...","Erreur 0x01");
    }

    private void restoreFromBackupToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.dgServerGames.SelectedRows.Count != 1)
        return;
      game game = this.dgServerGames.SelectedRows[0].Tag as game;
      string[] files = Directory.GetFiles(Util.GetBackupLocation(), game.PSN_ID + "_" + Path.GetFileName(Path.GetDirectoryName(game.LocalSaveFolder)) + "_" + Path.GetFileNameWithoutExtension(game.LocalSaveFolder) + "_*");
      if (files.Length == 1)
      {
        int num1 = (int) new RestoreBackup(files[0], Path.GetDirectoryName(game.LocalSaveFolder)).ShowDialog();
      }
      else if (files.Length == 0)
      {
      }
      else
      {
        int num4 = (int) new ChooseBackup(game.name, game.PSN_ID + "_" + Path.GetFileName(Path.GetDirectoryName(game.LocalSaveFolder)) + "_", game.LocalSaveFolder).ShowDialog();
      }
    }

    private void btnDeactivate_Click(object sender, EventArgs e)
    {
            MessageBox.Show("Version cracker ...", "Erreur 0x02");
    }

    private bool ActivateLicense()
    {
      try
      {
        WebClientEx webClientEx = new WebClientEx();
        webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
        webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
        if ((string) (new JavaScriptSerializer().Deserialize(Encoding.ASCII.GetString(webClientEx.UploadData(Util.GetAuthBaseUrl() + "/ps4auth", Encoding.ASCII.GetBytes(string.Format("{{\"action\":\"UNREGISTER_UUID\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}", (object) Util.GetUserId(), (object) Util.GetUID(false, false))))), typeof (Dictionary<string, object>)) as Dictionary<string, object>)["status"] == "OK")
        {
          RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Util.GetRegistryBase(), true);
          foreach (string name in registryKey.GetValueNames())
          {
            if (name != "Location")
              registryKey.DeleteValue(name);
          }
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
      Process process = new Process();
      process.StartInfo = new ProcessStartInfo("explorer", this.txtBackupLocation.Text);
      process.StartInfo.Verb = "open";
      process.StartInfo.UseShellExecute = true;
      process.Start();
    }

    private void btnHelp_Click(object sender, EventArgs e)
    {
      Path.GetDirectoryName(Application.ExecutablePath);
      Process.Start(new ProcessStartInfo()
      {
        UseShellExecute = true,
        Verb = "open",
        FileName = "AdvanceEdit.cs"
      });
    }

    private void MainForm_ResizeEnd(object sender, EventArgs e)
    {
    }

    private string FindGGUSB()
    {
      ManagementScope scope = new ManagementScope("root\\cimv2");
      WqlObjectQuery wqlObjectQuery1 = new WqlObjectQuery("SELECT * FROM Win32_DiskDrive where Model = 'dpdev GameGenie USB Device'");
      ManagementObjectSearcher managementObjectSearcher1 = new ManagementObjectSearcher(scope, (ObjectQuery) wqlObjectQuery1);
      ManagementBaseObject[] objectCollection1 = new ManagementBaseObject[1];
      ManagementObjectCollection objectCollection2 = managementObjectSearcher1.Get();
      if (objectCollection2.Count > 0)
      {
        objectCollection2.CopyTo(objectCollection1, 0);
        string str1 = ((string) objectCollection1[0].Properties["DeviceID"].Value).Replace("\\\\", "\\\\\\\\").Replace(".\\", ".\\\\");
        string[] strArray = objectCollection1[0].Properties["PNPDeviceID"].Value.ToString().Split(new char[2]
        {
          '\\',
          '&'
        });
        WqlObjectQuery wqlObjectQuery2 = new WqlObjectQuery("ASSOCIATORS OF {Win32_DiskDrive.DeviceID=\"" + str1 + "\"} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
        ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher(scope, (ObjectQuery) wqlObjectQuery2);
        ManagementObjectCollection objectCollection3 = managementObjectSearcher2.Get();
        if (objectCollection3.Count == 1)
        {
          objectCollection3.CopyTo(objectCollection1, 0);
          WqlObjectQuery wqlObjectQuery3 = new WqlObjectQuery("ASSOCIATORS OF {Win32_DiskPartition.DeviceID=\"" + (string) objectCollection1[0].Properties["DeviceID"].Value + "\"} WHERE AssocClass = Win32_LogicalDiskToPartition");
          ManagementObjectSearcher managementObjectSearcher3 = new ManagementObjectSearcher(scope, (ObjectQuery) wqlObjectQuery3);
          ManagementObjectCollection objectCollection4 = managementObjectSearcher3.Get();
          if (objectCollection4.Count == 1)
          {
            objectCollection4.CopyTo(objectCollection1, 0);
            string str2 = (string) objectCollection1[0].Properties["DeviceID"].Value;
            managementObjectSearcher3.Dispose();
            managementObjectSearcher2.Dispose();
            managementObjectSearcher1.Dispose();
            return strArray[5];
          }
          managementObjectSearcher3.Dispose();
        }
        managementObjectSearcher2.Dispose();
      }
      managementObjectSearcher1.Dispose();
      return (string) null;
    }

    private void deleteSaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      object obj = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].Value;
      string toolTipText = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].ToolTipText;
      game game = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as game;
      string path = game != null ? game.LocalSaveFolder : this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as string;
      if (path == null)
        return;
      try
      {
        System.IO.File.Delete(path);
        System.IO.File.Delete(path.Substring(0, game.LocalSaveFolder.Length - 4));
      }
      catch (Exception ex)
      {
      }
      this.dgServerGames.Rows.Remove(this.dgServerGames.SelectedRows[0]);
      int scrollingRowIndex = this.dgServerGames.FirstDisplayedScrollingRowIndex;
      this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
      if (this.dgServerGames.Rows.Count <= scrollingRowIndex || scrollingRowIndex < 0)
        return;
      this.dgServerGames.FirstDisplayedScrollingRowIndex = scrollingRowIndex;
    }

    private void btnGamesInServer_Click(object sender, EventArgs e)
    {
    }

    private void chkBackup_Click(object sender, EventArgs e)
    {
       this.chkBackup.Checked = true;
    }

    private void btnManageProfiles_Click(object sender, EventArgs e)
    {
      ManageProfiles manageProfiles = new ManageProfiles("", this.m_psnIDs);
      int num = (int) manageProfiles.ShowDialog();
      if (!string.IsNullOrEmpty(manageProfiles.PsnIDResponse))
      {
        Dictionary<string, object> dictionary = new JavaScriptSerializer().Deserialize(manageProfiles.PsnIDResponse, typeof (Dictionary<string, object>)) as Dictionary<string, object>;
        if (dictionary.ContainsKey("status") && (string) dictionary["status"] == "OK")
        {
          this.m_psnIDs = !dictionary.ContainsKey("psnid") ? new Dictionary<string, object>() : dictionary["psnid"] as Dictionary<string, object>;
          this.m_psn_quota = Convert.ToInt32(dictionary["psnid_quota"]);
          this.m_psn_remaining = Convert.ToInt32(dictionary["psnid_remaining"]);
        }
      }
      this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
    }

    private void registerPSNIDToolStripMenuItem_Click(object sender, EventArgs e)
    {

      {
        if (this.dgServerGames.SelectedRows.Count != 1)
          return;
        ManageProfiles manageProfiles = new ManageProfiles((this.dgServerGames.SelectedRows[0].Tag as game).PSN_ID, this.m_psnIDs);
        if (manageProfiles.ShowDialog() != DialogResult.OK)
          return;
        if (!string.IsNullOrEmpty(manageProfiles.PsnIDResponse))
        {
          Dictionary<string, object> dictionary = new JavaScriptSerializer().Deserialize(manageProfiles.PsnIDResponse, typeof (Dictionary<string, object>)) as Dictionary<string, object>;
          if (dictionary.ContainsKey("status") && (string) dictionary["status"] == "OK")
          {
            this.m_psnIDs = !dictionary.ContainsKey("psnid") ? new Dictionary<string, object>() : dictionary["psnid"] as Dictionary<string, object>;
            this.m_psn_quota = Convert.ToInt32(dictionary["psnid_quota"]);
            this.m_psn_remaining = Convert.ToInt32(dictionary["psnid_remaining"]);
          }
        }
        this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
      }
    }

    private void resignToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.dgServerGames.SelectedCells.Count == 0)
        return;
      game game = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as game;
      switch (game != null ? game.LocalSaveFolder : this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Tag as string)
      {
        case null:
          break;
        default:
          this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
          break;
      }
    }

    private bool RegisterSerial()
    {
      try
      {
        WebClientEx webClientEx = new WebClientEx();
        webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
        string str1 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Util.GetRegistryValue("Serial")))).Replace("-", "");
        string uid = Util.GetUID(false, true);
        if (string.IsNullOrEmpty(uid))
        {
          return false;
        }
        string uriString = string.Format("{0}/ps4auth", (object) Util.GetBaseUrl(), (object) uid, (object) str1);
        string str2 = webClientEx.DownloadString(new Uri(uriString, UriKind.Absolute));
        if (str2.IndexOf('#') > 0)
        {
          string[] strArray = str2.Split('#');
          if (strArray.Length > 1)
          {
            if (strArray[0] == "4")
            {
              return false;
            }
            if (strArray[0] == "5")
            {
              return false;
            }
          }
        }
        else if (str2 == null || str2.ToLower().Contains("error") || str2.ToLower().Contains("not found"))
        {
          string str3 = str2.Replace("ERROR", "");
          if (str3.Contains("1007"))
          {
            Util.GetUID(true, true);
            return this.RegisterSerial();
          }
          if (str3.Contains("1004"))
          {
            return false;
          }
          if (str3.Contains("1005"))
          {
            return false;
          }
          return false;
        }
        return true;
      }
      catch (Exception ex)
      {
        int num1 = (int) MessageBox.Show(ex.Message, ex.StackTrace);
      }
      return false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.simpleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerPSNIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreFromBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlHome = new System.Windows.Forms.Panel();
            this.tabPageResign = new System.Windows.Forms.Panel();
            this.dgResign = new CSUST.Data.CustomDataGridView();
            this._Head = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JuegoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SysVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PSNID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resignToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageGames = new System.Windows.Forms.Panel();
            this.pnlNoSaves = new System.Windows.Forms.Panel();
            this.lblNoSaves = new System.Windows.Forms.Label();
            this.dgServerGames = new CSUST.Data.CustomDataGridView();
            this.Choose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Añadido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkShowAll = new System.Windows.Forms.CheckBox();
            this.btnResign = new System.Windows.Forms.Button();
            this.btnCheats = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnGamesInServer = new System.Windows.Forms.Button();
            this.pnlBackup = new System.Windows.Forms.Panel();
            this.gbManageProfile = new PS3SaveEditor.CustomGroupBox();
            this.gbProfiles = new PS3SaveEditor.CustomGroupBox();
            this.lblManageProfiles = new System.Windows.Forms.Label();
            this.btnManageProfiles = new System.Windows.Forms.Button();
            this.groupBox2 = new PS3SaveEditor.CustomGroupBox();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblDeactivate = new System.Windows.Forms.Label();
            this.btnDeactivate = new System.Windows.Forms.Button();
            this.groupBox1 = new PS3SaveEditor.CustomGroupBox();
            this.lblRSSSection = new System.Windows.Forms.Label();
            this.btnRss = new System.Windows.Forms.Button();
            this.gbBackupLocation = new PS3SaveEditor.CustomGroupBox();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblBackup = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtBackupLocation = new System.Windows.Forms.TextBox();
            this.chkBackup = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.Multi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbDrives = new System.Windows.Forms.ComboBox();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            this.pnlHome.SuspendLayout();
            this.tabPageResign.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResign)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.tabPageGames.SuspendLayout();
            this.pnlNoSaves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgServerGames)).BeginInit();
            this.pnlBackup.SuspendLayout();
            this.gbManageProfile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbBackupLocation.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simpleToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.toolStripSeparator1,
            this.resignToolStripMenuItem,
            this.registerPSNIDToolStripMenuItem,
            this.toolStripSeparator2,
            this.restoreFromBackupToolStripMenuItem,
            this.deleteSaveToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.Size = new System.Drawing.Size(272, 208);
            this.contextMenuStrip1.Text = "jjjjj";
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // simpleToolStripMenuItem
            // 
            this.simpleToolStripMenuItem.Name = "simpleToolStripMenuItem";
            this.simpleToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.simpleToolStripMenuItem.Text = "Simple";
            this.simpleToolStripMenuItem.Click += new System.EventHandler(this.simpleToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.advancedToolStripMenuItem.Text = "Advanced";
            this.advancedToolStripMenuItem.Click += new System.EventHandler(this.advancedToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(268, 6);
            // 
            // resignToolStripMenuItem
            // 
            this.resignToolStripMenuItem.Name = "resignToolStripMenuItem";
            this.resignToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.resignToolStripMenuItem.Text = "Re-sign...";
            this.resignToolStripMenuItem.Click += new System.EventHandler(this.resignToolStripMenuItem_Click);
            // 
            // registerPSNIDToolStripMenuItem
            // 
            this.registerPSNIDToolStripMenuItem.Name = "registerPSNIDToolStripMenuItem";
            this.registerPSNIDToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.registerPSNIDToolStripMenuItem.Text = "Register PSN ID...";
            this.registerPSNIDToolStripMenuItem.Click += new System.EventHandler(this.registerPSNIDToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(268, 6);
            // 
            // restoreFromBackupToolStripMenuItem
            // 
            this.restoreFromBackupToolStripMenuItem.Name = "restoreFromBackupToolStripMenuItem";
            this.restoreFromBackupToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.restoreFromBackupToolStripMenuItem.Text = "Restore from Backup";
            this.restoreFromBackupToolStripMenuItem.Click += new System.EventHandler(this.restoreFromBackupToolStripMenuItem_Click);
            // 
            // deleteSaveToolStripMenuItem
            // 
            this.deleteSaveToolStripMenuItem.Name = "deleteSaveToolStripMenuItem";
            this.deleteSaveToolStripMenuItem.Size = new System.Drawing.Size(271, 32);
            this.deleteSaveToolStripMenuItem.Text = "Delete Save";
            this.deleteSaveToolStripMenuItem.Click += new System.EventHandler(this.deleteSaveToolStripMenuItem_Click);
            // 
            // pnlHome
            // 
            this.pnlHome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.pnlHome.Controls.Add(this.tabPageResign);
            this.pnlHome.Controls.Add(this.tabPageGames);
            this.pnlHome.Controls.Add(this.chkShowAll);
            this.pnlHome.Controls.Add(this.btnResign);
            this.pnlHome.Controls.Add(this.btnCheats);
            this.pnlHome.Controls.Add(this.btnImport);
            this.pnlHome.Location = new System.Drawing.Point(381, 22);
            this.pnlHome.Margin = new System.Windows.Forms.Padding(4);
            this.pnlHome.Name = "pnlHome";
            this.pnlHome.Size = new System.Drawing.Size(766, 520);
            this.pnlHome.TabIndex = 8;
            this.pnlHome.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHome_Paint);
            // 
            // tabPageResign
            // 
            this.tabPageResign.Controls.Add(this.dgResign);
            this.tabPageResign.Location = new System.Drawing.Point(6, 33);
            this.tabPageResign.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageResign.Name = "tabPageResign";
            this.tabPageResign.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageResign.Size = new System.Drawing.Size(760, 488);
            this.tabPageResign.TabIndex = 1;
            this.tabPageResign.Text = "Re-Sign";
            // 
            // dgResign
            // 
            this.dgResign.AllowUserToAddRows = false;
            this.dgResign.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgResign.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgResign.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResign.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._Head,
            this.JuegoID,
            this.SysVer,
            this.PSNID});
            this.dgResign.ContextMenuStrip = this.contextMenuStrip2;
            this.dgResign.Location = new System.Drawing.Point(-2, 0);
            this.dgResign.Margin = new System.Windows.Forms.Padding(4);
            this.dgResign.Name = "dgResign";
            this.dgResign.ReadOnly = true;
            this.dgResign.RowHeadersVisible = false;
            this.dgResign.RowHeadersWidth = 62;
            this.dgResign.RowTemplate.Height = 24;
            this.dgResign.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgResign.Size = new System.Drawing.Size(762, 488);
            this.dgResign.TabIndex = 0;
            this.dgResign.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResign_CellContentClick);
            this.dgResign.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResign_CellDoubleClick);
            // 
            // _Head
            // 
            this._Head.FillWeight = 20F;
            this._Head.HeaderText = "";
            this._Head.MinimumWidth = 8;
            this._Head.Name = "_Head";
            this._Head.ReadOnly = true;
            this._Head.Width = 20;
            // 
            // JuegoID
            // 
            this.JuegoID.FillWeight = 200F;
            this.JuegoID.HeaderText = "Game";
            this.JuegoID.MinimumWidth = 8;
            this.JuegoID.Name = "JuegoID";
            this.JuegoID.ReadOnly = true;
            this.JuegoID.Width = 200;
            // 
            // SysVer
            // 
            this.SysVer.FillWeight = 50F;
            this.SysVer.HeaderText = "Sys. Ver.";
            this.SysVer.MinimumWidth = 8;
            this.SysVer.Name = "SysVer";
            this.SysVer.ReadOnly = true;
            this.SysVer.Width = 150;
            // 
            // PSNID
            // 
            this.PSNID.FillWeight = 50F;
            this.PSNID.HeaderText = "Profile/PSN ID";
            this.PSNID.MinimumWidth = 8;
            this.PSNID.Name = "PSNID";
            this.PSNID.ReadOnly = true;
            this.PSNID.Width = 250;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resignToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(173, 36);
            // 
            // resignToolStripMenuItem1
            // 
            this.resignToolStripMenuItem1.Name = "resignToolStripMenuItem1";
            this.resignToolStripMenuItem1.Size = new System.Drawing.Size(172, 32);
            this.resignToolStripMenuItem1.Text = "Mis Chetos";
            // 
            // tabPageGames
            // 
            this.tabPageGames.Controls.Add(this.pnlNoSaves);
            this.tabPageGames.Controls.Add(this.dgServerGames);
            this.tabPageGames.Location = new System.Drawing.Point(6, 33);
            this.tabPageGames.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageGames.Name = "tabPageGames";
            this.tabPageGames.Size = new System.Drawing.Size(760, 488);
            this.tabPageGames.TabIndex = 0;
            this.tabPageGames.Text = "Chetos";
            // 
            // pnlNoSaves
            // 
            this.pnlNoSaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNoSaves.Controls.Add(this.lblNoSaves);
            this.pnlNoSaves.Location = new System.Drawing.Point(2, 0);
            this.pnlNoSaves.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNoSaves.Name = "pnlNoSaves";
            this.pnlNoSaves.Size = new System.Drawing.Size(759, 488);
            this.pnlNoSaves.TabIndex = 7;
            this.pnlNoSaves.Visible = false;
            // 
            // lblNoSaves
            // 
            this.lblNoSaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNoSaves.BackColor = System.Drawing.Color.Transparent;
            this.lblNoSaves.ForeColor = System.Drawing.Color.White;
            this.lblNoSaves.Location = new System.Drawing.Point(18, 188);
            this.lblNoSaves.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNoSaves.Name = "lblNoSaves";
            this.lblNoSaves.Size = new System.Drawing.Size(722, 30);
            this.lblNoSaves.TabIndex = 10;
            this.lblNoSaves.Text = "No hay ningun almacenamiento USB conectado...";
            this.lblNoSaves.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgServerGames
            // 
            this.dgServerGames.AllowUserToAddRows = false;
            this.dgServerGames.AllowUserToDeleteRows = false;
            this.dgServerGames.AllowUserToResizeRows = false;
            this.dgServerGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgServerGames.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgServerGames.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgServerGames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgServerGames.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Choose,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewTextBoxColumn5,
            this.Añadido});
            this.dgServerGames.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgServerGames.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgServerGames.Location = new System.Drawing.Point(0, 0);
            this.dgServerGames.Margin = new System.Windows.Forms.Padding(4);
            this.dgServerGames.Name = "dgServerGames";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgServerGames.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgServerGames.RowHeadersVisible = false;
            this.dgServerGames.RowHeadersWidth = 25;
            this.dgServerGames.RowTemplate.Height = 24;
            this.dgServerGames.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgServerGames.Size = new System.Drawing.Size(762, 488);
            this.dgServerGames.TabIndex = 12;
            this.dgServerGames.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgServerGames_CellClick);
            this.dgServerGames.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgServerGames_CellDoubleClick);
            // 
            // Choose
            // 
            this.Choose.Frozen = true;
            this.Choose.HeaderText = "Choose";
            this.Choose.MinimumWidth = 8;
            this.Choose.Name = "Choose";
            this.Choose.ReadOnly = true;
            this.Choose.Width = 20;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 20F;
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "Game Name";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 240;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "Cheats";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn3.HeaderText = "GameCode";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Client";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "Local Save Exist";
            this.dataGridViewCheckBoxColumn1.MinimumWidth = 8;
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewCheckBoxColumn1.Visible = false;
            this.dataGridViewCheckBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Client";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Visible = false;
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // Añadido
            // 
            this.Añadido.HeaderText = "Added";
            this.Añadido.MinimumWidth = 8;
            this.Añadido.Name = "Añadido";
            this.Añadido.ReadOnly = true;
            this.Añadido.Width = 150;
            // 
            // chkShowAll
            // 
            this.chkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowAll.Location = new System.Drawing.Point(622, 6);
            this.chkShowAll.Margin = new System.Windows.Forms.Padding(4);
            this.chkShowAll.Name = "chkShowAll";
            this.chkShowAll.Size = new System.Drawing.Size(146, 24);
            this.chkShowAll.TabIndex = 13;
            this.chkShowAll.Text = "Mostrar Todos";
            this.chkShowAll.UseVisualStyleBackColor = true;
            // 
            // btnResign
            // 
            this.btnResign.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnResign.FlatAppearance.BorderSize = 0;
            this.btnResign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResign.Location = new System.Drawing.Point(120, 0);
            this.btnResign.Margin = new System.Windows.Forms.Padding(4);
            this.btnResign.Name = "btnResign";
            this.btnResign.Size = new System.Drawing.Size(112, 34);
            this.btnResign.TabIndex = 9;
            this.btnResign.Text = "Mis Chetos";
            this.btnResign.UseVisualStyleBackColor = false;
            this.btnResign.Click += new System.EventHandler(this.btnResign_Click_1);
            // 
            // btnCheats
            // 
            this.btnCheats.BackColor = System.Drawing.Color.White;
            this.btnCheats.FlatAppearance.BorderSize = 0;
            this.btnCheats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheats.Location = new System.Drawing.Point(6, 0);
            this.btnCheats.Margin = new System.Windows.Forms.Padding(4);
            this.btnCheats.Name = "btnCheats";
            this.btnCheats.Size = new System.Drawing.Size(112, 34);
            this.btnCheats.TabIndex = 8;
            this.btnCheats.Text = "Chetos";
            this.btnCheats.UseVisualStyleBackColor = false;
            this.btnCheats.Click += new System.EventHandler(this.btnCheats_Click_1);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(656, -2);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(112, 34);
            this.btnImport.TabIndex = 16;
            this.btnImport.Text = "Importar";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnGamesInServer
            // 
            this.btnGamesInServer.Location = new System.Drawing.Point(0, 0);
            this.btnGamesInServer.Name = "btnGamesInServer";
            this.btnGamesInServer.Size = new System.Drawing.Size(75, 23);
            this.btnGamesInServer.TabIndex = 0;
            // 
            // pnlBackup
            // 
            this.pnlBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(74)))), ((int)(((byte)(20)))));
            this.pnlBackup.Controls.Add(this.gbManageProfile);
            this.pnlBackup.Controls.Add(this.groupBox2);
            this.pnlBackup.Controls.Add(this.groupBox1);
            this.pnlBackup.Controls.Add(this.gbBackupLocation);
            this.pnlBackup.Location = new System.Drawing.Point(386, 22);
            this.pnlBackup.Margin = new System.Windows.Forms.Padding(4);
            this.pnlBackup.Name = "pnlBackup";
            this.pnlBackup.Size = new System.Drawing.Size(762, 520);
            this.pnlBackup.TabIndex = 9;
            // 
            // gbManageProfile
            // 
            this.gbManageProfile.Controls.Add(this.gbProfiles);
            this.gbManageProfile.Controls.Add(this.lblManageProfiles);
            this.gbManageProfile.Controls.Add(this.btnManageProfiles);
            this.gbManageProfile.Location = new System.Drawing.Point(18, 405);
            this.gbManageProfile.Margin = new System.Windows.Forms.Padding(4);
            this.gbManageProfile.Name = "gbManageProfile";
            this.gbManageProfile.Padding = new System.Windows.Forms.Padding(4);
            this.gbManageProfile.Size = new System.Drawing.Size(724, 98);
            this.gbManageProfile.TabIndex = 10;
            this.gbManageProfile.TabStop = false;
            // 
            // gbProfiles
            // 
            this.gbProfiles.Location = new System.Drawing.Point(201, 45);
            this.gbProfiles.Margin = new System.Windows.Forms.Padding(4);
            this.gbProfiles.Name = "gbProfiles";
            this.gbProfiles.Padding = new System.Windows.Forms.Padding(4);
            this.gbProfiles.Size = new System.Drawing.Size(120, 40);
            this.gbProfiles.TabIndex = 9;
            this.gbProfiles.TabStop = false;
            // 
            // lblManageProfiles
            // 
            this.lblManageProfiles.AutoSize = true;
            this.lblManageProfiles.ForeColor = System.Drawing.Color.White;
            this.lblManageProfiles.Location = new System.Drawing.Point(15, 22);
            this.lblManageProfiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblManageProfiles.Name = "lblManageProfiles";
            this.lblManageProfiles.Size = new System.Drawing.Size(61, 20);
            this.lblManageProfiles.TabIndex = 8;
            this.lblManageProfiles.Text = "Perfiles";
            // 
            // btnManageProfiles
            // 
            this.btnManageProfiles.AutoSize = true;
            this.btnManageProfiles.ForeColor = System.Drawing.Color.White;
            this.btnManageProfiles.Location = new System.Drawing.Point(15, 50);
            this.btnManageProfiles.Margin = new System.Windows.Forms.Padding(4);
            this.btnManageProfiles.Name = "btnManageProfiles";
            this.btnManageProfiles.Size = new System.Drawing.Size(200, 45);
            this.btnManageProfiles.TabIndex = 0;
            this.btnManageProfiles.Text = "Perfiles";
            this.btnManageProfiles.UseVisualStyleBackColor = false;
            this.btnManageProfiles.Click += new System.EventHandler(this.btnManageProfiles_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbLanguage);
            this.groupBox2.Controls.Add(this.lblLanguage);
            this.groupBox2.Controls.Add(this.lblDeactivate);
            this.groupBox2.Controls.Add(this.btnDeactivate);
            this.groupBox2.Location = new System.Drawing.Point(18, 300);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(724, 98);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // cbLanguage
            // 
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Location = new System.Drawing.Point(502, 54);
            this.cbLanguage.Margin = new System.Windows.Forms.Padding(4);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(211, 28);
            this.cbLanguage.TabIndex = 10;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.BackColor = System.Drawing.Color.Transparent;
            this.lblLanguage.ForeColor = System.Drawing.Color.White;
            this.lblLanguage.Location = new System.Drawing.Point(498, 24);
            this.lblLanguage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(75, 20);
            this.lblLanguage.TabIndex = 9;
            this.lblLanguage.Text = "Languaje";
            // 
            // lblDeactivate
            // 
            this.lblDeactivate.AutoSize = true;
            this.lblDeactivate.ForeColor = System.Drawing.Color.White;
            this.lblDeactivate.Location = new System.Drawing.Point(15, 22);
            this.lblDeactivate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDeactivate.Name = "lblDeactivate";
            this.lblDeactivate.Size = new System.Drawing.Size(61, 20);
            this.lblDeactivate.TabIndex = 8;
            this.lblDeactivate.Text = "Testing";
            // 
            // btnDeactivate
            // 
            this.btnDeactivate.AutoSize = true;
            this.btnDeactivate.ForeColor = System.Drawing.Color.White;
            this.btnDeactivate.Location = new System.Drawing.Point(15, 52);
            this.btnDeactivate.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeactivate.Name = "btnDeactivate";
            this.btnDeactivate.Size = new System.Drawing.Size(172, 45);
            this.btnDeactivate.TabIndex = 0;
            this.btnDeactivate.Text = "Deactivar";
            this.btnDeactivate.UseVisualStyleBackColor = false;
            this.btnDeactivate.Click += new System.EventHandler(this.btnDeactivate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblRSSSection);
            this.groupBox1.Controls.Add(this.btnRss);
            this.groupBox1.Location = new System.Drawing.Point(18, 192);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(724, 100);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // lblRSSSection
            // 
            this.lblRSSSection.AutoSize = true;
            this.lblRSSSection.ForeColor = System.Drawing.Color.White;
            this.lblRSSSection.Location = new System.Drawing.Point(15, 22);
            this.lblRSSSection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRSSSection.Name = "lblRSSSection";
            this.lblRSSSection.Size = new System.Drawing.Size(433, 20);
            this.lblRSSSection.TabIndex = 6;
            this.lblRSSSection.Text = "Select below button to check the availability of latest version.";
            // 
            // btnRss
            // 
            this.btnRss.ForeColor = System.Drawing.Color.White;
            this.btnRss.Location = new System.Drawing.Point(15, 56);
            this.btnRss.Margin = new System.Windows.Forms.Padding(4);
            this.btnRss.Name = "btnRss";
            this.btnRss.Size = new System.Drawing.Size(172, 34);
            this.btnRss.TabIndex = 0;
            this.btnRss.Text = "Actualizar";
            this.btnRss.UseVisualStyleBackColor = false;
            this.btnRss.Click += new System.EventHandler(this.btnRss_Click);
            // 
            // gbBackupLocation
            // 
            this.gbBackupLocation.Controls.Add(this.btnOpenFolder);
            this.gbBackupLocation.Controls.Add(this.lblBackup);
            this.gbBackupLocation.Controls.Add(this.btnBrowse);
            this.gbBackupLocation.Controls.Add(this.txtBackupLocation);
            this.gbBackupLocation.Controls.Add(this.chkBackup);
            this.gbBackupLocation.Controls.Add(this.btnApply);
            this.gbBackupLocation.ForeColor = System.Drawing.Color.White;
            this.gbBackupLocation.Location = new System.Drawing.Point(18, 12);
            this.gbBackupLocation.Margin = new System.Windows.Forms.Padding(0);
            this.gbBackupLocation.Name = "gbBackupLocation";
            this.gbBackupLocation.Padding = new System.Windows.Forms.Padding(0);
            this.gbBackupLocation.Size = new System.Drawing.Size(724, 172);
            this.gbBackupLocation.TabIndex = 3;
            this.gbBackupLocation.TabStop = false;
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.Location = new System.Drawing.Point(16, 128);
            this.btnOpenFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(184, 34);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // lblBackup
            // 
            this.lblBackup.AutoSize = true;
            this.lblBackup.Location = new System.Drawing.Point(15, 60);
            this.lblBackup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackup.Name = "lblBackup";
            this.lblBackup.Size = new System.Drawing.Size(0, 20);
            this.lblBackup.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(422, 90);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(112, 34);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Examinar";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtBackupLocation
            // 
            this.txtBackupLocation.Location = new System.Drawing.Point(16, 92);
            this.txtBackupLocation.Margin = new System.Windows.Forms.Padding(4);
            this.txtBackupLocation.Name = "txtBackupLocation";
            this.txtBackupLocation.Size = new System.Drawing.Size(394, 26);
            this.txtBackupLocation.TabIndex = 0;
            // 
            // chkBackup
            // 
            this.chkBackup.AutoSize = true;
            this.chkBackup.ForeColor = System.Drawing.Color.White;
            this.chkBackup.Location = new System.Drawing.Point(15, 22);
            this.chkBackup.Margin = new System.Windows.Forms.Padding(4);
            this.chkBackup.Name = "chkBackup";
            this.chkBackup.Size = new System.Drawing.Size(162, 24);
            this.chkBackup.TabIndex = 0;
            this.chkBackup.Text = "Guardar los datos";
            this.chkBackup.UseVisualStyleBackColor = true;
            this.chkBackup.CheckedChanged += new System.EventHandler(this.chkBackup_CheckedChanged);
            this.chkBackup.Click += new System.EventHandler(this.chkBackup_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(16, 128);
            this.btnApply.Margin = new System.Windows.Forms.Padding(4);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(112, 34);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Aplicar";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // Multi
            // 
            this.Multi.FillWeight = 20F;
            this.Multi.Frozen = true;
            this.Multi.MinimumWidth = 8;
            this.Multi.Name = "Multi";
            this.Multi.ReadOnly = true;
            this.Multi.Width = 20;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackgroundImage = global::PS4SaveEditor.Properties.Resources.usb1;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.cbDrives);
            this.panel1.Location = new System.Drawing.Point(24, 498);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(354, 45);
            this.panel1.TabIndex = 11;
            // 
            // cbDrives
            // 
            this.cbDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrives.FormattingEnabled = true;
            this.cbDrives.IntegralHeight = false;
            this.cbDrives.Location = new System.Drawing.Point(284, 9);
            this.cbDrives.Margin = new System.Windows.Forms.Padding(4);
            this.cbDrives.Name = "cbDrives";
            this.cbDrives.Size = new System.Drawing.Size(64, 28);
            this.cbDrives.TabIndex = 3;
            // 
            // btnOptions
            // 
            this.btnOptions.BackColor = System.Drawing.Color.Transparent;
            this.btnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOptions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnOptions.FlatAppearance.BorderSize = 0;
            this.btnOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOptions.Image = global::PS4SaveEditor.Properties.Resources.setting0;
            this.btnOptions.Location = new System.Drawing.Point(22, 118);
            this.btnOptions.Margin = new System.Windows.Forms.Padding(4);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(356, 92);
            this.btnOptions.TabIndex = 5;
            this.btnOptions.UseVisualStyleBackColor = false;
            this.btnOptions.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHome.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Image = global::PS4SaveEditor.Properties.Resources.Game1;
            this.btnHome.Location = new System.Drawing.Point(22, 22);
            this.btnHome.Margin = new System.Windows.Forms.Padding(4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(356, 92);
            this.btnHome.TabIndex = 3;
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Image = global::PS4SaveEditor.Properties.Resources.Help0;
            this.btnHelp.Location = new System.Drawing.Point(22, 214);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(4);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(356, 92);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PS4SaveEditor.Properties.Resources.bg;
            this.pictureBox1.Location = new System.Drawing.Point(22, 322);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(356, 166);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(74)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1170, 566);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnlHome);
            this.Controls.Add(this.pnlBackup);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnHelp);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1159, 538);
            this.Name = "MainForm3";
            this.ShowIcon = false;
            this.Text = "PS4 Chetos Inc Save Special";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm3_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlHome.ResumeLayout(false);
            this.tabPageResign.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResign)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.tabPageGames.ResumeLayout(false);
            this.pnlNoSaves.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgServerGames)).EndInit();
            this.pnlBackup.ResumeLayout(false);
            this.gbManageProfile.ResumeLayout(false);
            this.gbManageProfile.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbBackupLocation.ResumeLayout(false);
            this.gbBackupLocation.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

    }

    private delegate void ClearDrivesDelegate();

    private delegate void AddItemDelegate(string item);

    private delegate void GetTrafficDelegate();

    public struct DEV_BROADCAST_HDR
    {
      public uint dbch_Size;
      public uint dbch_DeviceType;
      public uint dbch_Reserved;
    }

    public struct DEV_BROADCAST_VOLUME
    {
      public uint dbch_Size;
      public uint dbch_DeviceType;
      public uint dbch_Reserved;
      public uint dbcv_unitmask;
      public ushort dbcv_flags;
    }

        private void MainForm3_Load(object sender, EventArgs e)
        {

        }

        private void btnResign_Click_1(object sender, EventArgs e)
        {

        }

        private void btnCheats_Click_1(object sender, EventArgs e)
        {

        }

        private void dgResign_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlHome_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
