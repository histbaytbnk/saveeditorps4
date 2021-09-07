
// Type: PS3SaveEditor.DiffResults


// Hacked by SystemAce

using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class DiffResults : Form
  {
    private IContainer components;
    private DataGridView dataGridView1;
    private DataGridViewTextBoxColumn StartAddress;
    private DataGridViewTextBoxColumn EndAddress;
    private DataGridViewTextBoxColumn Bytes;
    private Button btnClose;

    public Dictionary<long, byte> Differences
    {
      set
      {
        this.dataGridView1.Rows.Clear();
        foreach (long index1 in value.Keys)
        {
          int index2 = this.dataGridView1.Rows.Add();
          this.dataGridView1.Rows[index2].Cells[0].Value = (object) index1.ToString("X8");
          if ((int) value[index1] != 1)
            this.dataGridView1.Rows[index2].Cells[1].Value = (object) (index1 + (long) value[index1]).ToString("X8");
          this.dataGridView1.Rows[index2].Cells[2].Value = (object) value[index1].ToString("X2");
        }
      }
    }

    public event EventHandler OnDiffRowSelected;

    public DiffResults()
    {
      this.InitializeComponent();
      this.CenterToScreen();
      this.dataGridView1.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dataGridView1_RowStateChanged);
      this.FormClosing += new FormClosingEventHandler(this.DiffResults_FormClosing);
    }

    private void DiffResults_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
    }

    private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.StateChanged != DataGridViewElementStates.Selected || this.OnDiffRowSelected == null)
        return;
      this.OnDiffRowSelected(sender, EventArgs.Empty);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Hide();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.dataGridView1 = new DataGridView();
      this.StartAddress = new DataGridViewTextBoxColumn();
      this.EndAddress = new DataGridViewTextBoxColumn();
      this.Bytes = new DataGridViewTextBoxColumn();
      this.btnClose = new Button();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeColumns = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.StartAddress, (DataGridViewColumn) this.EndAddress, (DataGridViewColumn) this.Bytes);
      this.dataGridView1.Location = new Point(12, 12);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView1.Size = new Size(322, 213);
      this.dataGridView1.TabIndex = 0;
      this.StartAddress.HeaderText = "Start Address";
      this.StartAddress.Name = "StartAddress";
      this.StartAddress.ReadOnly = true;
      this.EndAddress.HeaderText = "End Address";
      this.EndAddress.Name = "EndAddress";
      this.EndAddress.ReadOnly = true;
      this.EndAddress.Width = 120;
      this.Bytes.HeaderText = "Bytes";
      this.Bytes.Name = "Bytes";
      this.Bytes.ReadOnly = true;
      this.Bytes.Width = 90;
      this.btnClose.Location = new Point(26, 239);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(346, 274);
      this.Controls.Add((Control) this.btnClose);
      this.Controls.Add((Control) this.dataGridView1);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DiffResults";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "DiffResults";
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
