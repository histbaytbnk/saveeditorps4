
// Type: PS3SaveEditor.GameListDownloader


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using Rss;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class GameListDownloader : Form
  {
    private static string GAME_LIST_URL = "{0}/games?token={1}";
    private GameListDownloader.UpdateProgressDelegate UpdateProgress;
    private GameListDownloader.UpdateStatusDelegate UpdateStatus;
    private GameListDownloader.CloseDelegate CloseForm;
    private bool appClosing;
    private IContainer components;
    private Label lblStatus;
    private PS4ProgressBar pbProgress;
    private Panel panel1;

    public string GameListXml { get; set; }

    public GameListDownloader()
    {
      string registryValue = Util.GetRegistryValue("Language");
      if (registryValue != null)
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(registryValue);
      this.InitializeComponent();
      this.ControlBox = false;
      this.BackColor = Color.FromArgb(80, 29, 11);
      this.panel1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.lblStatus.BackColor = Color.Transparent;
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.CenterToScreen();
      this.UpdateProgress = new GameListDownloader.UpdateProgressDelegate(this.UpdateProgressSafe);
      this.UpdateStatus = new GameListDownloader.UpdateStatusDelegate(this.UpdateStatusSafe);
      this.CloseForm = new GameListDownloader.CloseDelegate(this.CloseFormSafe);
      this.Load += new EventHandler(this.GameListDownloader_Load);
      this.Visible = false;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0, 138, 213), Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void CloseThisForm(bool bSuccess)
    {
      if (!this.IsHandleCreated)
        return;
    }

    private void CloseFormSafe(bool bSuccess)
    {
      if (!bSuccess)
        this.DialogResult = DialogResult.Abort;
      else
        this.DialogResult = DialogResult.OK;
      this.appClosing = true;
      this.Close();
    }

    private void SetStatus(string status)
    {
      this.lblStatus.Invoke((Delegate) this.UpdateStatus, (object) status);
    }

    private void UpdateStatusSafe(string status)
    {
      this.lblStatus.Text = status;
    }

    private void SetProgress(int val)
    {
      this.pbProgress.Invoke((Delegate) this.UpdateProgress, (object) val);
    }

    private void UpdateProgressSafe(int val)
    {
      this.pbProgress.Value = val;
    }

    private void GameListDownloader_Load(object sender, EventArgs e)
    {
      Thread thread = new Thread(new ThreadStart(this.GetOnlineGamesList));
      string registryValue = Util.GetRegistryValue("Language");
      if (registryValue != null)
        thread.CurrentUICulture = CultureInfo.CreateSpecificCulture(registryValue);
      thread.Start();
      try
      {
        long ticks = DateTime.Now.Ticks;
        RssChannel channel = RssFeed.Read(string.Format("{0}/ps4/rss?token={0}", (object) Util.GetBaseUrl(), (object) Util.GetAuthToken())).Channels[0];
        if (channel.Items.Count <= 0)
          return;
        int num = (int) new RSSForm(channel).ShowDialog();
      }
      catch (Exception ex)
      {
            
      }
    }

    private void GetOnlineGamesList()
    {
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(string.Format(GameListDownloader.GAME_LIST_URL, (object) Util.GetBaseUrl(), (object) Util.GetAuthToken()));
        httpWebRequest.Method = "GET";
        httpWebRequest.Credentials = (ICredentials) Util.GetNetworkCredential();
        string str = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Util.GetHtaccessUser() + ":" + Util.GetHtaccessPwd()));
        httpWebRequest.UserAgent = Util.GetUserAgent();
        httpWebRequest.Headers.Add("Authorization", str);
        httpWebRequest.Headers.Add("accept-charset", "UTF-8");
        Encoding.UTF8.GetBytes("" + "form_build_id=af33c2669bb1dc77eb5b3fcdc4526938&" + "license=" + Util.GetUserId() + "&" + "mac=" + Util.GetUID(false, false) + "&" + "gameid=&" + "version=v2&" + "action=list");
    
        HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
        if (HttpStatusCode.OK == httpWebResponse.StatusCode)
        {
          Stream responseStream = httpWebResponse.GetResponseStream();
          int num1 = 0;
          string tempFileName = Path.GetTempFileName();
          FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.Write);
          byte[] buffer = new byte[1024];
          if (httpWebResponse.ContentLength == -1L)
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
                streamWriter.Write(streamReader.ReadToEnd());
            }
          }
          else
          {
            while ((long) num1 < httpWebResponse.ContentLength)
            {
              int count = responseStream.Read(buffer, 0, Math.Min(1024, (int) httpWebResponse.ContentLength - num1));
              fileStream.Write(buffer, 0, count);
              num1 += count;
              this.SetProgress((int) ((long) (num1 * 100) / httpWebResponse.ContentLength));
            }
          }
          this.SetProgress(100);
          fileStream.Close();
          this.GameListXml = System.IO.File.ReadAllText(tempFileName);
          if (this.GameListXml.IndexOf("ERROR") > 0)
          {
            this.GameListXml = "";
            this.CloseThisForm(false);
            return;
          }
          httpWebResponse.Close();
          System.IO.File.Delete(tempFileName);
        }
        else
        {
        }
        Thread.Sleep(1000);
        this.CloseThisForm(true);
      }
      catch (Exception ex)
      {
        this.CloseThisForm(false);
        throw ex;
      }
    }

    private void GameListDownloader_FormClosing(object sender, FormClosingEventArgs e)
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new PS3SaveEditor.PS4ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(11, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(246, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Please wait while the game list being downloaded..";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(11, 46);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(402, 19);
            this.pbProgress.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.pbProgress);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 104);
            this.panel1.TabIndex = 2;
            // 
            // GameListDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(453, 130);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GameListDownloader";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Downloading Games List from Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameListDownloader_FormClosing);
            this.Load += new System.EventHandler(this.GameListDownloader_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

    }

    private delegate void UpdateProgressDelegate(int value);

    private delegate void UpdateStatusDelegate(string status);

    private delegate void CloseDelegate(bool bSuccess);

        private void GameListDownloader_Load_1(object sender, EventArgs e)
        {

        }
    }
}
