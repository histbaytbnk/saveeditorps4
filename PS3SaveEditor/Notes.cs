
// Type: PS3SaveEditor.Notes


// Hacked by SystemAce

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class Notes : Form
  {
    private IContainer components;
    private Panel panel1;
    private Button btnOk;
    private WebBrowser webBrowser;

    public Notes(string notes)
    {
      this.InitializeComponent();
      this.CenterToScreen();
      this.panel1.BackColor = Color.FromArgb((int) sbyte.MaxValue, 204, 204, 204);
      this.webBrowser.DocumentText = "<html><body style='font-size:12px;overflow-y:auto'>" + notes + "</body></html>";
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.Close();
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
      this.webBrowser = new WebBrowser();
      this.btnOk = new Button();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.panel1.Controls.Add((Control) this.btnOk);
      this.panel1.Controls.Add((Control) this.webBrowser);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(10, 10);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new Padding(12);
      this.panel1.Size = new Size(548, 256);
      this.panel1.TabIndex = 0;
      this.webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.webBrowser.Location = new Point(12, 12);
      this.webBrowser.MinimumSize = new Size(20, 20);
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.Size = new Size(524, 206);
      this.webBrowser.TabIndex = 0;
      this.btnOk.Location = new Point(236, 225);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(75, 23);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(568, 276);
      this.Controls.Add((Control) this.panel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Notes";
      this.Padding = new Padding(10);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Notes";
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
