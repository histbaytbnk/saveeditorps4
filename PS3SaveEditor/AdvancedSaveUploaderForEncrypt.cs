﻿
// Type: PS3SaveEditor.AdvancedSaveUploaderForEncrypt


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class AdvancedSaveUploaderForEncrypt : Form
  {
    private AdvancedSaveUploaderForEncrypt.CloseDelegate CloseForm;
    private string m_action;
    private string[] m_files;
    private bool appClosing;
    private IContainer components;
    private SaveUploadDownloder saveUploadDownloder1;

    public Dictionary<string, byte[]> DecryptedSaveData { get; set; }

    public byte[] DependentSaveData { get; set; }

    public string ListResult { get; set; }

    public AdvancedSaveUploaderForEncrypt(string[] files, game gameItem, string profile, string action)
    {
      this.m_files = files;
      this.InitializeComponent();
      this.DecryptedSaveData = new Dictionary<string, byte[]>();
      this.saveUploadDownloder1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.m_action = action;
      if (action == "list")
      this.CenterToScreen();
      this.saveUploadDownloder1.Files = files;
      this.saveUploadDownloder1.Action = action;
      this.saveUploadDownloder1.OutputFolder = !(this.m_action == "encrypt") ? ZipUtil.GetPs3SeTempFolder() : Path.GetDirectoryName(gameItem.LocalSaveFolder);
      this.saveUploadDownloder1.Game = gameItem;
      this.CloseForm = new AdvancedSaveUploaderForEncrypt.CloseDelegate(this.CloseFormSafe);
      this.Load += new EventHandler(this.AdvancedSaveUploaderForEncrypt_Load);
      this.FormClosing += new FormClosingEventHandler(this.AdvancedSaveUploaderForEncrypt_FormClosing);
      this.saveUploadDownloder1.DownloadFinish += new SaveUploadDownloder.DownloadFinishEventHandler(this.saveUploadDownloder1_DownloadFinish);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0, 138, 213), Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }




    private void saveUploadDownloder1_DownloadFinish(object sender, DownloadFinishEventArgs e)
    {
      if (this.m_action == "list" && !string.IsNullOrEmpty(this.saveUploadDownloder1.ListResult))
      {
        this.ListResult = this.saveUploadDownloder1.ListResult;
        if (this.IsDisposed)
          return;
        this.CloseThis(e.Status);
      }
      else
      {
        List<string> saveFiles = this.saveUploadDownloder1.Game.GetSaveFiles();
        string[] files = Directory.GetFiles(this.saveUploadDownloder1.OutputFolder, "*");
        foreach (string pattern in saveFiles)
        {
          this.saveUploadDownloder1.Game.GetTargetGameFolder();
          if (e.Status)
          {
            if (this.m_action == "decrypt")
            {
              foreach (string path2 in this.saveUploadDownloder1.OrderedEntries)
              {
                string path = Path.Combine(Util.GetTempFolder(), path2);
                if (Array.IndexOf<string>(files, path) >= 0 && !Path.GetFileName(path).Equals("param.sfo", StringComparison.CurrentCultureIgnoreCase) && (!Path.GetFileName(path).Equals("param.pfd", StringComparison.CurrentCultureIgnoreCase) && !Path.GetFileName(path).Equals("devlog.txt", StringComparison.CurrentCultureIgnoreCase)) && (!Path.GetFileName(path).Equals("ps4_list.xml", StringComparison.CurrentCultureIgnoreCase) && !this.DecryptedSaveData.ContainsKey(Path.GetFileName(path)) && (pattern == Path.GetFileName(path) || Util.IsMatch(Path.GetFileName(path), pattern))))
                  this.DecryptedSaveData.Add(Path.GetFileName(path), File.ReadAllBytes(path));
              }
            }
          }
          else
          {
            if (!(e.Error == "Abort"))
            {
              if (!string.IsNullOrEmpty(e.Error))
              {
                break;
              }
              break;
            }
            break;
          }
        }
        if (this.IsDisposed)
          return;
        this.CloseThis(e.Status);
      }
    }

    private void CloseThis(bool bStatus)
    {
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

    private void AdvancedSaveUploaderForEncrypt_Load(object sender, EventArgs e)
    {
      this.saveUploadDownloder1.Start();
    }

    private void AdvancedSaveUploaderForEncrypt_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!this.appClosing && e.CloseReason == CloseReason.UserClosing)
      {
        this.saveUploadDownloder1.AbortEvent.Set();
        this.DialogResult = DialogResult.Abort;
        this.appClosing = true;
        e.Cancel = true;
      }
      else
      {
        if (this.appClosing)
          return;
        e.Cancel = true;
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
      this.saveUploadDownloder1 = new SaveUploadDownloder();
      this.SuspendLayout();
      this.saveUploadDownloder1.Action = (string) null;
      this.saveUploadDownloder1.BackColor = Color.FromArgb(102, 102, 102);
      this.saveUploadDownloder1.FilePath = (string) null;
      this.saveUploadDownloder1.Files = (string[]) null;
      this.saveUploadDownloder1.Game = (game) null;
      this.saveUploadDownloder1.IsUpload = false;
      this.saveUploadDownloder1.ListResult = (string) null;
      this.saveUploadDownloder1.Location = new Point(12, 12);
      this.saveUploadDownloder1.Name = "saveUploadDownloder1";
      this.saveUploadDownloder1.OrderedEntries = (List<string>) null;
      this.saveUploadDownloder1.OutputFolder = (string) null;
      this.saveUploadDownloder1.Profile = (string) null;
      this.saveUploadDownloder1.ProgressBar = (ProgressBar) null;
      this.saveUploadDownloder1.SaveId = (string) null;
      this.saveUploadDownloder1.Size = new Size(446, 146);
      this.saveUploadDownloder1.StatusLabel = (Label) null;
      this.saveUploadDownloder1.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Black;
      this.ClientSize = new Size(472, 170);
      this.Controls.Add((Control) this.saveUploadDownloder1);
      this.FormBorderStyle = FormBorderStyle.Fixed3D;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AdvancedSaveUploaderForEncrypt";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Save Downloader";
      this.FormClosing += new FormClosingEventHandler(this.AdvancedSaveUploaderForEncrypt_FormClosing);
      this.ResumeLayout(false);
    }

    private delegate void CloseDelegate(bool bStatus);
  }
}
