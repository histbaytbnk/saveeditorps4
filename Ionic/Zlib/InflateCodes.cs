
// Type: Ionic.Zlib.InflateCodes


// Hacked by SystemAce

using System;

namespace Ionic.Zlib
{
  internal sealed class InflateCodes
  {
    private const int START = 0;
    private const int LEN = 1;
    private const int LENEXT = 2;
    private const int DIST = 3;
    private const int DISTEXT = 4;
    private const int COPY = 5;
    private const int LIT = 6;
    private const int WASH = 7;
    private const int END = 8;
    private const int BADCODE = 9;
    internal int mode;
    internal int len;
    internal int[] tree;
    internal int tree_index;
    internal int need;
    internal int lit;
    internal int bitsToGet;
    internal int dist;
    internal byte lbits;
    internal byte dbits;
    internal int[] ltree;
    internal int ltree_index;
    internal int[] dtree;
    internal int dtree_index;

    internal InflateCodes()
    {
    }

    internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
    {
      this.mode = 0;
      this.lbits = (byte) bl;
      this.dbits = (byte) bd;
      this.ltree = tl;
      this.ltree_index = tl_index;
      this.dtree = td;
      this.dtree_index = td_index;
      this.tree = (int[]) null;
    }

    internal int Process(InflateBlocks blocks, int r)
    {
      ZlibCodec z = blocks._codec;
      int num1 = z.NextIn;
      int num2 = z.AvailableBytesIn;
      int num3 = blocks.bitb;
      int num4 = blocks.bitk;
      int num5 = blocks.writeAt;
      int num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
      while (true)
      {
        switch (this.mode)
        {
          case 0:
            if (num6 >= 258 && num2 >= 10)
            {
              blocks.bitb = num3;
              blocks.bitk = num4;
              z.AvailableBytesIn = num2;
              z.TotalBytesIn += (long) (num1 - z.NextIn);
              z.NextIn = num1;
              blocks.writeAt = num5;
              r = this.InflateFast((int) this.lbits, (int) this.dbits, this.ltree, this.ltree_index, this.dtree, this.dtree_index, blocks, z);
              num1 = z.NextIn;
              num2 = z.AvailableBytesIn;
              num3 = blocks.bitb;
              num4 = blocks.bitk;
              num5 = blocks.writeAt;
              num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
              if (r != 0)
              {
                this.mode = r == 1 ? 7 : 9;
                continue;
              }
            }
            this.need = (int) this.lbits;
            this.tree = this.ltree;
            this.tree_index = this.ltree_index;
            this.mode = 1;
            goto case 1;
          case 1:
            int index1 = this.need;
            while (num4 < index1)
            {
              if (num2 != 0)
              {
                r = 0;
                --num2;
                num3 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num4;
                num4 += 8;
              }
              else
              {
                blocks.bitb = num3;
                blocks.bitk = num4;
                z.AvailableBytesIn = num2;
                z.TotalBytesIn += (long) (num1 - z.NextIn);
                z.NextIn = num1;
                blocks.writeAt = num5;
                return blocks.Flush(r);
              }
            }
            int index2 = (this.tree_index + (num3 & InternalInflateConstants.InflateMask[index1])) * 3;
            num3 >>= this.tree[index2 + 1];
            num4 -= this.tree[index2 + 1];
            int num7 = this.tree[index2];
            if (num7 == 0)
            {
              this.lit = this.tree[index2 + 2];
              this.mode = 6;
              continue;
            }
            if ((num7 & 16) != 0)
            {
              this.bitsToGet = num7 & 15;
              this.len = this.tree[index2 + 2];
              this.mode = 2;
              continue;
            }
            if ((num7 & 64) == 0)
            {
              this.need = num7;
              this.tree_index = index2 / 3 + this.tree[index2 + 2];
              continue;
            }
            if ((num7 & 32) != 0)
            {
              this.mode = 7;
              continue;
            }
            goto label_19;
          case 2:
            int index3 = this.bitsToGet;
            while (num4 < index3)
            {
              if (num2 != 0)
              {
                r = 0;
                --num2;
                num3 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num4;
                num4 += 8;
              }
              else
              {
                blocks.bitb = num3;
                blocks.bitk = num4;
                z.AvailableBytesIn = num2;
                z.TotalBytesIn += (long) (num1 - z.NextIn);
                z.NextIn = num1;
                blocks.writeAt = num5;
                return blocks.Flush(r);
              }
            }
            this.len += num3 & InternalInflateConstants.InflateMask[index3];
            num3 >>= index3;
            num4 -= index3;
            this.need = (int) this.dbits;
            this.tree = this.dtree;
            this.tree_index = this.dtree_index;
            this.mode = 3;
            goto case 3;
          case 3:
            int index4 = this.need;
            while (num4 < index4)
            {
              if (num2 != 0)
              {
                r = 0;
                --num2;
                num3 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num4;
                num4 += 8;
              }
              else
              {
                blocks.bitb = num3;
                blocks.bitk = num4;
                z.AvailableBytesIn = num2;
                z.TotalBytesIn += (long) (num1 - z.NextIn);
                z.NextIn = num1;
                blocks.writeAt = num5;
                return blocks.Flush(r);
              }
            }
            int index5 = (this.tree_index + (num3 & InternalInflateConstants.InflateMask[index4])) * 3;
            num3 >>= this.tree[index5 + 1];
            num4 -= this.tree[index5 + 1];
            int num8 = this.tree[index5];
            if ((num8 & 16) != 0)
            {
              this.bitsToGet = num8 & 15;
              this.dist = this.tree[index5 + 2];
              this.mode = 4;
              continue;
            }
            if ((num8 & 64) == 0)
            {
              this.need = num8;
              this.tree_index = index5 / 3 + this.tree[index5 + 2];
              continue;
            }
            goto label_35;
          case 4:
            int index6 = this.bitsToGet;
            while (num4 < index6)
            {
              if (num2 != 0)
              {
                r = 0;
                --num2;
                num3 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num4;
                num4 += 8;
              }
              else
              {
                blocks.bitb = num3;
                blocks.bitk = num4;
                z.AvailableBytesIn = num2;
                z.TotalBytesIn += (long) (num1 - z.NextIn);
                z.NextIn = num1;
                blocks.writeAt = num5;
                return blocks.Flush(r);
              }
            }
            this.dist += num3 & InternalInflateConstants.InflateMask[index6];
            num3 >>= index6;
            num4 -= index6;
            this.mode = 5;
            goto case 5;
          case 5:
            int num9 = num5 - this.dist;
            while (num9 < 0)
              num9 += blocks.end;
            for (; this.len != 0; --this.len)
            {
              if (num6 == 0)
              {
                if (num5 == blocks.end && blocks.readAt != 0)
                {
                  num5 = 0;
                  num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
                }
                if (num6 == 0)
                {
                  blocks.writeAt = num5;
                  r = blocks.Flush(r);
                  num5 = blocks.writeAt;
                  num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
                  if (num5 == blocks.end && blocks.readAt != 0)
                  {
                    num5 = 0;
                    num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
                  }
                  if (num6 == 0)
                  {
                    blocks.bitb = num3;
                    blocks.bitk = num4;
                    z.AvailableBytesIn = num2;
                    z.TotalBytesIn += (long) (num1 - z.NextIn);
                    z.NextIn = num1;
                    blocks.writeAt = num5;
                    return blocks.Flush(r);
                  }
                }
              }
              blocks.window[num5++] = blocks.window[num9++];
              --num6;
              if (num9 == blocks.end)
                num9 = 0;
            }
            this.mode = 0;
            continue;
          case 6:
            if (num6 == 0)
            {
              if (num5 == blocks.end && blocks.readAt != 0)
              {
                num5 = 0;
                num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
              }
              if (num6 == 0)
              {
                blocks.writeAt = num5;
                r = blocks.Flush(r);
                num5 = blocks.writeAt;
                num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
                if (num5 == blocks.end && blocks.readAt != 0)
                {
                  num5 = 0;
                  num6 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
                }
                if (num6 == 0)
                  goto label_65;
              }
            }
            r = 0;
            blocks.window[num5++] = (byte) this.lit;
            --num6;
            this.mode = 0;
            continue;
          case 7:
            goto label_67;
          case 8:
            goto label_72;
          case 9:
            goto label_73;
          default:
            goto label_74;
        }
      }
label_19:
      this.mode = 9;
      z.Message = "invalid literal/length code";
      r = -3;
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
label_35:
      this.mode = 9;
      z.Message = "invalid distance code";
      r = -3;
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
label_65:
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
label_67:
      if (num4 > 7)
      {
        num4 -= 8;
        ++num2;
        --num1;
      }
      blocks.writeAt = num5;
      r = blocks.Flush(r);
      num5 = blocks.writeAt;
      int num10 = num5 < blocks.readAt ? blocks.readAt - num5 - 1 : blocks.end - num5;
      if (blocks.readAt != blocks.writeAt)
      {
        blocks.bitb = num3;
        blocks.bitk = num4;
        z.AvailableBytesIn = num2;
        z.TotalBytesIn += (long) (num1 - z.NextIn);
        z.NextIn = num1;
        blocks.writeAt = num5;
        return blocks.Flush(r);
      }
      this.mode = 8;
label_72:
      r = 1;
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
label_73:
      r = -3;
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
label_74:
      r = -2;
      blocks.bitb = num3;
      blocks.bitk = num4;
      z.AvailableBytesIn = num2;
      z.TotalBytesIn += (long) (num1 - z.NextIn);
      z.NextIn = num1;
      blocks.writeAt = num5;
      return blocks.Flush(r);
    }

    internal int InflateFast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InflateBlocks s, ZlibCodec z)
    {
      int num1 = z.NextIn;
      int num2 = z.AvailableBytesIn;
      int num3 = s.bitb;
      int num4 = s.bitk;
      int destinationIndex = s.writeAt;
      int num5 = destinationIndex < s.readAt ? s.readAt - destinationIndex - 1 : s.end - destinationIndex;
      int num6 = InternalInflateConstants.InflateMask[bl];
      int num7 = InternalInflateConstants.InflateMask[bd];
      do
      {
        while (num4 < 20)
        {
          --num2;
          num3 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num4;
          num4 += 8;
        }
        int num8 = num3 & num6;
        int[] numArray1 = tl;
        int num9 = tl_index;
        int index1 = (num9 + num8) * 3;
        int index2;
        if ((index2 = numArray1[index1]) == 0)
        {
          num3 >>= numArray1[index1 + 1];
          num4 -= numArray1[index1 + 1];
          s.window[destinationIndex++] = (byte) numArray1[index1 + 2];
          --num5;
        }
        else
        {
          do
          {
            num3 >>= numArray1[index1 + 1];
            num4 -= numArray1[index1 + 1];
            if ((index2 & 16) != 0)
            {
              int index3 = index2 & 15;
              int length1 = numArray1[index1 + 2] + (num3 & InternalInflateConstants.InflateMask[index3]);
              int num10 = num3 >> index3;
              int num11 = num4 - index3;
              while (num11 < 15)
              {
                --num2;
                num10 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num11;
                num11 += 8;
              }
              int num12 = num10 & num7;
              int[] numArray2 = td;
              int num13 = td_index;
              int index4 = (num13 + num12) * 3;
              int index5 = numArray2[index4];
              while (true)
              {
                num10 >>= numArray2[index4 + 1];
                num11 -= numArray2[index4 + 1];
                if ((index5 & 16) == 0)
                {
                  if ((index5 & 64) == 0)
                  {
                    num12 = num12 + numArray2[index4 + 2] + (num10 & InternalInflateConstants.InflateMask[index5]);
                    index4 = (num13 + num12) * 3;
                    index5 = numArray2[index4];
                  }
                  else
                    goto label_30;
                }
                else
                  break;
              }
              int index6 = index5 & 15;
              while (num11 < index6)
              {
                --num2;
                num10 |= ((int) z.InputBuffer[num1++] & (int) byte.MaxValue) << num11;
                num11 += 8;
              }
              int num14 = numArray2[index4 + 2] + (num10 & InternalInflateConstants.InflateMask[index6]);
              num3 = num10 >> index6;
              num4 = num11 - index6;
              num5 -= length1;
              int sourceIndex1;
              int num15;
              if (destinationIndex >= num14)
              {
                int sourceIndex2 = destinationIndex - num14;
                if (destinationIndex - sourceIndex2 > 0 && 2 > destinationIndex - sourceIndex2)
                {
                  byte[] numArray3 = s.window;
                  int index7 = destinationIndex;
                  int num16 = 1;
                  int num17 = index7 + num16;
                  byte[] numArray4 = s.window;
                  int index8 = sourceIndex2;
                  int num18 = 1;
                  int num19 = index8 + num18;
                  int num20 = (int) numArray4[index8];
                  numArray3[index7] = (byte) num20;
                  byte[] numArray5 = s.window;
                  int index9 = num17;
                  int num21 = 1;
                  destinationIndex = index9 + num21;
                  byte[] numArray6 = s.window;
                  int index10 = num19;
                  int num22 = 1;
                  sourceIndex1 = index10 + num22;
                  int num23 = (int) numArray6[index10];
                  numArray5[index9] = (byte) num23;
                  length1 -= 2;
                }
                else
                {
                  Array.Copy((Array) s.window, sourceIndex2, (Array) s.window, destinationIndex, 2);
                  destinationIndex += 2;
                  sourceIndex1 = sourceIndex2 + 2;
                  length1 -= 2;
                }
              }
              else
              {
                sourceIndex1 = destinationIndex - num14;
                do
                {
                  sourceIndex1 += s.end;
                }
                while (sourceIndex1 < 0);
                int length2 = s.end - sourceIndex1;
                if (length1 > length2)
                {
                  length1 -= length2;
                  if (destinationIndex - sourceIndex1 > 0 && length2 > destinationIndex - sourceIndex1)
                  {
                    do
                    {
                      s.window[destinationIndex++] = s.window[sourceIndex1++];
                    }
                    while (--length2 != 0);
                  }
                  else
                  {
                    Array.Copy((Array) s.window, sourceIndex1, (Array) s.window, destinationIndex, length2);
                    destinationIndex += length2;
                    num15 = sourceIndex1 + length2;
                  }
                  sourceIndex1 = 0;
                }
              }
              if (destinationIndex - sourceIndex1 > 0 && length1 > destinationIndex - sourceIndex1)
              {
                do
                {
                  s.window[destinationIndex++] = s.window[sourceIndex1++];
                }
                while (--length1 != 0);
                goto label_37;
              }
              else
              {
                Array.Copy((Array) s.window, sourceIndex1, (Array) s.window, destinationIndex, length1);
                destinationIndex += length1;
                num15 = sourceIndex1 + length1;
                goto label_37;
              }
label_30:
              z.Message = "invalid distance code";
              int num24 = z.AvailableBytesIn - num2;
              int num25 = num11 >> 3 < num24 ? num11 >> 3 : num24;
              int num26 = num2 + num25;
              int num27 = num1 - num25;
              int num28 = num11 - (num25 << 3);
              s.bitb = num10;
              s.bitk = num28;
              z.AvailableBytesIn = num26;
              z.TotalBytesIn += (long) (num27 - z.NextIn);
              z.NextIn = num27;
              s.writeAt = destinationIndex;
              return -3;
            }
            if ((index2 & 64) == 0)
            {
              num8 = num8 + numArray1[index1 + 2] + (num3 & InternalInflateConstants.InflateMask[index2]);
              index1 = (num9 + num8) * 3;
            }
            else
              goto label_34;
          }
          while ((index2 = numArray1[index1]) != 0);
          num3 >>= numArray1[index1 + 1];
          num4 -= numArray1[index1 + 1];
          s.window[destinationIndex++] = (byte) numArray1[index1 + 2];
          --num5;
          goto label_37;
label_34:
          if ((index2 & 32) != 0)
          {
            int num10 = z.AvailableBytesIn - num2;
            int num11 = num4 >> 3 < num10 ? num4 >> 3 : num10;
            int num12 = num2 + num11;
            int num13 = num1 - num11;
            int num14 = num4 - (num11 << 3);
            s.bitb = num3;
            s.bitk = num14;
            z.AvailableBytesIn = num12;
            z.TotalBytesIn += (long) (num13 - z.NextIn);
            z.NextIn = num13;
            s.writeAt = destinationIndex;
            return 1;
          }
          z.Message = "invalid literal/length code";
          int num29 = z.AvailableBytesIn - num2;
          int num30 = num4 >> 3 < num29 ? num4 >> 3 : num29;
          int num31 = num2 + num30;
          int num32 = num1 - num30;
          int num33 = num4 - (num30 << 3);
          s.bitb = num3;
          s.bitk = num33;
          z.AvailableBytesIn = num31;
          z.TotalBytesIn += (long) (num32 - z.NextIn);
          z.NextIn = num32;
          s.writeAt = destinationIndex;
          return -3;
        }
label_37:;
      }
      while (num5 >= 258 && num2 >= 10);
      int num34 = z.AvailableBytesIn - num2;
      int num35 = num4 >> 3 < num34 ? num4 >> 3 : num34;
      int num36 = num2 + num35;
      int num37 = num1 - num35;
      int num38 = num4 - (num35 << 3);
      s.bitb = num3;
      s.bitk = num38;
      z.AvailableBytesIn = num36;
      z.TotalBytesIn += (long) (num37 - z.NextIn);
      z.NextIn = num37;
      s.writeAt = destinationIndex;
      return 0;
    }
  }
}
