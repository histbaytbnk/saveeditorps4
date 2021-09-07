
// Type: PS3SaveEditor.ServerSaveDownloader


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class ServerSaveDownloader : Form
  {
    private string m_saveFolder;
    private ServerSaveDownloader.CloseDelegate CloseForm;
    private string m_gamecode;
    private bool appClosing;
    private game m_game;
    private IContainer components;
    private SaveUploadDownloder saveUploadDownloder1;

    public ServerSaveDownloader(string saveid, string saveFolder, game game)
    {
      this.m_saveFolder = saveFolder;
      this.m_game = game;
      this.InitializeComponent();
      this.BackColor = Color.FromArgb(80, 29, 11);
      this.saveUploadDownloder1.BackColor = Color.FromArgb(200, 100, 10);
      this.CenterToScreen();
      this.saveUploadDownloder1.SaveId = saveid;
      this.saveUploadDownloder1.Action = "download";
      this.saveUploadDownloder1.OutputFolder = this.m_saveFolder;
      this.CloseForm = new ServerSaveDownloader.CloseDelegate(this.CloseFormSafe);
      this.Load += new EventHandler(this.SimpleSaveUploader_Load);
    }

    private List<string> PrepareZipFile(game game)
    {
      List<string> list = new List<string>();
      list.Add(Path.Combine(this.m_saveFolder, "PARAM.SFO"));
      list.Add(Path.Combine(this.m_saveFolder, "PARAM.PFD"));
      string path = Path.Combine(Util.GetTempFolder(), "ps3_files_list.xml");
      if (game != null)
      {
        File.WriteAllText(path, "<files><game>" + game.id + "</game><pfd>PARAM.PFD</pfd><sfo>PARAM.SFO</sfo></files>");
      }
      else
      {
        string str = MainForm.GetParamInfo(Path.Combine(this.m_saveFolder, "PARAM.SFO"), "SAVEDATA_DIRECTORY");
        if (string.IsNullOrEmpty(str) || str.Length < 9)
          str = Path.GetDirectoryName(this.m_saveFolder);
        File.WriteAllText(path, "<files><game>" + str.Substring(0, 9) + "</game><pfd>PARAM.PFD</pfd><sfo>PARAM.SFO</sfo></files>");
      }
      list.Add(path);
      return list;
    }



    private void saveUploadDownloder1_DownloadFinish(object sender, DownloadFinishEventArgs e)
    {
        Directory.Exists(this.m_game.LocalSaveFolder);
      this.CloseThis(e.Status);
    }

    private void SimpleSaveUploader_Load(object sender, EventArgs e)
    {
      this.saveUploadDownloder1.Start();
    }

    private void CloseThis(bool status)
    {
      if (this.IsDisposed)
        return;
    }

    private void CloseFormSafe(bool bStatus)
    {
      if (bStatus)
        this.DialogResult = DialogResult.OK;
      else
        this.DialogResult = DialogResult.Abort;
      this.appClosing = true;
      this.Close();
    }

    private void SimpleSaveUploader_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.appClosing)
        return;
      e.Cancel = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.saveUploadDownloder1 = new SaveUploadDownloder();
      this.SuspendLayout();
      this.saveUploadDownloder1.Action = (string) null;
      this.saveUploadDownloder1.BackColor = Color.FromArgb(200, 100, 10);
      this.saveUploadDownloder1.FilePath = (string) null;
      this.saveUploadDownloder1.Files = (string[]) null;
      this.saveUploadDownloder1.Game = (game) null;
      this.saveUploadDownloder1.IsUpload = false;
      this.saveUploadDownloder1.Location = new Point(13, 12);
      this.saveUploadDownloder1.Name = "saveUploadDownloder1";
      this.saveUploadDownloder1.OutputFolder = (string) null;
      this.saveUploadDownloder1.Profile = (string) null;
      this.saveUploadDownloder1.Size = new Size(446, 113);
      this.saveUploadDownloder1.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(473, 138);
      this.Controls.Add((Control) this.saveUploadDownloder1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ServerSaveDownloader";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "ServerSaveDownloader";
      this.ResumeLayout(false);
    }

    private delegate void CloseDelegate(bool bStatus);
  }
}
