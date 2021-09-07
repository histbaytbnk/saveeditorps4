
// Type: CustomControls.CustomHScrollbar


// Hacked by SystemAce

using PS3SaveEditor.CustomScrollbar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControls
{
  public class CustomHScrollbar : UserControl
  {
    protected Color moChannelColor = Color.Empty;
    protected int moLargeChange = 10;
    protected int moSmallChange = 1;
    protected int moMaximum = 100;
    protected Image moUpArrowImage;
    protected Image moDownArrowImage;
    protected Image moThumbArrowImage;
    protected Image moThumbRightImage;
    protected Image moThumbRightSpanImage;
    protected Image moThumbLeftImage;
    protected Image moThumbLeftSpanImage;
    protected Image moThumbMiddleImage;
    protected int moMinimum;
    protected int moValue;
    private int nClickPoint;
    protected int moThumbRight;
    protected bool moAutoSize;
    private bool moThumbDown;
    private bool moThumbDragging;

    [Category("Behavior")]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("LargeChange")]
    [Browsable(true)]
    public int LargeChange
    {
      get
      {
        return this.moLargeChange;
      }
      set
      {
        this.moLargeChange = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("SmallChange")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    public int SmallChange
    {
      get
      {
        return this.moSmallChange;
      }
      set
      {
        this.moSmallChange = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    [Category("Behavior")]
    [Description("Minimum")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public int Minimum
    {
      get
      {
        return this.moMinimum;
      }
      set
      {
        this.moMinimum = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Browsable(true)]
    [Description("Maximum")]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public int Maximum
    {
      get
      {
        return this.moMaximum;
      }
      set
      {
        this.moMaximum = value;
        this.Invalidate();
      }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Value")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Behavior")]
    public int Value
    {
      get
      {
        return this.moValue;
      }
      set
      {
        this.moValue = value;
        int num1 = this.Width - (this.LeftArrowImage.Width + this.RightArrowImage.Width);
        int num2 = (int) ((float) this.LargeChange / (float) this.Maximum * (float) num1);
        float num3;
        if (num2 > num1)
        {
          num2 = num1;
          num3 = (float) num1;
        }
        if (num2 < 56)
        {
          num2 = 56;
          num3 = 56f;
        }
        int num4 = num1 - num2;
        int num5 = this.Maximum - this.Minimum - this.LargeChange;
        float num6 = 0.0f;
        if (num5 != 0)
          num6 = (float) this.moValue / (float) num5;
        this.moThumbRight = (int) (num6 * (float) num4);
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Channel Color")]
    [Category("Skin")]
    [Browsable(true)]
    public Color ChannelColor
    {
      get
      {
        return this.moChannelColor;
      }
      set
      {
        this.moChannelColor = value;
      }
    }

    [DefaultValue(false)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Image LeftArrowImage
    {
      get
      {
        return this.moUpArrowImage;
      }
      set
      {
        this.moUpArrowImage = value;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Up Arrow Graphic")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    public Image RightArrowImage
    {
      get
      {
        return this.moDownArrowImage;
      }
      set
      {
        this.moDownArrowImage = value;
      }
    }

    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    public Image ThumbRightImage
    {
      get
      {
        return this.moThumbRightImage;
      }
      set
      {
        this.moThumbRightImage = value;
      }
    }

    [DefaultValue(false)]
    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [Category("Skin")]
    public Image ThumbRightSpanImage
    {
      get
      {
        return this.moThumbRightSpanImage;
      }
      set
      {
        this.moThumbRightSpanImage = value;
      }
    }

    [DefaultValue(false)]
    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [Category("Skin")]
    public Image ThumbLeftImage
    {
      get
      {
        return this.moThumbLeftImage;
      }
      set
      {
        this.moThumbLeftImage = value;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(false)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [Browsable(true)]
    public Image ThumbLeftSpanImage
    {
      get
      {
        return this.moThumbLeftSpanImage;
      }
      set
      {
        this.moThumbLeftSpanImage = value;
      }
    }

    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    public Image ThumbMiddleImage
    {
      get
      {
        return this.moThumbMiddleImage;
      }
      set
      {
        this.moThumbMiddleImage = value;
      }
    }

    public override bool AutoSize
    {
      get
      {
        return base.AutoSize;
      }
      set
      {
        base.AutoSize = value;
        if (!base.AutoSize)
          return;
        this.Width = this.moUpArrowImage.Width;
      }
    }

    public event EventHandler Scroll;

    public event EventHandler ValueChanged;

    public CustomHScrollbar()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.DoubleBuffer, true);
      this.moChannelColor = Color.FromArgb(51, 166, 3);
      this.LeftArrowImage = (Image) Resource.leftarrow;
      this.RightArrowImage = (Image) Resource.rightarrow;
      this.ThumbLeftImage = (Image) Resource.ThumbLeft;
      this.ThumbLeftSpanImage = (Image) Resource.ThumbSpanLeft;
      this.ThumbRightImage = (Image) Resource.ThumbRight;
      this.ThumbRightSpanImage = (Image) Resource.ThumbSpanRight;
      this.ThumbMiddleImage = (Image) Resource.ThumbMiddleH;
      this.Height = this.LeftArrowImage.Height;
      this.MinimumSize = new Size(this.LeftArrowImage.Width + this.RightArrowImage.Width + this.GetThumbWidth(), this.LeftArrowImage.Height);
    }

    private int GetThumbWidth()
    {
      int num1 = this.Width - (this.LeftArrowImage.Width + this.RightArrowImage.Width);
      int num2 = (int) ((float) this.LargeChange / (float) this.Maximum * (float) num1);
      float num3;
      if (num2 > num1)
      {
        num2 = num1;
        num3 = (float) num1;
      }
      if (num2 < 56)
      {
        num2 = 56;
        num3 = 56f;
      }
      return num2;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      if (this.LeftArrowImage != null)
        e.Graphics.DrawImage(this.LeftArrowImage, new Rectangle(new Point(0, 0), new Size(this.LeftArrowImage.Width, this.Height)));
      Brush brush1 = (Brush) new SolidBrush(this.moChannelColor);
      Brush brush2 = (Brush) new SolidBrush(Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
      e.Graphics.FillRectangle(brush2, new Rectangle(this.LeftArrowImage.Width, 0, this.Width - this.RightArrowImage.Width, 1));
      e.Graphics.FillRectangle(brush2, new Rectangle(this.LeftArrowImage.Width, this.Height - 1, this.Width - this.RightArrowImage.Width, this.Height));
      e.Graphics.FillRectangle(brush1, new Rectangle(this.LeftArrowImage.Width, 1, this.Width - this.RightArrowImage.Width, this.Height - 2));
      int num1 = this.Width - (this.LeftArrowImage.Width + this.RightArrowImage.Width);
      float num2 = (float) this.LargeChange / (float) this.Maximum * (float) num1;
      int num3 = (int) num2;
      if (num3 > num1)
      {
        num3 = num1;
        num2 = (float) num1;
      }
      if (num3 < 56)
        num2 = 56f;
      float num4 = (float) (((double) num2 - (double) (this.ThumbMiddleImage.Width + this.ThumbRightImage.Width + this.ThumbRightImage.Width)) / 2.0);
      int width = (int) num4;
      int x1 = this.moThumbRight + this.LeftArrowImage.Width;
      e.Graphics.DrawImage(this.ThumbLeftImage, new Rectangle(x1, 1, this.ThumbLeftImage.Width, this.Height - 2));
      int x2 = x1 + this.ThumbLeftImage.Width;
      Rectangle rect = new Rectangle(x2, 1, width, this.Height - 2);
      e.Graphics.DrawImage(this.ThumbLeftSpanImage, (float) x2, 1f, num4 * 2f, (float) this.Height - 2f);
      int x3 = x2 + width;
      e.Graphics.DrawImage(this.ThumbMiddleImage, new Rectangle(x3, 1, this.ThumbMiddleImage.Width, this.Height - 2));
      int x4 = x3 + this.ThumbMiddleImage.Width;
      rect = new Rectangle(x4, 1, width * 2, this.Height - 2);
      e.Graphics.DrawImage(this.ThumbRightSpanImage, rect);
      int x5 = x4 + width;
      e.Graphics.DrawImage(this.ThumbRightImage, new Rectangle(x5, 1, width, this.Height - 2));
      if (this.RightArrowImage == null)
        return;
      e.Graphics.DrawImage(this.RightArrowImage, new Rectangle(new Point(this.Width - this.RightArrowImage.Width, 0), new Size(this.RightArrowImage.Width, this.Height)));
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.Name = "CustomHScrollbar";
      this.MouseDown += new MouseEventHandler(this.CustomScrollbar_MouseDown);
      this.MouseMove += new MouseEventHandler(this.CustomScrollbar_MouseMove);
      this.MouseUp += new MouseEventHandler(this.CustomScrollbar_MouseUp);
      this.ResumeLayout(false);
    }

    private void CustomScrollbar_MouseDown(object sender, MouseEventArgs e)
    {
      Point pt = this.PointToClient(Cursor.Position);
      int num1 = this.Width - (this.LeftArrowImage.Width + this.RightArrowImage.Width);
      int width = (int) ((float) this.LargeChange / (float) this.Maximum * (float) num1);
      float num2;
      if (width > num1)
      {
        width = num1;
        num2 = (float) num1;
      }
      if (width < 56)
      {
        width = 56;
        num2 = 56f;
      }
      int x = this.moThumbRight + this.LeftArrowImage.Width;
      if (new Rectangle(new Point(x, 1), new Size(width, this.ThumbMiddleImage.Height)).Contains(pt))
      {
        this.nClickPoint = pt.Y - x;
        this.moThumbDown = true;
      }
      if (new Rectangle(new Point(1, 0), new Size(this.LeftArrowImage.Width, this.LeftArrowImage.Height)).Contains(pt))
      {
        int num3 = this.Maximum - this.Minimum - this.LargeChange;
        int num4 = num1 - width;
        if (num3 > 0 && num4 > 0)
        {
          if (this.moThumbRight - this.SmallChange < 0)
            this.moThumbRight = 0;
          else
            this.moThumbRight -= this.SmallChange;
          this.moValue = (int) ((float) this.moThumbRight / (float) num4 * (float) (this.Maximum - this.LargeChange));
          if (this.ValueChanged != null)
            this.ValueChanged((object) this, new EventArgs());
          if (this.Scroll != null)
            this.Scroll((object) this, new EventArgs());
          this.Invalidate();
        }
      }
      if (!new Rectangle(new Point(this.LeftArrowImage.Width + num1, 1), new Size(this.LeftArrowImage.Width, this.LeftArrowImage.Height)).Contains(pt))
        return;
      int num5 = this.Maximum - this.Minimum - this.LargeChange;
      int num6 = num1 - width;
      if (num5 <= 0 || num6 <= 0)
        return;
      if (this.moThumbRight + this.SmallChange > num6)
        this.moThumbRight = num6;
      else
        this.moThumbRight += this.SmallChange;
      this.moValue = (int) ((float) this.moThumbRight / (float) num6 * (float) (this.Maximum - this.LargeChange));
      if (this.ValueChanged != null)
        this.ValueChanged((object) this, new EventArgs());
      if (this.Scroll != null)
        this.Scroll((object) this, new EventArgs());
      this.Invalidate();
    }

    private void CustomScrollbar_MouseUp(object sender, MouseEventArgs e)
    {
      this.moThumbDown = false;
      this.moThumbDragging = false;
    }

    private void MoveThumb(int x)
    {
      int num1 = this.Maximum - this.Minimum;
      int num2 = this.Width - (this.LeftArrowImage.Width + this.RightArrowImage.Width);
      int num3 = (int) ((float) this.LargeChange / (float) this.Maximum * (float) num2);
      float num4;
      if (num3 > num2)
      {
        num3 = num2;
        num4 = (float) num2;
      }
      if (num3 < 56)
      {
        num3 = 56;
        num4 = 56f;
      }
      int num5 = this.nClickPoint;
      int num6 = num2 - num3;
      if (!this.moThumbDown || num1 <= 0 || num6 <= 0)
        return;
      int num7 = x - (this.LeftArrowImage.Width + num5);
      int num8;
      this.moThumbRight = num7 >= 0 ? (num7 <= num6 ? x - (this.LeftArrowImage.Width + num5) : (num8 = num6)) : (num8 = 0);
      this.moValue = (int) ((float) this.moThumbRight / (float) num6 * (float) (this.Maximum - this.LargeChange));
      Application.DoEvents();
      this.Invalidate();
    }

    private void CustomScrollbar_MouseMove(object sender, MouseEventArgs e)
    {
      if (this.moThumbDown)
        this.moThumbDragging = true;
      if (this.moThumbDragging)
        this.MoveThumb(e.X);
      if (this.ValueChanged != null)
        this.ValueChanged((object) this, new EventArgs());
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, new EventArgs());
    }
  }
}
