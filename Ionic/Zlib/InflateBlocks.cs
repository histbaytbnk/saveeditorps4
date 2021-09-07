
// Type: Ionic.Zlib.InflateBlocks


// Hacked by SystemAce

using System;

namespace Ionic.Zlib
{
  internal sealed class InflateBlocks
  {
    internal static readonly int[] border = new int[19]
    {
      16,
      17,
      18,
      0,
      8,
      7,
      9,
      6,
      10,
      5,
      11,
      4,
      12,
      3,
      13,
      2,
      14,
      1,
      15
    };
    internal int[] bb = new int[1];
    internal int[] tb = new int[1];
    internal InflateCodes codes = new InflateCodes();
    internal InfTree inftree = new InfTree();
    private const int MANY = 1440;
    private InflateBlocks.InflateBlockMode mode;
    internal int left;
    internal int table;
    internal int index;
    internal int[] blens;
    internal int last;
    internal ZlibCodec _codec;
    internal int bitk;
    internal int bitb;
    internal int[] hufts;
    internal byte[] window;
    internal int end;
    internal int readAt;
    internal int writeAt;
    internal object checkfn;
    internal uint check;

    internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
    {
      this._codec = codec;
      this.hufts = new int[4320];
      this.window = new byte[w];
      this.end = w;
      this.checkfn = checkfn;
      this.mode = InflateBlocks.InflateBlockMode.TYPE;
      int num = (int) this.Reset();
    }

    internal uint Reset()
    {
      uint num = this.check;
      this.mode = InflateBlocks.InflateBlockMode.TYPE;
      this.bitk = 0;
      this.bitb = 0;
      this.readAt = this.writeAt = 0;
      if (this.checkfn != null)
        this._codec._Adler32 = this.check = Adler.Adler32(0U, (byte[]) null, 0, 0);
      return num;
    }

    internal int Process(int r)
    {
      int sourceIndex = this._codec.NextIn;
      int num1 = this._codec.AvailableBytesIn;
      int num2 = this.bitb;
      int num3 = this.bitk;
      int destinationIndex = this.writeAt;
      int num4 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
      int num5;
      int num6;
      while (true)
      {
        int length1;
                do
                {
                    switch (this.mode)
                    {
                        case InflateBlocks.InflateBlockMode.TYPE:
                            while (num3 < 3)
                            {
                                if (num1 != 0)
                                {
                                    r = 0;
                                    --num1;
                                    num2 |= ((int)this._codec.InputBuffer[sourceIndex++] & (int)byte.MaxValue) << num3;
                                    num3 += 8;
                                }
                                else
                                {
                                    this.bitb = num2;
                                    this.bitk = num3;
                                    this._codec.AvailableBytesIn = num1;
                                    this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                    this._codec.NextIn = sourceIndex;
                                    this.writeAt = destinationIndex;
                                    return this.Flush(r);
                                }
                            }
                            int num7 = num2 & 7;
                            this.last = num7 & 1;
                            switch ((uint)num7 >> 1)
                            {
                                case 0U:
                                    int num8 = num2 >> 3;
                                    int num9 = num3 - 3;
                                    int num10 = num9 & 7;
                                    num2 = num8 >> num10;
                                    num3 = num9 - num10;
                                    this.mode = InflateBlocks.InflateBlockMode.LENS;
                                    continue;
                                case 1U:
                                    int[] bl1 = new int[1];
                                    int[] bd1 = new int[1];
                                    int[][] tl1 = new int[1][];
                                    int[][] td1 = new int[1][];
                                    InfTree.inflate_trees_fixed(bl1, bd1, tl1, td1, this._codec);
                                    this.codes.Init(bl1[0], bd1[0], tl1[0], 0, td1[0], 0);
                                    num2 >>= 3;
                                    num3 -= 3;
                                    this.mode = InflateBlocks.InflateBlockMode.CODES;
                                    continue;
                                case 2U:
                                    num2 >>= 3;
                                    num3 -= 3;
                                    this.mode = InflateBlocks.InflateBlockMode.TABLE;
                                    continue;
                                case 3U:
                                    int num11 = num2 >> 3;
                                    int num12 = num3 - 3;
                                    this.mode = InflateBlocks.InflateBlockMode.BAD;
                                    this._codec.Message = "invalid block type";
                                    r = -3;
                                    this.bitb = num11;
                                    this.bitk = num12;
                                    this._codec.AvailableBytesIn = num1;
                                    this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                    this._codec.NextIn = sourceIndex;
                                    this.writeAt = destinationIndex;
                                    return this.Flush(r);
                                default:
                                    continue;
                            }
                        case InflateBlocks.InflateBlockMode.LENS:
                            while (num3 < 32)
                            {
                                if (num1 != 0)
                                {
                                    r = 0;
                                    --num1;
                                    num2 |= ((int)this._codec.InputBuffer[sourceIndex++] & (int)byte.MaxValue) << num3;
                                    num3 += 8;
                                }
                                else
                                {
                                    this.bitb = num2;
                                    this.bitk = num3;
                                    this._codec.AvailableBytesIn = num1;
                                    this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                    this._codec.NextIn = sourceIndex;
                                    this.writeAt = destinationIndex;
                                    return this.Flush(r);
                                }
                            }
                            if ((~num2 >> 16 & (int)ushort.MaxValue) != (num2 & (int)ushort.MaxValue))
                            {
                                this.mode = InflateBlocks.InflateBlockMode.BAD;
                                this._codec.Message = "invalid stored block lengths";
                                r = -3;
                                this.bitb = num2;
                                this.bitk = num3;
                                this._codec.AvailableBytesIn = num1;
                                this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                this._codec.NextIn = sourceIndex;
                                this.writeAt = destinationIndex;
                                return this.Flush(r);
                            }
                            this.left = num2 & (int)ushort.MaxValue;
                            num2 = num3 = 0;
                            this.mode = this.left != 0 ? InflateBlocks.InflateBlockMode.STORED : (this.last != 0 ? InflateBlocks.InflateBlockMode.DRY : InflateBlocks.InflateBlockMode.TYPE);
                            continue;
                        case InflateBlocks.InflateBlockMode.STORED:
                            if (num1 == 0)
                            {
                                this.bitb = num2;
                                this.bitk = num3;
                                this._codec.AvailableBytesIn = num1;
                                this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                this._codec.NextIn = sourceIndex;
                                this.writeAt = destinationIndex;
                                return this.Flush(r);
                            }
                            if (num4 == 0)
                            {
                                if (destinationIndex == this.end && this.readAt != 0)
                                {
                                    destinationIndex = 0;
                                    num4 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                                }
                                if (num4 == 0)
                                {
                                    this.writeAt = destinationIndex;
                                    r = this.Flush(r);
                                    destinationIndex = this.writeAt;
                                    num4 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                                    if (destinationIndex == this.end && this.readAt != 0)
                                    {
                                        destinationIndex = 0;
                                        num4 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
                                    }
                                    if (num4 == 0)
                                    {
                                        this.bitb = num2;
                                        this.bitk = num3;
                                        this._codec.AvailableBytesIn = num1;
                                        this._codec.TotalBytesIn += (long)(sourceIndex - this._codec.NextIn);
                                        this._codec.NextIn = sourceIndex;
                                        this.writeAt = destinationIndex;
                                        return this.Flush(r);
                                    }
                                }
                            }
                            r = 0;
                            length1 = this.left;
                            if (length1 > num1)
                                length1 = num1;
                            if (length1 > num4)
                                length1 = num4;
                            Array.Copy((Array)this._codec.InputBuffer, sourceIndex, (Array)this.window, destinationIndex, length1);
                            sourceIndex += length1;
                            num1 -= length1;
                            destinationIndex += length1;
                            num4 -= length1;
                            continue;
                        case InflateBlocks.InflateBlockMode.BTREE:
                            goto label_49;
                        case InflateBlocks.InflateBlockMode.DTREE:
                            goto label_57;
                        case InflateBlocks.InflateBlockMode.CODES:
                            goto label_79;
                        case InflateBlocks.InflateBlockMode.DRY:
                            goto label_84;
                        case InflateBlocks.InflateBlockMode.DONE:
                            goto label_87;
                        case InflateBlocks.InflateBlockMode.BAD:
                            goto label_88;
                        default:
                            goto label_89;
                    }
                }

                while (num3 < 14);
        {
          if (num1 != 0)
          {
            r = 0;
            --num1;
            num2 |= ((int) this._codec.InputBuffer[sourceIndex++] & (int) byte.MaxValue) << num3;
            num3 += 8;
          }
          else
          {
            this.bitb = num2;
            this.bitk = num3;
            this._codec.AvailableBytesIn = num1;
            this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
            this._codec.NextIn = sourceIndex;
            this.writeAt = destinationIndex;
            return this.Flush(r);
          }
        }
        int num13;
        this.table = num13 = num2 & 16383;
        if ((num13 & 31) <= 29 && (num13 >> 5 & 31) <= 29)
        {
          int length2 = 258 + (num13 & 31) + (num13 >> 5 & 31);
          if (this.blens == null || this.blens.Length < length2)
            this.blens = new int[length2];
          else
            Array.Clear((Array) this.blens, 0, length2);
          num2 >>= 14;
          num3 -= 14;
          this.index = 0;
          this.mode = InflateBlocks.InflateBlockMode.BTREE;
        }
        else
          break;
label_49:
        while (this.index < 4 + (this.table >> 10))
        {
          while (num3 < 3)
          {
            if (num1 != 0)
            {
              r = 0;
              --num1;
              num2 |= ((int) this._codec.InputBuffer[sourceIndex++] & (int) byte.MaxValue) << num3;
              num3 += 8;
            }
            else
            {
              this.bitb = num2;
              this.bitk = num3;
              this._codec.AvailableBytesIn = num1;
              this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
              this._codec.NextIn = sourceIndex;
              this.writeAt = destinationIndex;
              return this.Flush(r);
            }
          }
          this.blens[InflateBlocks.border[this.index++]] = num2 & 7;
          num2 >>= 3;
          num3 -= 3;
        }
        while (this.index < 19)
          this.blens[InflateBlocks.border[this.index++]] = 0;
        this.bb[0] = 7;
        num5 = this.inftree.inflate_trees_bits(this.blens, this.bb, this.tb, this.hufts, this._codec);
        if (num5 == 0)
        {
          this.index = 0;
          this.mode = InflateBlocks.InflateBlockMode.DTREE;
        }
        else
          goto label_53;
label_57:
        while (true)
        {
          int num14 = this.table;
          if (this.index < 258 + (num14 & 31) + (num14 >> 5 & 31))
          {
            int index1 = this.bb[0];
            while (num3 < index1)
            {
              if (num1 != 0)
              {
                r = 0;
                --num1;
                num2 |= ((int) this._codec.InputBuffer[sourceIndex++] & (int) byte.MaxValue) << num3;
                num3 += 8;
              }
              else
              {
                this.bitb = num2;
                this.bitk = num3;
                this._codec.AvailableBytesIn = num1;
                this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
                this._codec.NextIn = sourceIndex;
                this.writeAt = destinationIndex;
                return this.Flush(r);
              }
            }
            int index2 = this.hufts[(this.tb[0] + (num2 & InternalInflateConstants.InflateMask[index1])) * 3 + 1];
            int num15 = this.hufts[(this.tb[0] + (num2 & InternalInflateConstants.InflateMask[index2])) * 3 + 2];
            if (num15 < 16)
            {
              num2 >>= index2;
              num3 -= index2;
              this.blens[this.index++] = num15;
            }
            else
            {
              int index3 = num15 == 18 ? 7 : num15 - 14;
              int num16 = num15 == 18 ? 11 : 3;
              while (num3 < index2 + index3)
              {
                if (num1 != 0)
                {
                  r = 0;
                  --num1;
                  num2 |= ((int) this._codec.InputBuffer[sourceIndex++] & (int) byte.MaxValue) << num3;
                  num3 += 8;
                }
                else
                {
                  this.bitb = num2;
                  this.bitk = num3;
                  this._codec.AvailableBytesIn = num1;
                  this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
                  this._codec.NextIn = sourceIndex;
                  this.writeAt = destinationIndex;
                  return this.Flush(r);
                }
              }
              int num17 = num2 >> index2;
              int num18 = num3 - index2;
              int num19 = num16 + (num17 & InternalInflateConstants.InflateMask[index3]);
              num2 = num17 >> index3;
              num3 = num18 - index3;
              int num20 = this.index;
              int num21 = this.table;
              if (num20 + num19 <= 258 + (num21 & 31) + (num21 >> 5 & 31) && (num15 != 16 || num20 >= 1))
              {
                int num22 = num15 == 16 ? this.blens[num20 - 1] : 0;
                do
                {
                  this.blens[num20++] = num22;
                }
                while (--num19 != 0);
                this.index = num20;
              }
              else
                goto label_71;
            }
          }
          else
            break;
        }
        this.tb[0] = -1;
        int[] bl2 = new int[1]
        {
          9
        };
        int[] bd2 = new int[1]
        {
          6
        };
        int[] tl2 = new int[1];
        int[] td2 = new int[1];
        int num23 = this.table;
        num6 = this.inftree.inflate_trees_dynamic(257 + (num23 & 31), 1 + (num23 >> 5 & 31), this.blens, bl2, bd2, tl2, td2, this.hufts, this._codec);
        switch (num6)
        {
          case 0:
            this.codes.Init(bl2[0], bd2[0], this.hufts, tl2[0], this.hufts, td2[0]);
            this.mode = InflateBlocks.InflateBlockMode.CODES;
            break;
          case -3:
            goto label_76;
          default:
            goto label_77;
        }
label_79:
        this.bitb = num2;
        this.bitk = num3;
        this._codec.AvailableBytesIn = num1;
        this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
        this._codec.NextIn = sourceIndex;
        this.writeAt = destinationIndex;
        r = this.codes.Process(this, r);
        if (r == 1)
        {
          r = 0;
          sourceIndex = this._codec.NextIn;
          num1 = this._codec.AvailableBytesIn;
          num2 = this.bitb;
          num3 = this.bitk;
          destinationIndex = this.writeAt;
          num4 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
          if (this.last == 0)
            this.mode = InflateBlocks.InflateBlockMode.TYPE;
          else
            goto label_83;
        }
        else
          goto label_80;
      }
      this.mode = InflateBlocks.InflateBlockMode.BAD;
      this._codec.Message = "too many length or distance symbols";
      r = -3;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_53:
      r = num5;
      if (r == -3)
      {
        this.blens = (int[]) null;
        this.mode = InflateBlocks.InflateBlockMode.BAD;
      }
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_71:
      this.blens = (int[]) null;
      this.mode = InflateBlocks.InflateBlockMode.BAD;
      this._codec.Message = "invalid bit length repeat";
      r = -3;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_76:
      this.blens = (int[]) null;
      this.mode = InflateBlocks.InflateBlockMode.BAD;
label_77:
      r = num6;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_80:
      return this.Flush(r);
label_83:
      this.mode = InflateBlocks.InflateBlockMode.DRY;
label_84:
      this.writeAt = destinationIndex;
      r = this.Flush(r);
      destinationIndex = this.writeAt;
      int num24 = destinationIndex < this.readAt ? this.readAt - destinationIndex - 1 : this.end - destinationIndex;
      if (this.readAt != this.writeAt)
      {
        this.bitb = num2;
        this.bitk = num3;
        this._codec.AvailableBytesIn = num1;
        this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
        this._codec.NextIn = sourceIndex;
        this.writeAt = destinationIndex;
        return this.Flush(r);
      }
      this.mode = InflateBlocks.InflateBlockMode.DONE;
label_87:
      r = 1;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_88:
      r = -3;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
label_89:
      r = -2;
      this.bitb = num2;
      this.bitk = num3;
      this._codec.AvailableBytesIn = num1;
      this._codec.TotalBytesIn += (long) (sourceIndex - this._codec.NextIn);
      this._codec.NextIn = sourceIndex;
      this.writeAt = destinationIndex;
      return this.Flush(r);
    }

    internal void Free()
    {
      int num = (int) this.Reset();
      this.window = (byte[]) null;
      this.hufts = (int[]) null;
    }

    internal void SetDictionary(byte[] d, int start, int n)
    {
      Array.Copy((Array) d, start, (Array) this.window, 0, n);
      this.readAt = this.writeAt = n;
    }

    internal int SyncPoint()
    {
      return this.mode != InflateBlocks.InflateBlockMode.LENS ? 0 : 1;
    }

    internal int Flush(int r)
    {
      for (int index = 0; index < 2; ++index)
      {
        int num = index != 0 ? this.writeAt - this.readAt : (this.readAt <= this.writeAt ? this.writeAt : this.end) - this.readAt;
        if (num == 0)
        {
          if (r == -5)
            r = 0;
          return r;
        }
        if (num > this._codec.AvailableBytesOut)
          num = this._codec.AvailableBytesOut;
        if (num != 0 && r == -5)
          r = 0;
        this._codec.AvailableBytesOut -= num;
        this._codec.TotalBytesOut += (long) num;
        if (this.checkfn != null)
          this._codec._Adler32 = this.check = Adler.Adler32(this.check, this.window, this.readAt, num);
        Array.Copy((Array) this.window, this.readAt, (Array) this._codec.OutputBuffer, this._codec.NextOut, num);
        this._codec.NextOut += num;
        this.readAt += num;
        if (this.readAt == this.end && index == 0)
        {
          this.readAt = 0;
          if (this.writeAt == this.end)
            this.writeAt = 0;
        }
        else
          ++index;
      }
      return r;
    }

    private enum InflateBlockMode
    {
      TYPE,
      LENS,
      STORED,
      TABLE,
      BTREE,
      DTREE,
      CODES,
      DRY,
      DONE,
      BAD,
    }
  }
}
