
// Type: PS3SaveEditor.ResignMessage


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class ResignMessage : Form
  {
    private IContainer components;
    private CheckBox chkDeleteExisting;
    private Label label1;
    private Panel panel1;
    private Button btnOK;

    public bool DeleteExisting { get; set; }

    public ResignMessage()
    {
      this.InitializeComponent();
      this.CenterToScreen();
      this.btnOK.Click += new EventHandler(this.btnOK_Click);
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.DeleteExisting = this.chkDeleteExisting.Checked;
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
      this.chkDeleteExisting = new CheckBox();
      this.label1 = new Label();
      this.panel1 = new Panel();
      this.btnOK = new Button();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.chkDeleteExisting.AutoSize = true;
      this.chkDeleteExisting.Location = new Point(71, 50);
      this.chkDeleteExisting.Name = "chkDeleteExisting";
      this.chkDeleteExisting.Size = new Size(137, 17);
      this.chkDeleteExisting.TabIndex = 0;
      this.chkDeleteExisting.Text = "Delete the original save";
      this.chkDeleteExisting.UseVisualStyleBackColor = true;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(86, 17);
      this.label1.Name = "label1";
      this.label1.Size = new Size(110, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Re-signing successful";
      this.panel1.Controls.Add((Control) this.btnOK);
      this.panel1.Controls.Add((Control) this.chkDeleteExisting);
      this.panel1.Controls.Add((Control) this.label1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(10, 10);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(277, 115);
      this.panel1.TabIndex = 2;
      this.btnOK.Location = new Point(102, 83);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new Size(75, 23);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(297, 135);
      this.Controls.Add((Control) this.panel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ResignMessage";
      this.Padding = new Padding(10);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "ResignMessage";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
