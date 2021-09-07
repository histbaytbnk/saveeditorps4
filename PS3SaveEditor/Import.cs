
// Type: PS3SaveEditor.Import


// Hacked by SystemAce

using CSUST.Data;
using Ionic.Zip;
using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class Import : Form
  {
    private Dictionary<string, object> m_accounts;
    private ZipFile m_zipFile;
    private string m_drive;
    private List<game> m_games;
    private Dictionary<string, object> m_psnIDs;
    private string m_expandedGame;
    private Dictionary<string, List<game>> m_map;
    private IContainer components;
    private Panel panel1;
    private Button btnCancel;
    private Button btnImport;
    private CustomDataGridView dgImport;
    private DataGridViewTextBoxColumn Select;
    private DataGridViewTextBoxColumn GameName;
    private DataGridViewTextBoxColumn SysVer;
    private DataGridViewTextBoxColumn Account;

    public Import(List<game> games, Dictionary<ZipEntry, ZipEntry> entries, ZipFile zipFile, Dictionary<string, object> accounts, string drive)
    {
      this.InitializeComponent();
      this.m_games = games;
      this.m_psnIDs = accounts;
      this.m_accounts = accounts;
      this.m_drive = drive;
      this.m_zipFile = zipFile;
      this.panel1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.PrepareMap(entries, games);
      this.FillSaves((string) null, false);
      this.dgImport.SelectionChanged += new EventHandler(this.dgImport_SelectionChanged);
      this.btnImport.Click += new EventHandler(this.btnImport_Click);
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.dgImport.CellDoubleClick += new DataGridViewCellEventHandler(this.dgImport_CellDoubleClick);
    }

    private void dgImport_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || this.dgImport.SelectedCells.Count == 0 || this.dgImport.SelectedCells[0].RowIndex < 0)
        return;
      this.FillSaves(this.dgImport.Rows[this.dgImport.SelectedCells[0].RowIndex].Cells[1].Value as string, this.dgImport.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending);
    }

    private void FillSaves(string expandGame, bool bSortedAsc)
    {
      if (this.m_expandedGame == expandGame)
      {
        expandGame = (string) null;
        this.m_expandedGame = (string) null;
      }
      this.dgImport.Rows.Clear();
      List<string> list1 = new List<string>();
      foreach (string str1 in this.m_map.Keys)
      {
        string id = str1;
        foreach (alias alias in (this.m_games.Find((Predicate<game>) (a => a.id == id)) ?? this.m_map[id][0]).GetAllAliases(bSortedAsc))
        {
          string str2 = alias.name + " (" + alias.id + ")";
          string index1 = alias.id;
          if (this.m_map.ContainsKey(alias.id))
          {
            List<game> list2 = this.m_map[index1];
            if (list1.IndexOf(index1) < 0)
            {
              list1.Add(index1);
              int index2 = this.dgImport.Rows.Add();
              this.dgImport.Rows[index2].Cells[1].Value = (object) alias.name;
              if (list2.Count == 0)
              {
                game game = list2[0];
                this.dgImport.Rows[index2].Tag = (object) game;
                container targetGameFolder = game.GetTargetGameFolder();
                if (targetGameFolder != null)
                  this.dgImport.Rows[index2].Cells[2].Value = (object) targetGameFolder.GetCheatsCount();
                else
                  this.dgImport.Rows[index2].Cells[2].Value = (object) "N/A";
                this.dgImport.Rows[index2].Cells[0].ToolTipText = "";
                this.dgImport.Rows[index2].Cells[0].Tag = (object) index1;
                string[] strArray = game.PFSZipEntry.FileName.Split('/');
                if (strArray.Length >= 2)
                  this.dgImport.Rows[index2].Cells[2].Value = (object) strArray[strArray.Length - 2];
              }
              else
              {
                DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                this.dgImport.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                {
                  Font = new Font("Arial", 7f)
                });
                this.dgImport.Rows[index2].Cells[0].Value = (object) "►";
                string str3 = this.dgImport.Rows[index2].Cells[1].Value as string;
                this.dgImport.Rows[index2].Cells[1].Value = string.IsNullOrEmpty(str3) ? (object) alias.id : (object) (str3 + " (" + alias.id + ")");
                dataGridViewCellStyle.BackColor = Color.White;
                this.dgImport.Rows[index2].Cells[0].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgImport.Rows[index2].Cells[1].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgImport.Rows[index2].Cells[2].Style.ApplyStyle(dataGridViewCellStyle);
                this.dgImport.Rows[index2].Tag = (object) list2;
                if (str2 == expandGame || alias.id == expandGame)
                {
                  this.dgImport.Rows[index2].Cells[0].Style.ApplyStyle(new DataGridViewCellStyle()
                  {
                    Font = new Font("Arial", 7f)
                  });
                  this.dgImport.Rows[index2].Cells[0].Value = (object) "▼";
                  this.dgImport.Rows[index2].Cells[0].ToolTipText = "";
                  this.dgImport.Rows[index2].Cells[1].Value = string.IsNullOrEmpty(str3) ? (object) alias.id : (object) (str3 + " (" + alias.id + ")");
                  this.dgImport.Rows[index2].Cells[0].Tag = (object) index1;
                  foreach (game game in list2)
                  {
                    container container = game.containers._containers[0];
                    if (container != null)
                    {
                      int index3 = this.dgImport.Rows.Add();
                      Match match = Regex.Match(Path.GetFileNameWithoutExtension(game.LocalSaveFolder), container.pfs);
                      if (container.name != null && match.Groups != null && match.Groups.Count > 1)
                        this.dgImport.Rows[index3].Cells[1].Value = (object) ("    " + container.name.Replace("${1}", match.Groups[1].Value));
                      else
                        this.dgImport.Rows[index3].Cells[1].Value = (object) ("    " + (container.name ?? Path.GetFileNameWithoutExtension(game.LocalSaveFolder)));
                      this.dgImport.Rows[index3].Cells[0].Tag = (object) index1;
                      game.name = alias.name;
                      this.dgImport.Rows[index3].Tag = (object) game;
                                            this.dgImport.Rows[index3].Cells[1].ToolTipText = Path.GetFileNameWithoutExtension(game.LocalSaveFolder);
                      Stream memoryStream = new MemoryStream();
                      game.BinZipEntry.Extract((Stream) memoryStream);
                      memoryStream.Close();
                      memoryStream.Dispose();

                    }
                  }
                  this.m_expandedGame = expandGame;
                }
              }
            }
          }
        }
      }
    }

    public bool IsValidPSNID(string psnId)
    {
      return this.m_psnIDs != null && this.m_psnIDs.ContainsKey(psnId);
    }

    private void PrepareMap(Dictionary<ZipEntry, ZipEntry> entries, List<game> games)
    {
      this.m_map = new Dictionary<string, List<game>>();
      foreach (ZipEntry index in entries.Keys)
      {
        string[] strArray1 = index.FileName.Split('/');
        if (strArray1.Length > 1 && strArray1[strArray1.Length - 2].StartsWith("CUSA"))
        {
          string[] strArray2 = index.FileName.Split('/');
          string saveId;
          int onlineSaveIndex = MainForm3.GetOnlineSaveIndex(games, index.FileName, out saveId);
          if (onlineSaveIndex < 0)
            saveId = strArray2[strArray2.Length - 2];
          if (!this.m_map.ContainsKey(saveId))
            this.m_map.Add(saveId, new List<game>());
          string str = strArray2[strArray1.Length - 2];
          ZipEntry zipEntry1 = index;
          ZipEntry zipEntry2 = entries[index];
          string directoryName = Path.GetDirectoryName(zipEntry1.FileName);
          game game = new game()
          {
            id = str,
            name = "",
            containers = new containers()
            {
              _containers = new List<container>()
              {
                new container()
                {
                  pfs = Path.GetFileName(index.FileName)
                }
              }
            },
            PFSZipEntry = zipEntry1,
            BinZipEntry = zipEntry2,
            ZipFile = this.m_zipFile,
            LocalSaveFolder = Path.Combine(directoryName, Path.GetFileName(zipEntry2.FileName))
          };
          if (onlineSaveIndex >= 0)
          {
            string name = games[onlineSaveIndex].name;
            game = game.Copy(this.m_games[onlineSaveIndex]);
            game.id = saveId;
            game.LocalSaveFolder = Path.Combine(directoryName, Path.GetFileName(zipEntry2.FileName));
            game.PFSZipEntry = zipEntry1;
            game.BinZipEntry = zipEntry2;
            game.ZipFile = this.m_zipFile;
          }
          this.m_map[saveId].Add(game);
        }
      }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      if (Util.GetRegistryValue("NoResignMessage") == null)
      {
        int num1 = (int) new ResignInfo().ShowDialog((IWin32Window) this);
      }
      ChooseProfile chooseProfile = new ChooseProfile(this.m_accounts, "");
      if (chooseProfile.ShowDialog() != DialogResult.OK)
        return;
      game game1 = this.dgImport.SelectedRows[0].Tag as game;
      ZipEntry pfsZipEntry = game1.PFSZipEntry;
      ZipEntry binZipEntry = game1.BinZipEntry;
      string id = game1.id;
      string str = this.m_drive + "\\PS4\\SAVEDATA\\" + chooseProfile.SelectedAccount + "\\" + id + "\\";
      game game2 = new game()
      {
        id = id,
        name = "",
        containers = new containers()
        {
          _containers = new List<container>()
          {
            new container()
            {
              pfs = this.dgImport.SelectedRows[0].Cells[1].Value as string
            }
          }
        },
        PFSZipEntry = pfsZipEntry,
        BinZipEntry = binZipEntry,
        ZipFile = this.m_zipFile,
        LocalSaveFolder = Path.Combine(str, Path.GetFileName(pfsZipEntry.FileName))
      };
      if (File.Exists(game2.LocalSaveFolder))
        return;
      if (string.IsNullOrEmpty(this.m_drive) || !Directory.Exists(Path.GetPathRoot(game2.LocalSaveFolder)))
      {

      }
      else
      {
        if (new ResignFilesUplaoder(game2, str, chooseProfile.SelectedAccount, new List<string>()).ShowDialog() == DialogResult.OK)
        {
          int num3 = (int) new ResignMessage().ShowDialog((IWin32Window) this);
        }
        this.dgImport.ClearSelection();
        this.DialogResult = DialogResult.OK;
      }
    }

    private void dgImport_SelectionChanged(object sender, EventArgs e)
    {
      this.btnImport.Enabled = this.dgImport.SelectedRows.Count == 1 && !string.IsNullOrEmpty(this.dgImport.SelectedRows[0].Cells[1].Value as string) && (this.dgImport.SelectedRows[0].Cells[1].Value as string).StartsWith("    ");
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
        return;
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0, 138, 213), Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.panel1 = new Panel();
      this.btnCancel = new Button();
      this.btnImport = new Button();
      this.dgImport = new CustomDataGridView();
      this.Select = new DataGridViewTextBoxColumn();
      this.GameName = new DataGridViewTextBoxColumn();
      this.SysVer = new DataGridViewTextBoxColumn();
      this.Account = new DataGridViewTextBoxColumn();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.panel1.Controls.Add((Control) this.btnCancel);
      this.panel1.Controls.Add((Control) this.btnImport);
      this.panel1.Controls.Add((Control) this.dgImport);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(10, 10);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new Padding(12);
      this.panel1.Size = new Size(580, 353);
      this.panel1.TabIndex = 1;
      this.btnCancel.Location = new Point(277, 322);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(75, 23);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnImport.Location = new Point(199, 322);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new Size(75, 23);
      this.btnImport.TabIndex = 2;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.dgImport.AllowUserToAddRows = false;
      this.dgImport.AllowUserToDeleteRows = false;
      this.dgImport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgImport.Columns.AddRange((DataGridViewColumn) this.Select, (DataGridViewColumn) this.GameName, (DataGridViewColumn) this.SysVer, (DataGridViewColumn) this.Account);
      this.dgImport.Location = new Point(12, 12);
      this.dgImport.MultiSelect = false;
      this.dgImport.Name = "dgImport";
      this.dgImport.ReadOnly = true;
      this.dgImport.RowHeadersVisible = false;
      this.dgImport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgImport.Size = new Size(555, 302);
      this.dgImport.TabIndex = 1;
      this.Select.HeaderText = "";
      this.Select.Name = "Select";
      this.Select.ReadOnly = true;
      this.Select.Width = 20;
      this.GameName.HeaderText = "Game Name";
      this.GameName.Name = "GameName";
      this.GameName.ReadOnly = true;
      this.GameName.Resizable = DataGridViewTriState.True;
      this.GameName.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.GameName.Width = 350;
      this.SysVer.HeaderText = "Sys. Ver";
      this.SysVer.Name = "SysVer";
      this.SysVer.ReadOnly = true;
      this.SysVer.Width = 80;
      this.Account.HeaderText = "Profile/PSN ID";
      this.Account.Name = "Account";
      this.Account.ReadOnly = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(600, 373);
      this.Controls.Add((Control) this.panel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Import";
      this.Padding = new Padding(10);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Import";
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
