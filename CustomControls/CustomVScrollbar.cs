
// Type: CustomControls.CustomVScrollbar


// Hacked by SystemAce

using PS3SaveEditor.CustomScrollbar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControls
{
  public class CustomVScrollbar : UserControl
  {
    protected Color moChannelColor = Color.Empty;
    protected int moLargeChange = 10;
    protected int moSmallChange = 1;
    protected int moMaximum = 100;
    protected Image moUpArrowImage;
    protected Image moDownArrowImage;
    protected Image moThumbArrowImage;
    protected Image moThumbTopImage;
    protected Image moThumbTopSpanImage;
    protected Image moThumbBottomImage;
    protected Image moThumbBottomSpanImage;
    protected Image moThumbMiddleImage;
    protected int moMinimum;
    protected int moValue;
    private int nClickPoint;
    protected int moThumbTop;
    protected bool moAutoSize;
    private bool moThumbDown;
    private bool moThumbDragging;

    [Category("Behavior")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Description("LargeChange")]
    [EditorBrowsable(EditorBrowsableState.Always)]
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

    [Description("SmallChange")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Behavior")]
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

    [Description("Minimum")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Behavior")]
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

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(false)]
    [Category("Behavior")]
    [Description("Maximum")]
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

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Value")]
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
        int num1 = this.Height - (this.UpArrowImage.Height + this.DownArrowImage.Height);
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
        this.moThumbTop = (int) (num6 * (float) num4);
        this.Invalidate();
      }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Channel Color")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
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

    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Image UpArrowImage
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

    [Category("Skin")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Up Arrow Graphic")]
    [DefaultValue(false)]
    public Image DownArrowImage
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

    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Image ThumbTopImage
    {
      get
      {
        return this.moThumbTopImage;
      }
      set
      {
        this.moThumbTopImage = value;
      }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [DefaultValue(false)]
    public Image ThumbTopSpanImage
    {
      get
      {
        return this.moThumbTopSpanImage;
      }
      set
      {
        this.moThumbTopSpanImage = value;
      }
    }

    [Category("Skin")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Up Arrow Graphic")]
    [DefaultValue(false)]
    public Image ThumbBottomImage
    {
      get
      {
        return this.moThumbBottomImage;
      }
      set
      {
        this.moThumbBottomImage = value;
      }
    }

    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Skin")]
    [Description("Up Arrow Graphic")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Image ThumbBottomSpanImage
    {
      get
      {
        return this.moThumbBottomSpanImage;
      }
      set
      {
        this.moThumbBottomSpanImage = value;
      }
    }

    [Description("Up Arrow Graphic")]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
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

    public CustomVScrollbar()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.DoubleBuffer, true);
      this.moChannelColor = Color.FromArgb(125, 45, 17);
      this.UpArrowImage = (Image) Resource.uparrow;
      this.DownArrowImage = (Image) Resource.downarrow;
      this.ThumbBottomImage = (Image) Resource.ThumbBottom;
      this.ThumbBottomSpanImage = (Image) Resource.ThumbSpanBottom;
      this.ThumbTopImage = (Image) Resource.ThumbTop;
      this.ThumbTopSpanImage = (Image) Resource.ThumbSpanTop;
      this.ThumbMiddleImage = (Image) Resource.ThumbMiddle;
      this.Width = this.UpArrowImage.Width;
      this.MinimumSize = new Size(this.UpArrowImage.Width, this.UpArrowImage.Height + this.DownArrowImage.Height + this.GetThumbHeight());
    }

    private int GetThumbHeight()
    {
      int num1 = this.Height - (this.UpArrowImage.Height + this.DownArrowImage.Height);
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
      if (this.UpArrowImage != null)
        e.Graphics.DrawImage(this.UpArrowImage, new Rectangle(new Point(0, 0), new Size(this.Width, this.UpArrowImage.Height)));
      Brush brush = (Brush) new SolidBrush(this.moChannelColor);
      SolidBrush solidBrush = new SolidBrush(this.moChannelColor);
      e.Graphics.FillRectangle(brush, new Rectangle(0, this.UpArrowImage.Height, this.Width, this.Height - this.DownArrowImage.Height));
      int num1 = this.Height - (this.UpArrowImage.Height + this.DownArrowImage.Height);
      float num2 = (float) this.LargeChange / (float) this.Maximum * (float) num1;
      int num3 = (int) num2;
      if (num3 > num1)
      {
        num3 = num1;
        num2 = (float) num1;
      }
      if (num3 < 56)
        num2 = 56f;
      float num4 = (float) (((double) num2 - (double) (this.ThumbMiddleImage.Height + this.ThumbTopImage.Height + this.ThumbBottomImage.Height)) / 2.0);
      int height = (int) num4;
      int y1 = this.moThumbTop + this.UpArrowImage.Height;
      e.Graphics.DrawImage(this.ThumbTopImage, new Rectangle(1, y1, this.Width - 2, this.ThumbTopImage.Height));
      int y2 = y1 + this.ThumbTopImage.Height;
      Rectangle rect = new Rectangle(1, y2, this.Width - 2, height);
      e.Graphics.DrawImage(this.ThumbTopSpanImage, 1f, (float) y2, (float) this.Width - 2f, num4 * 2f);
      int y3 = y2 + height;
      e.Graphics.DrawImage(this.ThumbMiddleImage, new Rectangle(1, y3, this.Width - 2, this.ThumbMiddleImage.Height));
      int y4 = y3 + this.ThumbMiddleImage.Height;
      rect = new Rectangle(1, y4, this.Width - 2, height * 2);
      e.Graphics.DrawImage(this.ThumbBottomSpanImage, rect);
      int y5 = y4 + height;
      e.Graphics.DrawImage(this.ThumbBottomImage, new Rectangle(1, y5, this.Width - 2, height));
      if (this.DownArrowImage == null)
        return;
      e.Graphics.DrawImage(this.DownArrowImage, new Rectangle(new Point(0, this.Height - this.DownArrowImage.Height), new Size(this.Width, this.DownArrowImage.Height)));
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.Name = "CustomVScrollbar";
      this.MouseDown += new MouseEventHandler(this.CustomScrollbar_MouseDown);
      this.MouseMove += new MouseEventHandler(this.CustomScrollbar_MouseMove);
      this.MouseUp += new MouseEventHandler(this.CustomScrollbar_MouseUp);
      this.ResumeLayout(false);
    }

    private void CustomScrollbar_MouseDown(object sender, MouseEventArgs e)
    {
      Point pt = this.PointToClient(Cursor.Position);
      int num1 = this.Height - (this.UpArrowImage.Height + this.DownArrowImage.Height);
      int height = (int) ((float) this.LargeChange / (float) this.Maximum * (float) num1);
      float num2;
      if (height > num1)
      {
        height = num1;
        num2 = (float) num1;
      }
      if (height < 56)
      {
        height = 56;
        num2 = 56f;
      }
      int y = this.moThumbTop + this.UpArrowImage.Height;
      if (new Rectangle(new Point(1, y), new Size(this.ThumbMiddleImage.Width, height)).Contains(pt))
      {
        this.nClickPoint = pt.Y - y;
        this.moThumbDown = true;
      }
      if (new Rectangle(new Point(1, 0), new Size(this.UpArrowImage.Width, this.UpArrowImage.Height)).Contains(pt))
      {
        int num3 = this.Maximum - this.Minimum - this.LargeChange;
        int num4 = num1 - height;
        if (num3 > 0 && num4 > 0)
        {
          if (this.moThumbTop - this.SmallChange < 0)
            this.moThumbTop = 0;
          else
            this.moThumbTop -= this.SmallChange;
          this.moValue = (int) ((float) this.moThumbTop / (float) num4 * (float) (this.Maximum - this.LargeChange));
          if (this.ValueChanged != null)
            this.ValueChanged((object) this, new EventArgs());
          if (this.Scroll != null)
            this.Scroll((object) this, new EventArgs());
          this.Invalidate();
        }
      }
      if (!new Rectangle(new Point(1, this.UpArrowImage.Height + num1), new Size(this.UpArrowImage.Width, this.UpArrowImage.Height)).Contains(pt))
        return;
      int num5 = this.Maximum - this.Minimum - this.LargeChange;
      int num6 = num1 - height;
      if (num5 <= 0 || num6 <= 0)
        return;
      if (this.moThumbTop + this.SmallChange > num6)
        this.moThumbTop = num6;
      else
        this.moThumbTop += this.SmallChange;
      this.moValue = (int) ((float) this.moThumbTop / (float) num6 * (float) (this.Maximum - this.LargeChange));
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

    private void MoveThumb(int y)
    {
      int num1 = this.Maximum - this.Minimum;
      int num2 = this.Height - (this.UpArrowImage.Height + this.DownArrowImage.Height);
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
      int num7 = y - (this.UpArrowImage.Height + num5);
      int num8;
      this.moThumbTop = num7 >= 0 ? (num7 <= num6 ? y - (this.UpArrowImage.Height + num5) : (num8 = num6)) : (num8 = 0);
      this.moValue = (int) ((float) this.moThumbTop / (float) num6 * (float) (this.Maximum - this.LargeChange));
      Application.DoEvents();
      this.Invalidate();
    }

    private void CustomScrollbar_MouseMove(object sender, MouseEventArgs e)
    {
      if (this.moThumbDown)
        this.moThumbDragging = true;
      if (this.moThumbDragging)
        this.MoveThumb(e.Y);
      if (this.ValueChanged != null)
        this.ValueChanged((object) this, new EventArgs());
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, new EventArgs());
    }
  }
}
