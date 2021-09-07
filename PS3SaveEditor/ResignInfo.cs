
// Type: PS3SaveEditor.ResignInfo


// Hacked by SystemAce

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class ResignInfo : Form
  {
    private IContainer components;
    private Panel panel1;
    private Button btnOk;
    private CheckBox chkDontShow;
    private Label textBox1;

    public ResignInfo()
    {
      this.InitializeComponent();
      this.CenterToScreen();
      this.Load += new EventHandler(this.ResignInfo_Load);
    }

    private void ResignInfo_Load(object sender, EventArgs e)
    {
      this.btnOk.Focus();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      if (this.chkDontShow.Checked)
        Util.SetRegistryValue("NoResignMessage", "yes");
      this.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ResignInfo));
      this.panel1 = new Panel();
      this.btnOk = new Button();
      this.chkDontShow = new CheckBox();
      this.textBox1 = new Label();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.panel1.Controls.Add((Control) this.btnOk);
      this.panel1.Controls.Add((Control) this.chkDontShow);
      this.panel1.Controls.Add((Control) this.textBox1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(10, 10);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(566, 184);
      this.panel1.TabIndex = 0;
      this.btnOk.DialogResult = DialogResult.OK;
      this.btnOk.Location = new Point(243, 153);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(75, 23);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.chkDontShow.AutoSize = true;
      this.chkDontShow.Location = new Point(13, 154);
      this.chkDontShow.Name = "chkDontShow";
      this.chkDontShow.Size = new Size(179, 17);
      this.chkDontShow.TabIndex = 1;
      this.chkDontShow.Text = "Do not show this message again";
      this.chkDontShow.UseVisualStyleBackColor = true;
      this.textBox1.Location = new Point(11, 10);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(539, 135);
      this.textBox1.TabIndex = 0;
      this.textBox1.Text = componentResourceManager.GetString("textBox1.Text");
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(586, 204);
      this.Controls.Add((Control) this.panel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ResignInfo";
      this.Padding = new Padding(10);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.Text = "Important Information";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
