
// Type: PS3SaveEditor.MainForm


// Hacked by SystemAce

using CSUST.Data;
using CustomControls;
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
  public class MainForm : Form
  {
    public static string USER_CHEATS_FILE = "swusercheats.xml";
    private Dictionary<string, List<game>> m_dictLocalSaves = new Dictionary<string, List<game>>();
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
    private Dictionary<int, string> RegionMap;
    private string m_hash;
    private CustomVScrollbar verticalScroller;
    private CustomHScrollbar horizontalScroller;
    private MainForm.ClearDrivesDelegate ClearDrivesFunc;
    private MainForm.AddItemDelegate AddItemFunc;
    private MainForm.GetTrafficDelegate GetTrafficFunc;
    private List<game> m_games;
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
    private CustomDataGridView dgServerGames;
    private ToolStripMenuItem deleteSaveToolStripMenuItem;
    private Button btnGamesInServer;
    private CheckBox chkShowAll;
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
    private DataGridViewTextBoxColumn Choose;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private Panel panel2;
    private CustomGroupBox gbBackupLocation;
    private CustomGroupBox groupBox1;
    private CustomGroupBox groupBox2;
    private CustomGroupBox gbManageProfile;
    private CustomGroupBox gbProfiles;
    private Panel panel3;
    private PictureBox picTraffic;
    private PictureBox pictureBox2;
    private PictureBox picVersion;
    private ComboBox cbLanguage;
    private Label lblLanguage;

    public MainForm()
    {
      this.m_games = new List<game>();
      this.InitializeComponent();
      this.RegionMap = new Dictionary<int, string>();
      this.ClearDrivesFunc = new MainForm.ClearDrivesDelegate(this.ClearDrives);
      this.AddItemFunc = new MainForm.AddItemDelegate(this.AddItem);
      this.chkShowAll.CheckedChanged += new EventHandler(this.chkShowAll_CheckedChanged);
      this.chkShowAll.EnabledChanged += new EventHandler(this.chkShowAll_EnabledChanged);
      this.picTraffic.Visible = false;
      this.ResizeBegin += (EventHandler) ((s, e) => this.SuspendLayout());
      this.ResizeEnd += (EventHandler) ((s, e) =>
      {
        this.ResumeLayout(true);
        this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
        this.Invalidate(false);
      });
      this.SizeChanged += (EventHandler) ((s, e) =>
      {
        if (this.WindowState != FormWindowState.Maximized)
          return;
        this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
        this.Invalidate(false);
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
      this.pnlBackup.BackColor = this.pnlHome.BackColor = this.pnlHome.BackColor = this.pnlNoSaves.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.gbBackupLocation.BackColor = this.gbManageProfile.BackColor = this.groupBox1.BackColor = this.groupBox2.BackColor = System.Drawing.Color.Transparent;
      this.chkShowAll.BackColor = System.Drawing.Color.FromArgb(0, 204, 204, 204);
      this.chkShowAll.ForeColor = System.Drawing.Color.White;
      this.panel2.Visible = false;
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
      this.panel2.BackgroundImage = (Image) null;
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
      this.cbDrives.DrawMode = DrawMode.OwnerDrawFixed;
      this.cbDrives.DrawItem += new DrawItemEventHandler(this.cbDrives_DrawItem);
      this.FillDrives();
      this.Load += new EventHandler(this.MainForm_Load);
      this.btnHome.ChangeUICues += new UICuesEventHandler(this.btnHome_ChangeUICues);
      this.dgServerGames.BackgroundColor = System.Drawing.Color.White;
    }

    private void btnHome_ChangeUICues(object sender, UICuesEventArgs e)
    {
      if (!e.ChangeFocus)
        return;
      this.btnHome.Focus();
    }

    private void chkShowAll_EnabledChanged(object sender, EventArgs e)
    {

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

        }

        private void MainForm_Resize(object sender, EventArgs e)
    {
      this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
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
      if (e.RowIndex < 0 || this.dgServerGames.SelectedCells.Count == 0 || this.dgServerGames.SelectedCells[0].RowIndex < 0)
        return;
      string toolTipText = this.dgServerGames.Rows[this.dgServerGames.SelectedCells[0].RowIndex].Cells[1].ToolTipText;
      int num = (int) MessageBox.Show(toolTipText);
    }

    private void chkShowAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkShowAll.Checked)
      {
        this.pnlNoSaves.Visible = false;
        this.pnlNoSaves.SendToBack();
        this.dgServerGames.Columns[3].Visible = false;
        this.ShowAllGames();
      }
      else
      {
        this.dgServerGames.Columns[0].Visible = true;
        this.dgServerGames.Columns[3].Visible = true;
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
      this.dgServerGames.Columns[1].Width = (int) ((double) width * 0.800000011920929);
      this.dgServerGames.Columns[2].Width = (int) ((double) width * 0.200000002980232);
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
          if (m.LParam != IntPtr.Zero && (int) ((MainForm.DEV_BROADCAST_HDR) Marshal.PtrToStructure(m.LParam, typeof (MainForm.DEV_BROADCAST_HDR))).dbch_DeviceType == 2)
          {
            MainForm.DEV_BROADCAST_VOLUME devBroadcastVolume = (MainForm.DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(m.LParam, typeof (MainForm.DEV_BROADCAST_VOLUME));
            for (int index = 0; index < 26; ++index)
            {
              if (((int) (devBroadcastVolume.dbcv_unitmask >> index) & 1) == 1)
                new Thread(new ParameterizedThreadStart(this.HandleDrive)).Start((object) string.Format("{0}:\\", (object) (char) (65 + index)));
            }
          }
        }
        else if (m.WParam.ToInt32() == 32772 && m.LParam != IntPtr.Zero && (int) ((MainForm.DEV_BROADCAST_HDR) Marshal.PtrToStructure(m.LParam, typeof (MainForm.DEV_BROADCAST_HDR))).dbch_DeviceType == 2)
        {
          MainForm.DEV_BROADCAST_VOLUME devBroadcastVolume = (MainForm.DEV_BROADCAST_VOLUME) Marshal.PtrToStructure(m.LParam, typeof (MainForm.DEV_BROADCAST_VOLUME));
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
            this.chkShowAll.Checked = false;
            this.chkShowAll.Enabled = false;
          }
          else
            this.cbDrives.SelectedIndex = 0;
        }
      }

      base.WndProc(ref m);
    }

    private void HandleDrive(object drive)
    {
      this.cbDrives.Invoke((Delegate) this.AddItemFunc, (object) (string) drive);
    }

    private int InitSession()
    {
      try
      {
        WebClientEx webClientEx = new WebClientEx();
        webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
        string uid = Util.GetUID(false, false);
        if (string.IsNullOrEmpty(uid))
        {
          RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Util.GetRegistryBase(), true);
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
      catch (Exception ex)
      {
        if (ex is WebException)
          return -1;
      }
      return 0;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      IntPtr systemMenu = MainForm.GetSystemMenu(this.Handle, false);
      MainForm.InsertMenu(systemMenu, 5, 3072, 0, string.Empty);
      MainForm.InsertMenu(systemMenu, 6, 1024, 1000, "About Save Wizard for PS4 MAX...");
            
      {
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
          GameListDownloader gameListDownloader = new GameListDownloader();
          if (gameListDownloader.ShowDialog() == DialogResult.OK)
          {
            int count = this.m_psnIDs.Count;
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
              this.chkShowAll.Checked = true;
              this.chkShowAll.Enabled = false;
            }
            else
            {
              this.PrepareLocalSavesMap();
              this.FillLocalSaves((string) null, true);
              this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            }
            this.btnHome_Click((object) null, (EventArgs) null);
          }
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

label_7:
      while (!this.evt.WaitOne((num - 10) * 1000))
      {
        while (true)
        {
          try
          {

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
      if (this.cbDrives.SelectedItem == null)
        return;
      string str = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str + "PS4\\SAVEDATA"))
        return;
      string[] directories = Directory.GetDirectories(str + "PS4\\SAVEDATA");
      List<string> list = new List<string>();
      foreach (string path1 in directories)
      {
        long result;
        if (long.TryParse(Path.GetFileName(path1), NumberStyles.HexNumber, (IFormatProvider) null, out result))
        {
          foreach (string path2 in Directory.GetDirectories(path1))
            list.AddRange((IEnumerable<string>) Directory.GetFiles(path2, "*.bin"));
        }
      }
      string[] array = list.ToArray();
      Array.Sort<string>(array);
      foreach (string save in array)
      {
        string saveId;
        int onlineSaveIndex = this.GetOnlineSaveIndex(save, out saveId);
        if (onlineSaveIndex >= 0)
        {
          this.dgServerGames.Rows.Add();
          game game = game.Copy(this.m_games[onlineSaveIndex]);
          game.id = saveId;
          game.LocalCheatExists = true;
          game.LocalSaveFolder = save;
          if (game.GetTargetGameFolder() == null)
            game.LocalCheatExists = false;
          try
          {
            this.FillLocalCheats(ref game);
          }
          catch (Exception ex)
          {
          }
          if (!this.m_dictLocalSaves.ContainsKey(game.id))
            this.m_dictLocalSaves.Add(game.id, new List<game>()
            {
              game
            });
          else
            this.m_dictLocalSaves[game.id].Add(game);
        }
      }
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
                      if (match.Groups != null && match.Groups.Count > 1)
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
        string str = Convert.ToBase64String(MainForm.GetParamInfo(sfoPath, out profileId));
        string key1 = profileId.ToString() + ":" + str + ":" + Convert.ToBase64String(Util.GetPSNId(Path.GetDirectoryName(sfoPath)));
        if (mapProfiles.ContainsKey(key1))
          return mapProfiles[key1];
        string key2 = profileId.ToString() + ":" + str;
        if (mapProfiles.ContainsKey(key2))
          return mapProfiles[key2];
      }
      return "";
    }


    private void SetLabels()
    {
      this.picTraffic.BackgroundImageLayout = ImageLayout.None;
      this.picVersion.BackgroundImageLayout = ImageLayout.None;
      this.picVersion.Visible = false;
      this.pictureBox2.BackgroundImageLayout = ImageLayout.None;
      this.dgServerGames.Columns[3].Visible = false;
      this.Text = Util.PRODUCT_NAME;
      MainForm mainForm = this;
      string str = mainForm.Text + " - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
      mainForm.Text = str;
      this.panel3.BackgroundImageLayout = ImageLayout.Tile;
    }

    private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      CultureInfo cultureInfo = this.cbLanguage.SelectedItem as CultureInfo;
      Util.SetRegistryValue("Language", cultureInfo.Name);
      Thread.CurrentThread.CurrentUICulture = cultureInfo;
      this.SetLabels();
    }

    private void FillLocalCheats(ref game item)
    {
      string str = Util.GetBackupLocation() + (object) Path.DirectorySeparatorChar + MainForm.USER_CHEATS_FILE;
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
        if (this.GetOnlineSaveIndex(str2, out saveId) == -1)
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
            return string.Compare(item1.diskcode, item2.diskcode);
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
        this.chkShowAll.Enabled = true;
        this.chkShowAll.Checked = false;
      }
      else
      {
        if (this.m_games.Count <= 0)
          return;
        this.chkShowAll.Checked = true;
        this.chkShowAll.Enabled = false;
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
          this.m_games = ((games) new XmlSerializer(typeof (games)).Deserialize((TextReader) stringReader))._games;
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
      SimpleEdit simpleEdit = new SimpleEdit(gameItem, this.chkShowAll.Checked, files);
      if (simpleEdit.ShowDialog() == DialogResult.OK)
      {
        this.dgServerGames.Rows[rowIndex].Tag = (object) simpleEdit.GameItem;
        this.dgServerGames.Rows[rowIndex].Cells[2].Value = (object) simpleEdit.GameItem.GetCheatCount();
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

      {
        int? locked = game.GetTargetGameFolder().locked;
        if ((locked.GetValueOrDefault() <= 0 ? 0 : (locked.HasValue ? 1 : 0)) != 0 )
          return;
        string str = game.LocalSaveFolder.Substring(0, game.LocalSaveFolder.Length - 4);
        game.ToString(new List<string>(), "decrypt");
        containerFiles.Remove(str);
        AdvancedSaveUploaderForEncrypt uploaderForEncrypt = new AdvancedSaveUploaderForEncrypt(containerFiles.ToArray(), game, (string) null, "decrypt");
        if (uploaderForEncrypt.ShowDialog() == DialogResult.Abort || uploaderForEncrypt.DecryptedSaveData == null || (uploaderForEncrypt.DecryptedSaveData.Count <= 0 || new AdvancedEdit(game, uploaderForEncrypt.DecryptedSaveData, uploaderForEncrypt.DependentSaveData).ShowDialog((IWin32Window) this) != DialogResult.OK))
          return;
        this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
      }
    }

    private void cbDrives_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.cbDrives.SelectedItem == null)
        return;
      this.dgServerGames.Columns[0].Width = 25;
      int width = this.dgServerGames.Width;
      this.dgServerGames.Columns[1].Width = (int) ((double) (width - 25) * 0.5);
      this.dgServerGames.Columns[2].Width = (int) ((double) (width - 25) * 0.25);
      this.dgServerGames.Columns[3].Visible = false;
      this.dgServerGames.Columns[4].Width = (int) ((double) (width - 25) * 0.25);
      this.dgServerGames.Columns[4].Visible = true;
      string str = this.cbDrives.SelectedItem.ToString();
      if (!Directory.Exists(str + "PS4//SAVEDATA") || Directory.GetDirectories(str + "PS4//SAVEDATA").Length == 0)
      {
        if (!this.chkShowAll.Enabled)
        {
          this.chkShowAll.Enabled = true;
          this.chkShowAll.Checked = false;
        }
        if (this.chkShowAll.Checked)
          return;
        this.pnlNoSaves.Visible = true;
        this.pnlNoSaves.BringToFront();
      }
      else if (!this.chkShowAll.Checked)
      {
        this.pnlNoSaves.Visible = false;
        this.pnlNoSaves.SendToBack();
        this.PrepareLocalSavesMap();
        this.FillLocalSaves((string) null, true);
        this.dgServerGames.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
      }
      else
        this.chkShowAll_CheckedChanged((object) null, (EventArgs) null);
    }

    private int GetOnlineSaveIndex(string save, out string saveId)
    {
      string fileName = Path.GetFileName(Path.GetDirectoryName(save));
      string withoutExtension = Path.GetFileNameWithoutExtension(save);
      for (int index1 = 0; index1 < this.m_games.Count; ++index1)
      {
        saveId = this.m_games[index1].id;
        if (fileName.Equals(this.m_games[index1].id) || this.m_games[index1].IsAlias(fileName, out saveId))
        {
          for (int index2 = 0; index2 < this.m_games[index1].containers._containers.Count; ++index2)
          {
            if (withoutExtension == this.m_games[index1].containers._containers[index2].pfs || Util.IsMatch(withoutExtension, this.m_games[index1].containers._containers[index2].pfs))
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
      return MainForm.GetParamInfo(sfoFile, "SUB_TITLE") + "\r\n" + MainForm.GetParamInfo(sfoFile, "DETAIL");
    }

    private string GetSaveTitle(string sfoFile)
    {
      return MainForm.GetParamInfo(sfoFile, "TITLE");
    }

    private void btnHome_Click(object sender, EventArgs e)
    {
      this.pnlHome.Visible = true;
      this.pnlBackup.Visible = false;
      this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
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
      System.IO.File.WriteAllText(Util.GetBackupLocation() + (object) Path.DirectorySeparatorChar + MainForm.USER_CHEATS_FILE, contents);
    }

    private bool CheckForVersion()
    {
      return true;
    }

    private void btnRss_Click(object sender, EventArgs e)
    {
      try
      {
        int num = (int) new RSSForm(RssFeed.Read(string.Format("{0}/ps4/rss?token={1}", (object) Util.GetBaseUrl(), (object) Util.GetAuthToken())).Channels[0]).ShowDialog();
      }
      catch (Exception ex)
      {
      }
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

      this.m_sessionInited = false;
      Application.Restart();
    }

    private bool DeactivateLicense()
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
        FileName = "http://www.savewizard.net/manuals/swps4m/"
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

    private void chkBackup_Click(object sender, EventArgs e)
    {
      if (this.chkBackup.Checked)
        return;
      this.chkBackup.Checked = true;
    }

    private void btnManageProfiles_Click(object sender, EventArgs e)
    {
      int num = (int) new ManageProfiles("", this.m_psnIDs).ShowDialog();
      this.GetPSNIDInfo();
      this.cbDrives_SelectedIndexChanged((object) null, (EventArgs) null);
    }

    private void registerPSNIDToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.m_psnIDs.Count >= this.m_psn_quota || this.m_psn_remaining <= 0)
      {
      }
      else
      {
        if (this.dgServerGames.SelectedRows.Count != 1 || new ManageProfiles((this.dgServerGames.SelectedRows[0].Tag as game).PSN_ID, this.m_psnIDs).ShowDialog() != DialogResult.OK)
          return;
        this.GetPSNIDInfo();
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



    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.simpleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerPSNIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreFromBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.pnlHome = new System.Windows.Forms.Panel();
            this.chkShowAll = new System.Windows.Forms.CheckBox();
            this.dgServerGames = new CSUST.Data.CustomDataGridView();
            this.Choose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlNoSaves = new System.Windows.Forms.Panel();
            this.lblNoSaves = new System.Windows.Forms.Label();
            this.btnGamesInServer = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbDrives = new System.Windows.Forms.ComboBox();
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.picVersion = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.picTraffic = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            this.pnlHome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgServerGames)).BeginInit();
            this.pnlNoSaves.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlBackup.SuspendLayout();
            this.gbManageProfile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbBackupLocation.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTraffic)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(185, 148);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // simpleToolStripMenuItem
            // 
            this.simpleToolStripMenuItem.Name = "simpleToolStripMenuItem";
            this.simpleToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.simpleToolStripMenuItem.Text = "Simple";
            this.simpleToolStripMenuItem.Click += new System.EventHandler(this.simpleToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            this.advancedToolStripMenuItem.Click += new System.EventHandler(this.advancedToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // resignToolStripMenuItem
            // 
            this.resignToolStripMenuItem.Name = "resignToolStripMenuItem";
            this.resignToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.resignToolStripMenuItem.Text = "Re-sign...";
            this.resignToolStripMenuItem.Click += new System.EventHandler(this.resignToolStripMenuItem_Click);
            // 
            // registerPSNIDToolStripMenuItem
            // 
            this.registerPSNIDToolStripMenuItem.Name = "registerPSNIDToolStripMenuItem";
            this.registerPSNIDToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.registerPSNIDToolStripMenuItem.Text = "Register PSN ID...";
            this.registerPSNIDToolStripMenuItem.Click += new System.EventHandler(this.registerPSNIDToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // restoreFromBackupToolStripMenuItem
            // 
            this.restoreFromBackupToolStripMenuItem.Name = "restoreFromBackupToolStripMenuItem";
            this.restoreFromBackupToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.restoreFromBackupToolStripMenuItem.Text = "Restore from Backup";
            this.restoreFromBackupToolStripMenuItem.Click += new System.EventHandler(this.restoreFromBackupToolStripMenuItem_Click);
            // 
            // deleteSaveToolStripMenuItem
            // 
            this.deleteSaveToolStripMenuItem.Name = "deleteSaveToolStripMenuItem";
            this.deleteSaveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.deleteSaveToolStripMenuItem.Text = "Delete Save";
            this.deleteSaveToolStripMenuItem.Click += new System.EventHandler(this.deleteSaveToolStripMenuItem_Click);
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHome.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Location = new System.Drawing.Point(15, 15);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(237, 61);
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
            this.btnHelp.Location = new System.Drawing.Point(15, 143);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(237, 61);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.BackColor = System.Drawing.Color.Transparent;
            this.btnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOptions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.btnOptions.FlatAppearance.BorderSize = 0;
            this.btnOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOptions.Location = new System.Drawing.Point(15, 79);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(237, 61);
            this.btnOptions.TabIndex = 5;
            this.btnOptions.UseVisualStyleBackColor = false;
            this.btnOptions.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // pnlHome
            // 
            this.pnlHome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.pnlHome.Controls.Add(this.chkShowAll);
            this.pnlHome.Controls.Add(this.dgServerGames);
            this.pnlHome.Controls.Add(this.pnlNoSaves);
            this.pnlHome.Location = new System.Drawing.Point(257, 15);
            this.pnlHome.Name = "pnlHome";
            this.pnlHome.Size = new System.Drawing.Size(508, 347);
            this.pnlHome.TabIndex = 8;
            // 
            // chkShowAll
            // 
            this.chkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowAll.Location = new System.Drawing.Point(11, 324);
            this.chkShowAll.Name = "chkShowAll";
            this.chkShowAll.Size = new System.Drawing.Size(97, 16);
            this.chkShowAll.TabIndex = 11;
            this.chkShowAll.Text = "Show All";
            this.chkShowAll.UseVisualStyleBackColor = true;
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
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgServerGames.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgServerGames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgServerGames.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Choose,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewTextBoxColumn5});
            this.dgServerGames.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgServerGames.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgServerGames.Location = new System.Drawing.Point(12, 12);
            this.dgServerGames.Name = "dgServerGames";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgServerGames.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgServerGames.RowHeadersVisible = false;
            this.dgServerGames.RowHeadersWidth = 25;
            this.dgServerGames.RowTemplate.Height = 24;
            this.dgServerGames.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgServerGames.Size = new System.Drawing.Size(484, 304);
            this.dgServerGames.TabIndex = 1;
            // 
            // Choose
            // 
            this.Choose.Frozen = true;
            this.Choose.HeaderText = "Choose";
            this.Choose.Name = "Choose";
            this.Choose.ReadOnly = true;
            this.Choose.Width = 20;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 20F;
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "Game Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 240;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "Cheats";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn3.HeaderText = "GameCode";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Client";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "Local Save Exist";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewCheckBoxColumn1.Visible = false;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Client";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Visible = false;
            // 
            // pnlNoSaves
            // 
            this.pnlNoSaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNoSaves.Controls.Add(this.lblNoSaves);
            this.pnlNoSaves.Location = new System.Drawing.Point(12, 12);
            this.pnlNoSaves.Name = "pnlNoSaves";
            this.pnlNoSaves.Size = new System.Drawing.Size(485, 311);
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
            this.lblNoSaves.Location = new System.Drawing.Point(0, 100);
            this.lblNoSaves.Name = "lblNoSaves";
            this.lblNoSaves.Size = new System.Drawing.Size(480, 13);
            this.lblNoSaves.TabIndex = 10;
            this.lblNoSaves.Text = "No PS4 saves were found on this USB drive.";
            this.lblNoSaves.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnGamesInServer
            // 
            this.btnGamesInServer.Location = new System.Drawing.Point(0, 0);
            this.btnGamesInServer.Name = "btnGamesInServer";
            this.btnGamesInServer.Size = new System.Drawing.Size(75, 23);
            this.btnGamesInServer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.cbDrives);
            this.panel1.Location = new System.Drawing.Point(15, 332);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(237, 30);
            this.panel1.TabIndex = 11;
            // 
            // cbDrives
            // 
            this.cbDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrives.FormattingEnabled = true;
            this.cbDrives.IntegralHeight = false;
            this.cbDrives.Location = new System.Drawing.Point(190, 6);
            this.cbDrives.Name = "cbDrives";
            this.cbDrives.Size = new System.Drawing.Size(40, 21);
            this.cbDrives.TabIndex = 3;
            // 
            // pnlBackup
            // 
            this.pnlBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.pnlBackup.Controls.Add(this.gbManageProfile);
            this.pnlBackup.Controls.Add(this.groupBox2);
            this.pnlBackup.Controls.Add(this.groupBox1);
            this.pnlBackup.Controls.Add(this.gbBackupLocation);
            this.pnlBackup.Location = new System.Drawing.Point(257, 15);
            this.pnlBackup.Name = "pnlBackup";
            this.pnlBackup.Size = new System.Drawing.Size(508, 347);
            this.pnlBackup.TabIndex = 9;
            // 
            // gbManageProfile
            // 
            this.gbManageProfile.Controls.Add(this.gbProfiles);
            this.gbManageProfile.Controls.Add(this.lblManageProfiles);
            this.gbManageProfile.Controls.Add(this.btnManageProfiles);
            this.gbManageProfile.Location = new System.Drawing.Point(12, 270);
            this.gbManageProfile.Name = "gbManageProfile";
            this.gbManageProfile.Size = new System.Drawing.Size(483, 65);
            this.gbManageProfile.TabIndex = 10;
            this.gbManageProfile.TabStop = false;
            // 
            // gbProfiles
            // 
            this.gbProfiles.Location = new System.Drawing.Point(134, 30);
            this.gbProfiles.Name = "gbProfiles";
            this.gbProfiles.Size = new System.Drawing.Size(80, 27);
            this.gbProfiles.TabIndex = 9;
            this.gbProfiles.TabStop = false;
            // 
            // lblManageProfiles
            // 
            this.lblManageProfiles.AutoSize = true;
            this.lblManageProfiles.ForeColor = System.Drawing.Color.White;
            this.lblManageProfiles.Location = new System.Drawing.Point(10, 15);
            this.lblManageProfiles.Name = "lblManageProfiles";
            this.lblManageProfiles.Size = new System.Drawing.Size(106, 13);
            this.lblManageProfiles.TabIndex = 8;
            this.lblManageProfiles.Text = "Manage PS4 Profiles";
            // 
            // btnManageProfiles
            // 
            this.btnManageProfiles.AutoSize = true;
            this.btnManageProfiles.ForeColor = System.Drawing.Color.White;
            this.btnManageProfiles.Location = new System.Drawing.Point(10, 33);
            this.btnManageProfiles.Name = "btnManageProfiles";
            this.btnManageProfiles.Size = new System.Drawing.Size(115, 23);
            this.btnManageProfiles.TabIndex = 0;
            this.btnManageProfiles.Text = "Manage Profiles";
            this.btnManageProfiles.UseVisualStyleBackColor = false;
            this.btnManageProfiles.Click += new System.EventHandler(this.btnManageProfiles_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbLanguage);
            this.groupBox2.Controls.Add(this.lblLanguage);
            this.groupBox2.Controls.Add(this.lblDeactivate);
            this.groupBox2.Controls.Add(this.btnDeactivate);
            this.groupBox2.Location = new System.Drawing.Point(12, 200);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(483, 65);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // cbLanguage
            // 
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Location = new System.Drawing.Point(335, 36);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(142, 21);
            this.cbLanguage.TabIndex = 10;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.BackColor = System.Drawing.Color.Transparent;
            this.lblLanguage.ForeColor = System.Drawing.Color.White;
            this.lblLanguage.Location = new System.Drawing.Point(332, 16);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(55, 13);
            this.lblLanguage.TabIndex = 9;
            this.lblLanguage.Text = "Language";
            // 
            // lblDeactivate
            // 
            this.lblDeactivate.AutoSize = true;
            this.lblDeactivate.ForeColor = System.Drawing.Color.White;
            this.lblDeactivate.Location = new System.Drawing.Point(10, 15);
            this.lblDeactivate.Name = "lblDeactivate";
            this.lblDeactivate.Size = new System.Drawing.Size(42, 13);
            this.lblDeactivate.TabIndex = 8;
            this.lblDeactivate.Text = "Testing";
            // 
            // btnDeactivate
            // 
            this.btnDeactivate.AutoSize = true;
            this.btnDeactivate.ForeColor = System.Drawing.Color.White;
            this.btnDeactivate.Location = new System.Drawing.Point(10, 35);
            this.btnDeactivate.Name = "btnDeactivate";
            this.btnDeactivate.Size = new System.Drawing.Size(115, 23);
            this.btnDeactivate.TabIndex = 0;
            this.btnDeactivate.Text = "Deactivate";
            this.btnDeactivate.UseVisualStyleBackColor = false;
            this.btnDeactivate.Click += new System.EventHandler(this.btnDeactivate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblRSSSection);
            this.groupBox1.Controls.Add(this.btnRss);
            this.groupBox1.Location = new System.Drawing.Point(12, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(483, 67);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // lblRSSSection
            // 
            this.lblRSSSection.AutoSize = true;
            this.lblRSSSection.ForeColor = System.Drawing.Color.White;
            this.lblRSSSection.Location = new System.Drawing.Point(10, 15);
            this.lblRSSSection.Name = "lblRSSSection";
            this.lblRSSSection.Size = new System.Drawing.Size(295, 13);
            this.lblRSSSection.TabIndex = 6;
            this.lblRSSSection.Text = "Select below button to check the availability of latest version.";
            // 
            // btnRss
            // 
            this.btnRss.ForeColor = System.Drawing.Color.White;
            this.btnRss.Location = new System.Drawing.Point(10, 37);
            this.btnRss.Name = "btnRss";
            this.btnRss.Size = new System.Drawing.Size(115, 23);
            this.btnRss.TabIndex = 0;
            this.btnRss.Text = "Update";
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
            this.gbBackupLocation.Location = new System.Drawing.Point(12, 8);
            this.gbBackupLocation.Margin = new System.Windows.Forms.Padding(0);
            this.gbBackupLocation.Name = "gbBackupLocation";
            this.gbBackupLocation.Padding = new System.Windows.Forms.Padding(0);
            this.gbBackupLocation.Size = new System.Drawing.Size(483, 115);
            this.gbBackupLocation.TabIndex = 3;
            this.gbBackupLocation.TabStop = false;
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.Location = new System.Drawing.Point(11, 85);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(123, 23);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // lblBackup
            // 
            this.lblBackup.AutoSize = true;
            this.lblBackup.Location = new System.Drawing.Point(10, 40);
            this.lblBackup.Name = "lblBackup";
            this.lblBackup.Size = new System.Drawing.Size(0, 13);
            this.lblBackup.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(281, 60);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtBackupLocation
            // 
            this.txtBackupLocation.Location = new System.Drawing.Point(11, 61);
            this.txtBackupLocation.Name = "txtBackupLocation";
            this.txtBackupLocation.Size = new System.Drawing.Size(264, 20);
            this.txtBackupLocation.TabIndex = 0;
            // 
            // chkBackup
            // 
            this.chkBackup.AutoSize = true;
            this.chkBackup.ForeColor = System.Drawing.Color.White;
            this.chkBackup.Location = new System.Drawing.Point(10, 15);
            this.chkBackup.Name = "chkBackup";
            this.chkBackup.Size = new System.Drawing.Size(96, 17);
            this.chkBackup.TabIndex = 0;
            this.chkBackup.Text = "Backup Saves";
            this.chkBackup.UseVisualStyleBackColor = true;
            this.chkBackup.CheckedChanged += new System.EventHandler(this.chkBackup_CheckedChanged);
            this.chkBackup.Click += new System.EventHandler(this.chkBackup_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(11, 85);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // Multi
            // 
            this.Multi.FillWeight = 20F;
            this.Multi.Frozen = true;
            this.Multi.Name = "Multi";
            this.Multi.ReadOnly = true;
            this.Multi.Width = 20;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.panel2.Location = new System.Drawing.Point(15, 215);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(237, 3);
            this.panel2.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(56)))), ((int)(((byte)(115)))));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Controls.Add(this.picVersion);
            this.panel3.Controls.Add(this.pictureBox2);
            this.panel3.Controls.Add(this.picTraffic);
            this.panel3.Location = new System.Drawing.Point(15, 207);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(237, 122);
            this.panel3.TabIndex = 13;
            // 
            // picVersion
            // 
            this.picVersion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picVersion.Location = new System.Drawing.Point(0, 23);
            this.picVersion.Name = "picVersion";
            this.picVersion.Size = new System.Drawing.Size(237, 26);
            this.picVersion.TabIndex = 13;
            this.picVersion.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.Location = new System.Drawing.Point(0, 78);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(237, 45);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // picTraffic
            // 
            this.picTraffic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picTraffic.Location = new System.Drawing.Point(0, 0);
            this.picTraffic.Name = "picTraffic";
            this.picTraffic.Size = new System.Drawing.Size(237, 26);
            this.picTraffic.TabIndex = 11;
            this.picTraffic.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(44)))), ((int)(((byte)(101)))));
            this.ClientSize = new System.Drawing.Size(780, 377);
            this.Controls.Add(this.pnlHome);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnlBackup);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnHelp);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(780, 377);
            this.Name = "MainForm";
            this.Text = "PS4 Save Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlHome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgServerGames)).EndInit();
            this.pnlNoSaves.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pnlBackup.ResumeLayout(false);
            this.gbManageProfile.ResumeLayout(false);
            this.gbManageProfile.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbBackupLocation.ResumeLayout(false);
            this.gbBackupLocation.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTraffic)).EndInit();
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
    }
}
