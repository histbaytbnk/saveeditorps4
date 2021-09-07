﻿
// Type: PS3SaveEditor.Goto


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class Goto : Form
  {
    private long m_maxLength;
    private IContainer components;
    private Label lblEnterLoc;
    private TextBox txtLocation;
    private Button btnOk;
    private Button btnCancel;

    public long AddressLocation { get; set; }

    public Goto(long maxLength)
    {
      this.m_maxLength = maxLength;
      this.InitializeComponent();
      this.CenterToScreen();
      this.btnOk.Enabled = false;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.AddressLocation = !this.txtLocation.Text.StartsWith("0x") ? long.Parse(this.txtLocation.Text) : long.Parse(this.txtLocation.Text.Substring(2), NumberStyles.HexNumber);
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void txtLocation_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.txtLocation.Text))
      {
        this.btnOk.Enabled = false;
      }
      else
      {
        if (this.txtLocation.Text.StartsWith("0x"))
        {
          if (this.txtLocation.Text.Length > 2)
          {
            if (long.Parse(this.txtLocation.Text.Substring(2), NumberStyles.HexNumber) > this.m_maxLength)
            {
              this.btnOk.Enabled = false;
              return;
            }
          }
          else
          {
            this.btnOk.Enabled = false;
            return;
          }
        }
        else
        {
          long result;
          if (long.TryParse(this.txtLocation.Text.Trim(), out result))
          {
            if (result > this.m_maxLength)
            {
              this.btnOk.Enabled = false;
              return;
            }
          }
          else if (long.TryParse(this.txtLocation.Text.Trim(), NumberStyles.HexNumber, (IFormatProvider) null, out result))
          {
            this.txtLocation.Text = "0x" + this.txtLocation.Text.Trim();
            if (result > this.m_maxLength)
            {
              this.btnOk.Enabled = false;
              return;
            }
          }
          else
            this.txtLocation.Text = "";
        }
        this.btnOk.Enabled = true;
      }
    }

    private void txtLocation_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || (e.Control || e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && !e.Shift || e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 && !e.Shift) || (this.txtLocation.SelectionStart == 1 && e.KeyCode == Keys.X && (int) this.txtLocation.Text[0] == 48 || this.txtLocation.Text.StartsWith("0x") && e.KeyCode >= Keys.A && e.KeyCode <= Keys.F))
        return;
      e.SuppressKeyPress = true;
    }

    private void txtLocation_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.lblEnterLoc = new Label();
      this.txtLocation = new TextBox();
      this.btnOk = new Button();
      this.btnCancel = new Button();
      this.SuspendLayout();
      this.lblEnterLoc.AutoSize = true;
      this.lblEnterLoc.Location = new Point(14, 11);
      this.lblEnterLoc.Name = "lblEnterLoc";
      this.lblEnterLoc.Size = new Size(79, 13);
      this.lblEnterLoc.TabIndex = 0;
      this.lblEnterLoc.Text = "Enter Location:";
      this.txtLocation.Location = new Point(99, 9);
      this.txtLocation.Name = "txtLocation";
      this.txtLocation.Size = new Size(97, 20);
      this.txtLocation.TabIndex = 1;
      this.txtLocation.TextChanged += new EventHandler(this.txtLocation_TextChanged);
      this.txtLocation.KeyDown += new KeyEventHandler(this.txtLocation_KeyDown);
      this.txtLocation.KeyPress += new KeyPressEventHandler(this.txtLocation_KeyPress);
      this.btnOk.DialogResult = DialogResult.OK;
      this.btnOk.Location = new Point(121, 35);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(75, 23);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "Ok";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.Location = new Point(202, 35);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(75, 23);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(284, 69);
      this.Controls.Add((Control) this.btnCancel);
      this.Controls.Add((Control) this.btnOk);
      this.Controls.Add((Control) this.txtLocation);
      this.Controls.Add((Control) this.lblEnterLoc);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Goto";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Go To Location";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
