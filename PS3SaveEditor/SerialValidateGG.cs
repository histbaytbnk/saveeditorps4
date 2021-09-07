using Microsoft.Win32;
using PS3SaveEditor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class SerialValidateGG : Form

      // Token: 0x0600072D RID: 1837 RVA: 0x00029BF4 File Offset: 0x00027DF4
  {
        public const string SERIAL_VALIDATE_URL = "{0}/ps4auth";
    public const string LICNESE_INFO = "{{\"action\":\"ACTIVATE_LICENSE\",\"license\":\"{0}\"}}";
    private const string REGISTER_UID = "{{\"action\":\"REGISTER_UUID\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}";
    private string m_serial;
    private string m_hash;
    private SerialValidateGG.CloseDelegate CloseForm;
    private SerialValidateGG.UpdateStatusDelegate UpdateStatus;
    private SerialValidateGG.EnableOkDelegate EnableOk;
    private int m_retryCount;
    private bool m_bRetry;
    private IContainer components;
    private Label label1;
    private Panel panel1;
    private Label lblInstruction;
    private Label lblInstruction2;
    private TextBox txtSerial4;
    private TextBox txtSerial3;
    private TextBox txtSerial2;
    private TextBox txtSerial1;
    private Button btnCancel;
    private Button btnOk;
    private Label label4;
    private Label label3;
    private Label label2;

    public SerialValidateGG()
    {
      this.InitializeComponent();
      this.BackColor = System.Drawing.Color.FromArgb(247, 74, 20);
      this.panel1.BackColor = System.Drawing.Color.FromArgb(247, 74, 20);
      this.panel1.BackColor = System.Drawing.Color.FromArgb((int) sbyte.MaxValue, 247, 74, 20);
      this.lblInstruction.BackColor = System.Drawing.Color.Transparent;
      this.lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
      this.lblInstruction2.BackColor = System.Drawing.Color.Transparent;
      this.label1.BackColor = this.label2.BackColor = this.label3.BackColor = this.label4.BackColor = System.Drawing.Color.Transparent;
      this.UpdateStatus = new SerialValidateGG.UpdateStatusDelegate(this.UpdateStatusSafe);
      this.CloseForm = new SerialValidateGG.CloseDelegate(this.CloseFormSafe);
      this.EnableOk = new SerialValidateGG.EnableOkDelegate(this.EnableOkSafe);
      Util.SetForegroundWindow(this.Handle);
      this.CenterToScreen();
      this.txtSerial1.TextChanged += new EventHandler(this.txtSerial_TextChanged);
      this.txtSerial1.KeyDown += new KeyEventHandler(this.txtSerial_KeyDown);
      this.txtSerial1.KeyPress += new KeyPressEventHandler(this.txtSerial_KeyPress);
      this.txtSerial2.TextChanged += new EventHandler(this.txtSerial_TextChanged);
      this.txtSerial2.KeyDown += new KeyEventHandler(this.txtSerial_KeyDown);
      this.txtSerial2.KeyPress += new KeyPressEventHandler(this.txtSerial_KeyPress);
      this.txtSerial3.TextChanged += new EventHandler(this.txtSerial_TextChanged);
      this.txtSerial3.KeyDown += new KeyEventHandler(this.txtSerial_KeyDown);
      this.txtSerial3.KeyPress += new KeyPressEventHandler(this.txtSerial_KeyPress);
      this.txtSerial4.TextChanged += new EventHandler(this.txtSerial_TextChanged);
      this.txtSerial4.KeyDown += new KeyEventHandler(this.txtSerial_KeyDown);
      this.txtSerial4.KeyPress += new KeyPressEventHandler(this.txtSerial_KeyPress);
      this.lblInstruction.Text = "";
      this.lblInstruction2.Text = "Pon la Serial";
      this.Load += new EventHandler(this.SerialValidateGG_Load);
      this.btnOk.Enabled = false;
      if (this.m_serial != null)
        return;
      this.label1.Text = "";
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, System.Drawing.Color.FromArgb(0, 138, 213), System.Drawing.Color.FromArgb(0, 44, 101), 90f))
        e.Graphics.FillRectangle((Brush) linearGradientBrush, this.ClientRectangle);
    }

    private void CheckForDevice()
    {
      Thread.Sleep(10000);
      this.m_serial = (string) null;
      this.FindGGUSB();
      if (this.m_serial == null)
        return;
      if (this.label1.IsHandleCreated)
        this.label1.Invoke((Delegate) this.UpdateStatus, (object) "Please wait. Registering Game Genie Save Editor for PS3.");
      this.RegisterSerial();
    }

    private void UpdateStatusSafe(string status)
    {
      this.label1.Text = status;
    }

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 537 && m.WParam.ToInt32() == 32768)
      {
        int num = m.LParam != IntPtr.Zero ? 1 : 0;
      }
      base.WndProc(ref m);
    }

    private void SerialValidateGG_Load(object sender, EventArgs e)
    {
      this.txtSerial1.Select();
      string str = this.m_serial;
    }

    private void RegisterSerial()
    {
      try
      {
        WebClientEx webClientEx = new WebClientEx();
        webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
        this.m_hash = SerialValidateGG.ComputeHash(this.m_serial);
        if (string.IsNullOrEmpty(Util.GetUID(false, true)))
        {
          int num = (int) MessageBox.Show("There appears to have been an issue activating. Please contact support.");
        }
        else
        {
          string uriString = string.Format("{0}/ps4auth", (object) Util.GetBaseUrl(), (object) this.m_hash);
          webClientEx.DownloadStringAsync(new Uri(uriString, UriKind.Absolute));
          webClientEx.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.client_DownloadStringCompleted);
        }
      }
      catch (Exception ex)
      {
        int num1 = (int) MessageBox.Show(ex.Message, ex.StackTrace);
      }
    }

    public static string ComputeHash(string serial)
    {
      string str = "";
      byte[] buffer = new byte[32];
      byte[] numArray1 = new byte[16];
      byte[] numArray2 = new byte[16]
      {
        (byte) 59,
        (byte) 67,
        (byte) 235,
        (byte) 54,
        (byte) 183,
        (byte) 124,
        (byte) 22,
        (byte) 65,
        (byte) 172,
        (byte) 154,
        (byte) 31,
        (byte) 14,
        (byte) 188,
        (byte) 91,
        (byte) 48,
        (byte) 41
      };
      long num = long.Parse(serial, NumberStyles.HexNumber);
      byte[] numArray3 = (byte[]) null;
      if (serial.Length == 16)
      {
        byte[] bytes = BitConverter.GetBytes(num);
        Array.Reverse((Array) bytes, 0, bytes.Length);
        Array.Copy((Array) bytes, (Array) numArray1, bytes.Length);
        for (int index = 0; index < 8; ++index)
          buffer[index] = (byte) ((uint) numArray1[index] ^ (uint) numArray2[index]);
        Array.Copy((Array) Encoding.ASCII.GetBytes("GameGenie"), 0, (Array) buffer, 8, "GameGenie".Length);
        numArray3 = SHA1.Create().ComputeHash(buffer, 0, 8 + "GameGenie".Length);
      }
      else if (serial.Length == 20)
      {
        byte[] bytes = BitConverter.GetBytes(num);
        Array.Reverse((Array) bytes, 0, bytes.Length);
        Array.Copy((Array) bytes, 0, (Array) numArray1, 4, bytes.Length);
        for (int index = 0; index < 12; ++index)
          buffer[index] = (byte) ((uint) numArray1[index] ^ (uint) numArray2[index]);
        Array.Copy((Array) Encoding.ASCII.GetBytes("GameGenie"), 0, (Array) buffer, 12, "GameGenie".Length);
        numArray3 = SHA1.Create().ComputeHash(buffer, 0, 12 + "GameGenie".Length);
      }
      if (numArray3 != null)
      {
        for (int index = 0; index < numArray3.Length; ++index)
          str += numArray3[index].ToString("X2");
      }
      return str;
    }

    private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.Invoke((Delegate) this.CloseForm, (object) false);
      }
      else
      {
        string result = e.Result;
        if (result == null)
        {
          this.Invoke((Delegate) this.CloseForm, (object) false);
        }
        else
        {
          if (result.IndexOf('#') > 0)
          {
            string[] strArray = result.Split('#');
            if (strArray.Length > 1)
            {
              if (strArray[0] == "4")
              {
                this.Invoke((Delegate) this.CloseForm, (object) false);
                return;
              }
              if (strArray[0] == "5")
              {
                this.Invoke((Delegate) this.CloseForm, (object) false);
                return;
              }
            }
          }
          else
          {
            if (result.ToLower() == "toomanytimes" || result.ToLower().Contains("too many"))
            {
              this.Invoke((Delegate) this.CloseForm, (object) false);
              return;
            }
            if (result == null || result.ToLower().Contains("error") || result.ToLower().Contains("not found"))
            {
              string str = result.Replace("ERROR", "");
              if (!str.Contains("1002"))
              {
                if (str.Contains("1014"))
                {
                  this.Invoke((Delegate) this.CloseForm, (object) false);
                  return;
                }
                if (str.Contains("1005"))
                {
                  this.Invoke((Delegate) this.CloseForm, (object) false);
                  return;
                }
                if (str.Contains("1007"))
                {
                  Util.GetUID(true, true);
                  this.RegisterSerial();
                }
                else
                {

                  ++this.m_retryCount;
                  if (this.m_retryCount >= 3)
                  {
                    this.Invoke((Delegate) this.CloseForm, (object) false);
                    return;
                  }
                  if (this.m_serial == null)
                    this.btnOk.Invoke((Delegate) this.EnableOk, (object) true);
                  else
                    this.btnOk.Invoke((Delegate) this.EnableOk, (object) false);
                  this.label1.Invoke((Delegate) this.UpdateStatus, (object) "");
                  return;
                }
              }
            }
          }
          RegistryKey registryKey = Registry.CurrentUser;
          RegistryKey subKey = registryKey.CreateSubKey(Util.GetRegistryBase());
          if (this.m_serial == null)
          {
            this.m_hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(string.Format("{0}-{1}-{2}-{3}", (object) this.txtSerial1.Text, (object) this.txtSerial2.Text, (object) this.txtSerial3.Text, (object) this.txtSerial4.Text))));
            this.m_hash = this.m_hash.Replace("-", "");
          }
          else
            this.m_hash = SerialValidateGG.ComputeHash(this.m_serial);
          subKey.SetValue("Hash", (object) this.m_hash.ToUpper());
          subKey.SetValue("BackupSaves", (object) "true");
          string str1 = string.Format("{0}-{1}-{2}-{3}", (object) this.txtSerial1.Text, (object) this.txtSerial2.Text, (object) this.txtSerial3.Text, (object) this.txtSerial4.Text);
          subKey.SetValue("Serial", (object) str1);
          subKey.Close();
          registryKey.Close();
          try
          {
            if (!this.IsHandleCreated)
              return;
            this.Invoke((Delegate) this.CloseForm, (object) true);
          }
          catch
          {
          }
        }
      }
    }

    private void EnableOkSafe(bool bEnable)
    {
      this.btnOk.Enabled = bEnable;
    }

    private void CloseFormSafe(bool bSuccess)
    {
      if (!bSuccess)
        this.DialogResult = DialogResult.Abort;
      else
        this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void FindGGUSB()
    {
      foreach (USB.USBController usbController in USB.GetHostControllers())
        this.ProcessHub(usbController.GetRootHub());
    }

    private void ProcessHub(USB.USBHub hub)
    {
      foreach (USB.USBPort usbPort in hub.GetPorts())
      {
        if (usbPort.IsHub)
          this.ProcessHub(usbPort.GetHub());
        USB.USBDevice device = usbPort.GetDevice();
        if (device != null && device.DeviceManufacturer != null && (device.DeviceManufacturer.ToLower() == "dpdev" && device.DeviceProduct != null) && device.DeviceProduct.ToLower() == "gamegenie")
          this.m_serial = device.SerialNumber;
      }
    }

    private bool ValidateSerial()
    {
      for (int index = 1; index <= 4; ++index)
      {
        TextBox textBox = this.Controls.Find("txtSerial" + (object) index, true)[0] as TextBox;
        if (textBox.Text.Length < 4)
        {
          textBox.Focus();
          return false;
        }
      }
      return true;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.m_serial = (string) null;
      try
      {
        if (!this.ValidateSerial())
          return;
        this.btnOk.Invoke((Delegate) this.EnableOk, (object) false);
        this.RegisterLicense();
      }
      catch (Exception ex)
      {
        if (this.m_serial == null)
          this.btnOk.Enabled = true;
        this.btnCancel.Enabled = true;
      }
    }

    private void RegisterLicense()
    {
      WebClientEx webClientEx = new WebClientEx();
      webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
      webClientEx.Headers[HttpRequestHeader.ContentType] = "application/json";
      webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
      webClientEx.UploadDataCompleted += new UploadDataCompletedEventHandler(this.client_UploadDataCompleted);
      webClientEx.UploadDataAsync(new Uri(string.Format("{0}/ps4auth", (object) Util.GetAuthBaseUrl()), UriKind.Absolute), "POST", Encoding.ASCII.GetBytes(string.Format("{{\"action\":\"ACTIVATE_LICENSE\",\"license\":\"{0}\"}}", (object) string.Format("{0}-{1}-{2}-{3}", (object) this.txtSerial1.Text, (object) this.txtSerial2.Text, (object) this.txtSerial3.Text, (object) this.txtSerial4.Text))));
    }

    private void client_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
    {
      if (e.Error is WebException && !this.m_bRetry)
      {
        this.m_bRetry = true;
        Util.ChangeAuthServer();
        this.btnOk_Click((object) null, (EventArgs) null);
      }
      else if (e.Error != null)
      {
        this.btnOk.Invoke((Delegate) this.EnableOk, (object) true);
      }
      else
      {
        Dictionary<string, object> dictionary = new JavaScriptSerializer().Deserialize(Encoding.ASCII.GetString(e.Result), typeof (Dictionary<string, object>)) as Dictionary<string, object>;
        if ((string) dictionary["status"] == "ERROR" && dictionary["code"].ToString() != "4020")
        {
          this.btnOk.Invoke((Delegate) this.EnableOk, (object) true);
          this.label1.Invoke((Delegate) this.UpdateStatus, (object) "");
          
        }
        else
        {
          Util.SetRegistryValue("User", (string) dictionary["userid"]);
          this.RegisterUID();
        }
      }
    }

    private void RegisterUID()
    {
      string uid = Util.GetUID(false, false);
      WebClientEx webClientEx = new WebClientEx();
      webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
      webClientEx.Headers[HttpRequestHeader.ContentType] = "application/json";
      webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
      webClientEx.UploadDataCompleted += new UploadDataCompletedEventHandler(this.client2_UploadDataCompleted);
      webClientEx.UploadDataAsync(new Uri(string.Format("{0}/ps4auth", (object) Util.GetAuthBaseUrl()), UriKind.Absolute), "POST", Encoding.ASCII.GetBytes(string.Format("{{\"action\":\"REGISTER_UUID\",\"userid\":\"{0}\",\"uuid\":\"{1}\"}}", (object) Util.GetRegistryValue("User"), (object) uid)));
    }

    private void client2_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.btnOk.Invoke((Delegate) this.EnableOk, (object) true);
      }
      else
      {
        Dictionary<string, object> dictionary = new JavaScriptSerializer().Deserialize(Encoding.ASCII.GetString(e.Result), typeof (Dictionary<string, object>)) as Dictionary<string, object>;
        if ((string) dictionary["status"] == "ERROR" && dictionary["code"].ToString() != "4021")
        {
          this.btnOk.Invoke((Delegate) this.EnableOk, (object) true);
          this.label1.Invoke((Delegate) this.UpdateStatus, (object) "");
        }
        else
          this.Invoke((Delegate) this.CloseForm, (object) true);
      }
    }

    private void txtSerial_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || (sender as TextBox).Text.Length != 4)
        return;
      e.SuppressKeyPress = true;
    }

    private void txtSerial_KeyPress(object sender, KeyPressEventArgs e)
    {
      TextBox textBox1 = sender as TextBox;
      if (textBox1.Name == "txtSerial1" || textBox1.Text.Length != 0 || (int) e.KeyChar != 8)
        return;
      Control[] controlArray = textBox1.Parent.Controls.Find("txtSerial" + (object) (char) ((uint) textBox1.Name[9] - 1U), true);
      if (controlArray.Length != 1)
        return;
      TextBox textBox2 = controlArray[0] as TextBox;
      if (textBox2.Text.Length > 0)
        textBox2.SelectionStart = textBox2.Text.Length;
      controlArray[0].Focus();
    }

    private void txtSerial_TextChanged(object sender, EventArgs e)
    {
      TextBox textBox = sender as TextBox;
      int selectionStart = textBox.SelectionStart;
      textBox.Text = Regex.Replace(textBox.Text, "[^0-9a-zA-Z ]", "");
      textBox.SelectionStart = selectionStart;
      if (textBox.Name == "txtSerial1")
      {
        string[] strArray = Clipboard.GetText().Split('-');
        if (strArray.Length == 4)
        {
          strArray[0] = strArray[0].Trim();
          strArray[1] = strArray[1].Trim();
          strArray[2] = strArray[2].Trim();
          strArray[3] = strArray[3].Trim();
          if (strArray[0].Length != 4 || strArray[1].Length != 4 || (strArray[2].Length != 4 || strArray[3].Length != 4))
            return;
          Clipboard.Clear();
          this.txtSerial1.Text = strArray[0];
          this.txtSerial2.Text = strArray[1];
          this.txtSerial3.Text = strArray[2];
          this.txtSerial4.Text = strArray[3];
        }
      }
      if (textBox.Text.Length == 4)
      {
        Control[] controlArray = textBox.Parent.Controls.Find("txtSerial" + (object) (char) ((uint) textBox.Name[9] + 1U), true);
        if (controlArray.Length == 1)
          controlArray[0].Focus();
      }
      if (this.txtSerial1.Text.Length == 4 && this.txtSerial2.Text.Length == 4 && (this.txtSerial3.Text.Length == 4 && this.txtSerial4.Text.Length == 4))
      {
        this.btnOk.Enabled = true;
        this.btnOk.Focus();
      }
      else
        this.btnOk.Enabled = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtSerial4 = new System.Windows.Forms.TextBox();
            this.txtSerial3 = new System.Windows.Forms.TextBox();
            this.txtSerial2 = new System.Windows.Forms.TextBox();
            this.txtSerial1 = new System.Windows.Forms.TextBox();
            this.lblInstruction2 = new System.Windows.Forms.Label();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(65, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 15);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.txtSerial4);
            this.panel1.Controls.Add(this.txtSerial3);
            this.panel1.Controls.Add(this.txtSerial2);
            this.panel1.Controls.Add(this.txtSerial1);
            this.panel1.Controls.Add(this.lblInstruction2);
            this.panel1.Controls.Add(this.lblInstruction);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(10, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(439, 120);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(218, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(163, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(107, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "-";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(3, 54);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.ForeColor = System.Drawing.Color.Black;
            this.btnOk.Location = new System.Drawing.Point(293, 51);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Entrar";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtSerial4
            // 
            this.txtSerial4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerial4.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial4.Location = new System.Drawing.Point(232, 51);
            this.txtSerial4.MaxLength = 4;
            this.txtSerial4.Name = "txtSerial4";
            this.txtSerial4.Size = new System.Drawing.Size(40, 21);
            this.txtSerial4.TabIndex = 7;
            // 
            // txtSerial3
            // 
            this.txtSerial3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerial3.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial3.Location = new System.Drawing.Point(176, 51);
            this.txtSerial3.MaxLength = 4;
            this.txtSerial3.Name = "txtSerial3";
            this.txtSerial3.Size = new System.Drawing.Size(40, 21);
            this.txtSerial3.TabIndex = 6;
            // 
            // txtSerial2
            // 
            this.txtSerial2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerial2.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial2.Location = new System.Drawing.Point(120, 51);
            this.txtSerial2.MaxLength = 4;
            this.txtSerial2.Name = "txtSerial2";
            this.txtSerial2.Size = new System.Drawing.Size(40, 21);
            this.txtSerial2.TabIndex = 5;
            // 
            // txtSerial1
            // 
            this.txtSerial1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerial1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial1.Location = new System.Drawing.Point(64, 51);
            this.txtSerial1.MaxLength = 4;
            this.txtSerial1.Name = "txtSerial1";
            this.txtSerial1.Size = new System.Drawing.Size(40, 21);
            this.txtSerial1.TabIndex = 4;
            // 
            // lblInstruction2
            // 
            this.lblInstruction2.Location = new System.Drawing.Point(5, 25);
            this.lblInstruction2.Name = "lblInstruction2";
            this.lblInstruction2.Size = new System.Drawing.Size(430, 13);
            this.lblInstruction2.TabIndex = 2;
            this.lblInstruction2.Text = "Sample Text";
            this.lblInstruction2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Location = new System.Drawing.Point(13, 8);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(0, 13);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SerialValidateGG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 142);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SerialValidateGG";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Licencia de Save Special";
            this.Load += new System.EventHandler(this.SerialValidateGG_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

    }

    private delegate void CloseDelegate(bool bSuccess);

    private delegate void UpdateStatusDelegate(string status);

    private delegate void EnableOkDelegate(bool bEnable);

        private void SerialValidateGG_Load_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
