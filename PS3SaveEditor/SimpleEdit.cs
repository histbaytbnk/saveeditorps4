
// Type: PS3SaveEditor.SimpleEdit


// Hacked by SystemAce

using CSUST.Data;
using Microsoft.Win32;
using PS3SaveEditor.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace PS3SaveEditor
{
  public class SimpleEdit : Form
  {
    private game m_game;
    private bool m_bCheatsModified;
    private bool m_bShowOnly;
    private List<string> m_gameFiles;
    private IContainer components;
    private Label lblGameName;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem addCodeToolStripMenuItem;
    private ToolStripMenuItem editCodeToolStripMenuItem;
    private ToolStripMenuItem deleteCodeToolStripMenuItem;
    private Panel panel1;
    private Button btnApplyCodes;
    private DataGridViewTextBoxColumn Location;
    private DataGridViewTextBoxColumn Value;
    private Label label1;
    private Button btnClose;
    private Button btnApply;
    private CustomDataGridView dgCheatCodes;
    private CustomDataGridView dgCheats;
    private ComboBox cbProfile;
    private Label lblProfile;
    private DataGridViewCheckBoxColumn Select;
    private DataGridViewTextBoxColumn Description;
    private DataGridViewTextBoxColumn Comment;

    public game GameItem
    {
      get
      {
        return this.m_game;
      }
    }

    public SimpleEdit(game gameItem, bool bShowOnly, List<string> files = null)
    {
      this.m_bShowOnly = bShowOnly;
      this.m_game = game.Copy(gameItem);
      this.m_gameFiles = files;
      this.InitializeComponent();
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      this.btnApply.Enabled = false;
      this.btnApply.BackColor = SystemColors.ButtonFace;
      this.btnApply.ForeColor = System.Drawing.Color.Black;
      this.btnClose.BackColor = SystemColors.ButtonFace;
      this.btnClose.ForeColor = System.Drawing.Color.Black;
      this.panel1.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.lblProfile.Visible = false;
      this.cbProfile.Visible = false;
      this.label1.Visible = false;
      this.dgCheatCodes.Visible = false;
      this.lblGameName.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.lblGameName.ForeColor = System.Drawing.Color.White;
      this.lblGameName.Visible = false;
      this.SetLabels();
      this.CenterToScreen();
      this.FillProfiles();
      this.lblGameName.Text = gameItem.name;
      this.dgCheats.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgCheats_CellMouseClick);
      this.dgCheats.CellMouseDown += new DataGridViewCellMouseEventHandler(this.dgCheats_CellMouseDown);
      this.dgCheats.CellValidated += new DataGridViewCellEventHandler(this.dgCheats_CellValidated);
      this.dgCheats.CellValueChanged += new DataGridViewCellEventHandler(this.dgCheats_CellValueChanged);
      this.dgCheats.CurrentCellDirtyStateChanged += new EventHandler(this.dgCheats_CurrentCellDirtyStateChanged);
      this.dgCheats.CellMouseUp += new DataGridViewCellMouseEventHandler(this.dgCheats_CellMouseUp);
      this.dgCheats.MouseDown += new MouseEventHandler(this.dgCheats_MouseDown);
      this.btnApply.Click += new EventHandler(this.btnApply_Click);
      this.btnApplyCodes.Click += new EventHandler(this.btnApplyCodes_Click);
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      this.Resize += new EventHandler(this.SimpleEdit_ResizeEnd);
      this.SimpleEdit_ResizeEnd((object) null, (EventArgs) null);
      this.FillCheats((string) null);
    }

    protected override void OnResizeBegin(EventArgs e)
    {
      this.SuspendLayout();
      base.OnResizeBegin(e);
    }

    protected override void OnResizeEnd(EventArgs e)
    {
      base.OnResizeEnd(e);
      this.ResumeLayout();
      this.SimpleEdit_ResizeEnd((object) null, (EventArgs) null);
    }

    private void dgCheats_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex != 0)
        return;
      this.dgCheats.EndEdit();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, System.Drawing.Color.FromArgb(0, 138, 213), System.Drawing.Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void dgCheats_MouseDown(object sender, MouseEventArgs e)
    {
      Point location = e.Location;
      if (this.dgCheats.HitTest(location.X, location.Y).Type == DataGridViewHitTestType.Cell)
        return;
      this.dgCheats.ClearSelection();
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

    private void dgCheats_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
      if (!this.dgCheats.IsCurrentCellDirty)
        return;
      this.dgCheats.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private bool ValidateOneGroup(string curChecked)
    {
      foreach (DataGridViewRow dataGridViewRow in (IEnumerable) this.dgCheats.Rows)
      {
        if ("one".Equals(dataGridViewRow.Tag) && dataGridViewRow.Cells[0].Value != null && ((bool) dataGridViewRow.Cells[0].Value && dataGridViewRow.Cells[1].Tag != curChecked))
          dataGridViewRow.Cells[0].Value = (object) false;
      }
      return true;
    }

    private void dgCheats_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      bool flag = false;
      if (e.ColumnIndex == 0)
      {
        foreach (DataGridViewRow dataGridViewRow in (IEnumerable) this.dgCheats.Rows)
        {
          if (dataGridViewRow.Cells[0].Value != null && (bool) dataGridViewRow.Cells[0].Value && ((string) dataGridViewRow.Cells[0].Tag != "GameFile" && (string) dataGridViewRow.Cells[0].Tag != "CheatGroup"))
          {
            flag = true;
            break;
          }
          dataGridViewRow.Cells[0].Value = (object) false;
        }
      }
      this.btnApply.Enabled = flag;
    }

    private void dgCheats_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex != 0 || this.m_game.GetTargetGameFolder() != null)
        return;
    }

    private void SimpleEdit_ResizeEnd(object sender, EventArgs e)
    {
      this.btnApply.Left = this.Width / 2 - this.btnApply.Width - 2;
      this.btnClose.Left = this.Width / 2 + 2;
      this.lblProfile.Left = this.btnApply.Left - this.cbProfile.Width - this.lblProfile.Width - 30;
      this.cbProfile.Left = this.lblProfile.Left + this.lblProfile.Width + 5;
      this.dgCheats.Columns[1].Width = (this.dgCheats.Width - 2 - this.dgCheats.Columns[0].Width) / 2;
      this.dgCheats.Columns[2].Width = (this.dgCheats.Width - 2 - this.dgCheats.Columns[0].Width) / 2;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (this.m_bCheatsModified)
        this.DialogResult = DialogResult.OK;
      else
        this.DialogResult = DialogResult.Cancel;
      base.OnClosing(e);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void SetLabels() { 
      this.dgCheats.Columns[0].HeaderText = "";
    }

    private void FillCheats(string highlight)
    {
      this.dgCheats.Rows.Clear();
      container targetGameFolder = this.m_game.GetTargetGameFolder();
      if (targetGameFolder != null)
      {
        this.Select.Visible = true;
        if (this.m_game.GetAllCheats().Count == 0)
        {
          int index = this.dgCheats.Rows.Add(new DataGridViewRow());
          DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
          dataGridViewCellStyle.ForeColor = System.Drawing.Color.Gray;
          this.dgCheats.Rows[index].Cells[0].Tag = (object) "NoCheats";
          dataGridViewCellStyle.Font = new Font(this.dgCheats.Font, FontStyle.Italic);
          this.dgCheats.Rows[index].Cells[1].Style.ApplyStyle(dataGridViewCellStyle);
        }
        if (targetGameFolder.preprocess == 1 && this.m_gameFiles != null && this.m_gameFiles.Count > 0)
        {
          container gameFolder = container.Copy(targetGameFolder);
          List<file> files = gameFolder.files._files;
          targetGameFolder.files._files = new List<file>();
          foreach (string file1 in this.m_gameFiles)
          {
            file gameFile = file.GetGameFile(gameFolder, this.m_game.LocalSaveFolder, file1);
            if (gameFile != null)
            {
              file file2 = file.Copy(gameFile);
              file2.filename = file1;
              targetGameFolder.files._files.Add(file2);
            }
          }
        }
        foreach (file file in targetGameFolder.files._files)
        {
          if (targetGameFolder.files._files.Count > 1)
          {
            int index = this.dgCheats.Rows.Add(new DataGridViewRow());
            this.dgCheats.Rows[index].Cells[1].Value = (object) file.VisibleFileName;
            this.dgCheats.Rows[index].Cells[2].Value = (object) "";
            this.dgCheats.Rows[index].Cells[1].Tag = (object) file.id;
            this.dgCheats.Rows[index].Cells[0].Tag = (object) "GameFile";
            this.dgCheats.Rows[index].Tag = (object) file.filename;
          }
          foreach (cheat cheat in file.cheats._cheats)
          {
            int index = this.dgCheats.Rows.Add(new DataGridViewRow());
            this.dgCheats.Rows[index].Cells[1].Value = (object) cheat.name;
            this.dgCheats.Rows[index].Cells[2].Value = (object) cheat.note;
            this.dgCheats.Rows[index].Cells[1].Tag = (object) cheat.id;
            this.dgCheats.Rows[index].Cells[0].Tag = (object) file.filename;
            if (cheat.id == "-1")
            {
              this.dgCheats.Rows[index].Tag = (object) "UserCheat";
              this.dgCheats.Rows[index].Cells[1].Tag = (object) cheat.code;
            }
          }
          foreach (group g in file.groups)
            this.FillGroupCheats(file, g, file.filename, 0);
        }
      }
      else if (this.m_bShowOnly)
      {
        this.Select.Visible = false;
        this.btnApply.Enabled = false;
        foreach (container container in this.m_game.containers._containers)
        {
          foreach (file file in container.files._files)
          {
            foreach (cheat cheat in file.cheats._cheats)
            {
              int index = this.dgCheats.Rows.Add();
              this.dgCheats.Rows[index].Cells[1].Value = (object) cheat.name;
              this.dgCheats.Rows[index].Cells[2].Value = (object) cheat.note;
            }
            foreach (group g in file.groups)
              this.FillGroupCheats(file, g, file.filename, 0);
          }
        }
      }
      this.RefreshValue();
    }

    private void FillFileCheats(container target, file file, string saveFile)
    {
      for (int index1 = 0; index1 < file.Cheats.Count; ++index1)
      {
        cheat cheat = file.Cheats[index1];
        int index2 = this.dgCheats.Rows.Add(new DataGridViewRow());
        this.dgCheats.Rows[index2].Cells[1].Value = (object) cheat.name;
        this.dgCheats.Rows[index2].Cells[2].Value = (object) cheat.note;
        if (cheat.id == "-1")
        {
          this.dgCheats.Rows[index2].Cells[1].Style.ApplyStyle(new DataGridViewCellStyle()
          {
            ForeColor = System.Drawing.Color.Blue
          });
          this.dgCheats.Rows[index2].Cells[0].Tag = (object) "UserCheat";
          this.dgCheats.Rows[index2].Cells[1].Tag = (object) Path.GetFileName(saveFile);
          this.dgCheats.Rows[index2].Tag = (object) file.GetParent(target);
        }
        else
        {
          this.dgCheats.Rows[index2].Cells[0].Tag = (object) saveFile;
          this.dgCheats.Rows[index2].Cells[1].Tag = (object) cheat.id;
        }
      }
      if (file.groups.Count <= 0)
        return;
      foreach (group g in file.groups)
        this.FillGroupCheats(file, g, saveFile, 0);
    }

    private void FillGroupCheats(file file, group g, string saveFile, int level)
    {
      int index1 = this.dgCheats.Rows.Add(new DataGridViewRow());
      this.dgCheats.Rows[index1].Cells[0].Tag = (object) "CheatGroup";
      if (level > 0)
        this.dgCheats.Rows[index1].Cells[1].Value = (object) (new string(' ', level * 4) + g.name);
      else
        this.dgCheats.Rows[index1].Cells[1].Value = (object) g.name;
      this.dgCheats.Rows[index1].Cells[2].Value = (object) g.note;
      this.dgCheats.Rows[index1].Cells[2].Value = (object) "";
      foreach (cheat cheat in g.cheats)
      {
        int index2 = this.dgCheats.Rows.Add(new DataGridViewRow());
        this.dgCheats.Rows[index2].Cells[1].Value = (object) (new string(' ', (level + 1) * 4) + cheat.name);
        this.dgCheats.Rows[index2].Cells[0].Tag = (object) saveFile;
        this.dgCheats.Rows[index2].Cells[1].Tag = (object) cheat.id;
        this.dgCheats.Rows[index2].Tag = (object) g.options;
      }
      if (g._group == null)
        return;
      foreach (group g1 in g._group)
        this.FillGroupCheats(file, g1, saveFile, level + 1);
    }

    private bool ContainsGameFile(List<file> allGameFile, file @internal)
    {
      foreach (file file in allGameFile)
      {
        if (file.id == @internal.id)
          return true;
      }
      return false;
    }

    private void dgCheats_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      this.RefreshValue();
      if (e.RowIndex < 0 || e.ColumnIndex != 2)
        return;
      string str = this.dgCheats.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
      if (str == null || str.IndexOf("http://") < 0)
        return;
      int startIndex = str.IndexOf("http://");
      int num = str.IndexOf(' ', startIndex);
      if (num > 0)
        Process.Start(str.Substring(str.IndexOf("http"), num - startIndex));
      else
        Process.Start(str.Substring(str.IndexOf("http")));
    }

    private void RefreshValue()
    {
      this.dgCheatCodes.Rows.Clear();
      int num = -1;
      if (this.dgCheats.SelectedCells.Count == 1)
        num = this.dgCheats.SelectedCells[0].RowIndex;
      if (this.dgCheats.SelectedRows.Count == 1)
        num = this.dgCheats.SelectedRows[0].Index;
      if (num >= 0 || this.dgCheats.Rows.Count <= 0)
        ;
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
      bool flag = false;
      container targetGameFolder = this.m_game.GetTargetGameFolder();

      {
        List<string> files = new List<string>();
        for (int index = 0; index < this.dgCheats.Rows.Count; ++index)
        {
          if (this.dgCheats.Rows[index].Cells[0].Value != null && (bool) this.dgCheats.Rows[index].Cells[0].Value)
          {
            foreach (file file in new List<file>((IEnumerable<file>) targetGameFolder.files._files))
            {
              foreach (cheat cheat in file.GetAllCheats())
              {
                if ((this.dgCheats.Rows[index].Cells[1].Tag == cheat.id || (string) this.dgCheats.Rows[index].Tag == "UserCheat" && cheat.id == "-1" && cheat.name == this.dgCheats.Rows[index].Cells[1].Value) && (this.m_gameFiles == null || this.dgCheats.Rows[index].Cells[0].Tag == file.filename))
                {
                  cheat.Selected = true;
                  if (files.IndexOf(file.filename) < 0)
                    files.Add(file.filename);
                }
              }
            }
            flag = true;
          }
        }
        {
          this.Close();
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void dgCheats_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      this.RefreshValue();
    }

    private void btnApplyCodes_Click(object sender, EventArgs e)
    {
      {
        this.m_game.GetTargetGameFolder();
        int num2 = (int) this.dgCheatCodes.Tag;
      }
    }

    private void addCodeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      List<string> existingCodes = new List<string>();
      foreach (file file in this.m_game.GetTargetGameFolder().files._files)
      {
        foreach (cheat cheat in file.Cheats)
          existingCodes.Add(cheat.name);
      }
      AddCode addCode = new AddCode(existingCodes);
      if (addCode.ShowDialog() == DialogResult.OK)
      {
        cheat cheat = new cheat("-1", addCode.Description, addCode.Comment);
        cheat.code = addCode.Code;
        if (this.m_game.GetTargetGameFolder() == null)
        {
          return;
        }
        this.m_game.GetGameFile(this.m_game.GetTargetGameFolder(), this.GetSelectedSaveFile()).Cheats.Add(cheat);
        this.SaveUserCheats();
        this.m_bCheatsModified = true;
      }
      this.FillCheats(addCode.Description);
    }

    private string GetSelectedSaveFile()
    {
      int index1 = this.dgCheats.SelectedRows[0].Index;
      if (this.dgCheats.Rows[index1].Cells[0].Tag != null && this.dgCheats.Rows[index1].Cells[0].Tag == "GameFile")
        return this.dgCheats.Rows[index1].Cells[1].Value.ToString();
      for (int index2 = index1; index2 >= 0; --index2)
      {
        if (this.dgCheats.Rows[index2].Cells[0].Tag == "GameFile")
          return this.dgCheats.Rows[index2].Tag.ToString();
      }
      return (string) null;
    }

    private void SaveUserCheats()
    {

      {
        string xml = "<usercheats></usercheats>";
        string str = Util.GetBackupLocation() + (object) Path.DirectorySeparatorChar + MainForm.USER_CHEATS_FILE;
        if (File.Exists(str))
          xml = File.ReadAllText(str);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        bool flag = false;
        container targetGameFolder = this.m_game.GetTargetGameFolder();
        for (int index = 0; index < xmlDocument["usercheats"].ChildNodes.Count; ++index)
        {
          if (this.m_game.id + targetGameFolder.key == xmlDocument["usercheats"].ChildNodes[index].Attributes["id"].Value)
            flag = true;
        }
        if (!flag)
        {
          XmlElement element = xmlDocument.CreateElement("game");
          element.SetAttribute("id", this.m_game.id + targetGameFolder.key);
          xmlDocument["usercheats"].AppendChild((XmlNode) element);
        }
        for (int index = 0; index < xmlDocument["usercheats"].ChildNodes.Count; ++index)
        {
          if (this.m_game.id + targetGameFolder.key == xmlDocument["usercheats"].ChildNodes[index].Attributes["id"].Value)
          {
            XmlElement xmlElement = xmlDocument["usercheats"].ChildNodes[index] as XmlElement;
            xmlElement.InnerXml = "";
            List<file> list = new List<file>((IEnumerable<file>) targetGameFolder.files._files);
            foreach (file file in targetGameFolder.files._files)
            {
              if (file.internals != null && file.internals.files.Count > 0)
                list.AddRange((IEnumerable<file>) file.internals.files);
            }
            foreach (file file in list)
            {
              XmlElement element1 = xmlDocument.CreateElement("file");
              element1.SetAttribute("name", file.filename);
              xmlElement.AppendChild((XmlNode) element1);
              foreach (cheat cheat in file.Cheats)
              {
                if (cheat.id == "-1")
                {
                  XmlElement element2 = xmlDocument.CreateElement("cheat");
                  element2.SetAttribute("desc", cheat.name);
                  element2.SetAttribute("comment", cheat.note);
                  element1.AppendChild((XmlNode) element2);
                  XmlElement element3 = xmlDocument.CreateElement("code");
                  element3.InnerText = cheat.code;
                  element2.AppendChild((XmlNode) element3);
                }
              }
            }
          }
        }
        xmlDocument.Save(str);
      }
    }

    private void editCodeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      int index1 = this.dgCheats.SelectedRows[0].Index;
      file gameFile = this.m_game.GetGameFile(this.m_game.GetTargetGameFolder(), this.dgCheats.Rows[index1].Cells[0].Tag.ToString());
      cheat cheat1 = (cheat) null;
      foreach (cheat cheat2 in gameFile.Cheats)
      {
        if (cheat2.name == this.dgCheats.Rows[index1].Cells[1].Value.ToString())
        {
          cheat1 = cheat2;
          break;
        }
      }
      if (cheat1 == null)
        return;
      List<string> existingCodes = new List<string>();
      foreach (file file in this.m_game.GetTargetGameFolder().files._files)
      {
        foreach (cheat cheat2 in file.Cheats)
        {
          if (cheat2.name != this.dgCheats.Rows[index1].Cells[1].Value.ToString())
            existingCodes.Add(cheat2.name);
        }
      }
      AddCode addCode = new AddCode(cheat1, existingCodes);
      if (addCode.ShowDialog() == DialogResult.OK)
      {
        cheat cheat2 = new cheat("-1", addCode.Description, addCode.Comment);
        cheat2.code = addCode.Code;
        for (int index2 = 0; index2 < gameFile.Cheats.Count; ++index2)
        {
          if (gameFile.Cheats[index2].name == this.dgCheats.Rows[index1].Cells[1].Value.ToString())
            gameFile.Cheats[index2] = cheat2;
        }
        this.SaveUserCheats();
        this.m_bCheatsModified = true;
      }
      this.FillCheats(addCode.Description);
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      if (this.m_game.GetTargetGameFolder() == null)
        e.Cancel = true;
      else if (this.dgCheats.SelectedRows.Count == 1)
      {
        container targetGameFolder = this.m_game.GetTargetGameFolder();
        int? quickmode = targetGameFolder.quickmode;
        if ((quickmode.GetValueOrDefault() <= 0 ? 0 : (quickmode.HasValue ? 1 : 0)) != 0)
        {
          e.Cancel = true;
        }
        else
        {
          int index = this.dgCheats.SelectedRows[0].Index;
          if (this.dgCheats.Rows[index].Cells[0].Tag != null && (this.dgCheats.Rows[index].Cells[0].Tag.ToString() == "NoCheats" || this.dgCheats.Rows[index].Cells[0].Tag.ToString() == "GameFile"))
          {
            e.Cancel = false;
          }
          else
          {
            int count = targetGameFolder.files._files.Count;
          }
          this.editCodeToolStripMenuItem.Enabled = this.dgCheats.Rows[index].Tag == "UserCheat";
          this.deleteCodeToolStripMenuItem.Enabled = this.dgCheats.Rows[index].Tag == "UserCheat";
        }
      }
      else
        e.Cancel = true;
    }

    private void dgCheats_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.RowIndex < 0 || e.Button != MouseButtons.Right)
        return;
      this.dgCheats.ClearSelection();
      this.dgCheats.Rows[e.RowIndex].Selected = true;
    }

    private void deleteCodeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      int index1 = this.dgCheats.SelectedRows[0].Index;
      if (index1 < 0)
        return;
      file gameFile = this.m_game.GetGameFile(this.m_game.GetTargetGameFolder(), this.dgCheats.Rows[index1].Cells[0].Tag.ToString());
      for (int index2 = 0; index2 < gameFile.Cheats.Count; ++index2)
      {
        if (gameFile.Cheats[index2].name == this.dgCheats.Rows[index1].Cells[1].Value.ToString())
        {
          gameFile.Cheats.RemoveAt(index2);
          break;
        }
      }
      this.SaveUserCheats();
      this.FillCheats((string) null);
      this.m_bCheatsModified = true;
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
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle4 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle5 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle6 = new DataGridViewCellStyle();
      this.lblGameName = new Label();
      this.contextMenuStrip1 = new ContextMenuStrip(this.components);
      this.addCodeToolStripMenuItem = new ToolStripMenuItem();
      this.editCodeToolStripMenuItem = new ToolStripMenuItem();
      this.deleteCodeToolStripMenuItem = new ToolStripMenuItem();
      this.panel1 = new Panel();
      this.lblProfile = new Label();
      this.cbProfile = new ComboBox();
      this.btnApplyCodes = new Button();
      this.dgCheatCodes = new CustomDataGridView();
      this.Location = new DataGridViewTextBoxColumn();
      this.Value = new DataGridViewTextBoxColumn();
      this.label1 = new Label();
      this.dgCheats = new CustomDataGridView();
      this.Select = new DataGridViewCheckBoxColumn();
      this.Description = new DataGridViewTextBoxColumn();
      this.Comment = new DataGridViewTextBoxColumn();
      this.btnClose = new Button();
      this.btnApply = new Button();
      this.contextMenuStrip1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.lblGameName.AutoSize = true;
      this.lblGameName.Location = new Point(17, 9);
      this.lblGameName.Name = "lblGameName";
      this.lblGameName.Size = new Size(0, 13);
      this.lblGameName.TabIndex = 0;
      this.contextMenuStrip1.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.addCodeToolStripMenuItem,
        (ToolStripItem) this.editCodeToolStripMenuItem,
        (ToolStripItem) this.deleteCodeToolStripMenuItem
      });
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new Size(139, 70);
      this.contextMenuStrip1.Opening += new CancelEventHandler(this.contextMenuStrip1_Opening);
      this.addCodeToolStripMenuItem.Name = "addCodeToolStripMenuItem";
      this.addCodeToolStripMenuItem.Size = new Size(138, 22);
      this.addCodeToolStripMenuItem.Text = "Add Code";
      this.addCodeToolStripMenuItem.Click += new EventHandler(this.addCodeToolStripMenuItem_Click);
      this.editCodeToolStripMenuItem.Name = "editCodeToolStripMenuItem";
      this.editCodeToolStripMenuItem.Size = new Size(138, 22);
      this.editCodeToolStripMenuItem.Text = "Edit Code";
      this.editCodeToolStripMenuItem.Click += new EventHandler(this.editCodeToolStripMenuItem_Click);
      this.deleteCodeToolStripMenuItem.Name = "deleteCodeToolStripMenuItem";
      this.deleteCodeToolStripMenuItem.Size = new Size(138, 22);
      this.deleteCodeToolStripMenuItem.Text = "Delete Code";
      this.deleteCodeToolStripMenuItem.Click += new EventHandler(this.deleteCodeToolStripMenuItem_Click);
      this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel1.BackColor = System.Drawing.Color.FromArgb(102, 102, 102);
      this.panel1.Controls.Add((Control) this.lblProfile);
      this.panel1.Controls.Add((Control) this.cbProfile);
      this.panel1.Controls.Add((Control) this.btnApplyCodes);
      this.panel1.Controls.Add((Control) this.dgCheatCodes);
      this.panel1.Controls.Add((Control) this.label1);
      this.panel1.Controls.Add((Control) this.dgCheats);
      this.panel1.Controls.Add((Control) this.btnClose);
      this.panel1.Controls.Add((Control) this.btnApply);
      this.panel1.Location = new Point(10, 11);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(634, 277);
      this.panel1.TabIndex = 1;
      this.lblProfile.Anchor = AnchorStyles.Bottom;
      this.lblProfile.AutoSize = true;
      this.lblProfile.ForeColor = System.Drawing.Color.White;
      this.lblProfile.Location = new Point(72, 250);
      this.lblProfile.Name = "lblProfile";
      this.lblProfile.Size = new Size(39, 13);
      this.lblProfile.TabIndex = 17;
      this.lblProfile.Text = "Profile:";
      this.cbProfile.Anchor = AnchorStyles.Bottom;
      this.cbProfile.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbProfile.FormattingEnabled = true;
      this.cbProfile.Location = new Point(118, 246);
      this.cbProfile.Name = "cbProfile";
      this.cbProfile.Size = new Size(112, 21);
      this.cbProfile.TabIndex = 16;
      this.btnApplyCodes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btnApplyCodes.Location = new Point(551, 175);
      this.btnApplyCodes.Name = "btnApplyCodes";
      this.btnApplyCodes.Size = new Size(75, 23);
      this.btnApplyCodes.TabIndex = 15;
      this.btnApplyCodes.Text = "Apply";
      this.btnApplyCodes.UseVisualStyleBackColor = true;
      this.btnApplyCodes.Visible = false;
      this.dgCheatCodes.AllowUserToAddRows = false;
      this.dgCheatCodes.AllowUserToDeleteRows = false;
      this.dgCheatCodes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.dgCheatCodes.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgCheatCodes.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgCheatCodes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgCheatCodes.Columns.AddRange((DataGridViewColumn) this.Location, (DataGridViewColumn) this.Value);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
      gridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgCheatCodes.DefaultCellStyle = gridViewCellStyle2;
      this.dgCheatCodes.Location = new Point(539, 37);
      this.dgCheatCodes.Name = "dgCheatCodes";
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgCheatCodes.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgCheatCodes.Size = new Size(35, 48);
      this.dgCheatCodes.TabIndex = 14;
      this.dgCheatCodes.Visible = false;
      this.Location.HeaderText = "Location";
      this.Location.Name = "Location";
      this.Location.Width = 70;
      this.Value.HeaderText = "Value";
      this.Value.MaxInputLength = 13;
      this.Value.Name = "Value";
      this.Value.Width = 70;
      this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(452, -1);
      this.label1.Name = "label1";
      this.label1.Size = new Size(71, 13);
      this.label1.TabIndex = 13;
      this.label1.Text = "Cheat Codes:";
      this.label1.Visible = false;
      this.dgCheats.AllowUserToAddRows = false;
      this.dgCheats.AllowUserToDeleteRows = false;
      this.dgCheats.AllowUserToResizeRows = false;
      this.dgCheats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.dgCheats.BackgroundColor = System.Drawing.Color.FromArgb(175, 175, 175);
      this.dgCheats.BorderStyle = BorderStyle.None;
      this.dgCheats.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle4.BackColor = SystemColors.Control;
      gridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle4.ForeColor = SystemColors.WindowText;
      gridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle4.WrapMode = DataGridViewTriState.True;
      this.dgCheats.ColumnHeadersDefaultCellStyle = gridViewCellStyle4;
      this.dgCheats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgCheats.Columns.AddRange((DataGridViewColumn) this.Select, (DataGridViewColumn) this.Description, (DataGridViewColumn) this.Comment);
      this.dgCheats.ContextMenuStrip = this.contextMenuStrip1;
      gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle5.BackColor = SystemColors.Window;
      gridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
      gridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle5.WrapMode = DataGridViewTriState.False;
      this.dgCheats.DefaultCellStyle = gridViewCellStyle5;
      this.dgCheats.GridColor = System.Drawing.Color.FromArgb(175, 175, 175);
      this.dgCheats.Location = new Point(12, 13);
      this.dgCheats.Name = "dgCheats";
      gridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle6.BackColor = SystemColors.Control;
      gridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle6.ForeColor = SystemColors.WindowText;
      gridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle6.WrapMode = DataGridViewTriState.True;
      this.dgCheats.RowHeadersDefaultCellStyle = gridViewCellStyle6;
      this.dgCheats.RowHeadersVisible = false;
      this.dgCheats.RowHeadersWidth = 25;
      this.dgCheats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgCheats.Size = new Size(610, 222);
      this.dgCheats.TabIndex = 12;
      this.Select.Name = "Select";
      this.Select.Width = 30;
      this.Description.HeaderText = "Description";
      this.Description.Name = "Description";
      this.Description.ReadOnly = true;
      this.Description.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.Description.Width = 240;
      this.Comment.HeaderText = "Comment";
      this.Comment.Name = "Comment";
      this.Comment.ReadOnly = true;
      this.Comment.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.Comment.Width = 340;
      this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.btnClose.BackColor = System.Drawing.Color.FromArgb(246, 128, 31);
      this.btnClose.ForeColor = System.Drawing.Color.White;
      this.btnClose.Location = new Point(318, 246);
      this.btnClose.MaximumSize = new Size(60, 23);
      this.btnClose.MinimumSize = new Size(60, 23);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new Size(60, 23);
      this.btnClose.TabIndex = 11;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = false;
      this.btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.btnApply.BackColor = System.Drawing.Color.FromArgb(246, 128, 31);
      this.btnApply.ForeColor = System.Drawing.Color.White;
      this.btnApply.Location = new Point(254, 246);
      this.btnApply.MaximumSize = new Size(60, 23);
      this.btnApply.MinimumSize = new Size(60, 23);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new Size(60, 23);
      this.btnApply.TabIndex = 10;
      this.btnApply.Text = "Patch && Download Save";
      this.btnApply.UseVisualStyleBackColor = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new Size(654, 298);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.lblGameName);
      this.ForeColor = System.Drawing.Color.Black;
      this.MinimumSize = new Size(550, 336);
      this.Name = "SimpleEdit";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.Text = "Simple Edit";
      this.contextMenuStrip1.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
