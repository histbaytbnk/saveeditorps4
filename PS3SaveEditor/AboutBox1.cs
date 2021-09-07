
// Type: PS3SaveEditor.AboutBox1


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  internal class AboutBox1 : Form
  {
    private IContainer components;
    private PictureBox pictureBox1;
    private Label lblVersion;
    private Label lblDesc;
    private Label lblCopyright;
    private LinkLabel linkLabel1;
    private Button btnOk;

    public string AssemblyTitle
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
        if (customAttributes.Length > 0)
        {
          AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute) customAttributes[0];
          if (assemblyTitleAttribute.Title != "")
            return assemblyTitleAttribute.Title;
        }
        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public static string AssemblyVersion
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }

    public string AssemblyDescription
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        return ((AssemblyDescriptionAttribute) customAttributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        return ((AssemblyProductAttribute) customAttributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        return ((AssemblyCopyrightAttribute) customAttributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
        if (customAttributes.Length == 0)
          return "";
        return ((AssemblyCompanyAttribute) customAttributes[0]).Company;
      }
    }

    public AboutBox1()
    {
      this.InitializeComponent();
      this.Text = string.Format("About {0}", (object) this.AssemblyTitle);
      this.lblDesc.Visible = false;
      this.linkLabel1.Text = "http://www.savewizard.net/";
      this.lblVersion.Text = string.Format("Version {0}", (object) AboutBox1.AssemblyVersion);
      this.lblCopyright.Text = this.AssemblyCopyright;
      this.lblDesc.Text = this.AssemblyCompany + (Util.CURRENT_SERVER == 0 ? "" : ".");
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start(new ProcessStartInfo(this.linkLabel1.Text));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AboutBox1));
      this.lblVersion = new Label();
      this.lblDesc = new Label();
      this.lblCopyright = new Label();
      this.linkLabel1 = new LinkLabel();
      this.btnOk = new Button();
      this.pictureBox1 = new PictureBox();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.lblVersion.AutoSize = true;
      this.lblVersion.Location = new Point(59, 11);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new Size(0, 13);
      this.lblVersion.TabIndex = 2;
      this.lblDesc.AutoSize = true;
      this.lblDesc.Location = new Point(59, 30);
      this.lblDesc.Name = "lblDesc";
      this.lblDesc.Size = new Size(124, 13);
      this.lblDesc.TabIndex = 3;
      this.lblDesc.Text = "CYBER PS4 Save Editor";
      this.lblCopyright.AutoSize = true;
      this.lblCopyright.Location = new Point(59, 51);
      this.lblCopyright.Name = "lblCopyright";
      this.lblCopyright.Size = new Size(232, 13);
      this.lblCopyright.TabIndex = 4;
      this.lblCopyright.Text = "Copyright © CYBER Gadget. All rights reserved.";
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Location = new Point(59, 72);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new Size(123, 13);
      this.linkLabel1.TabIndex = 5;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "http://cybergadget.co.jp";
      this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      this.btnOk.Location = new Point(291, 70);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(75, 23);
      this.btnOk.TabIndex = 6;
      this.btnOk.Text = "Ok";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.pictureBox1.Image = (Image) componentResourceManager.GetObject("pictureBox1.Image");
      this.pictureBox1.Location = new Point(13, 11);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(32, 32);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(378, 105);
      this.Controls.Add((Control) this.btnOk);
      this.Controls.Add((Control) this.linkLabel1);
      this.Controls.Add((Control) this.lblCopyright);
      this.Controls.Add((Control) this.lblDesc);
      this.Controls.Add((Control) this.lblVersion);
      this.Controls.Add((Control) this.pictureBox1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox1";
      this.Padding = new Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "About PS4 Save Editor";
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
