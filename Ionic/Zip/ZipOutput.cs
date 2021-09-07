
// Type: Ionic.Zip.ZipOutput


// Hacked by SystemAce

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ionic.Zip
{
  internal static class ZipOutput
  {
    public static bool WriteCentralDirectoryStructure(Stream s, ICollection<ZipEntry> entries, uint numSegments, Zip64Option zip64, string comment, ZipContainer container)
    {
      ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
      if (zipSegmentedStream != null)
        zipSegmentedStream.ContiguousWrite = true;
      long num1 = 0L;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        foreach (ZipEntry zipEntry in (IEnumerable<ZipEntry>) entries)
        {
          if (zipEntry.IncludedInMostRecentSave)
            zipEntry.WriteCentralDirectoryEntry((Stream) memoryStream);
        }
        byte[] buffer = memoryStream.ToArray();
        s.Write(buffer, 0, buffer.Length);
        num1 = (long) buffer.Length;
      }
      CountingStream countingStream = s as CountingStream;
      long EndOfCentralDirectory = countingStream != null ? countingStream.ComputedPosition : s.Position;
      long StartOfCentralDirectory = EndOfCentralDirectory - num1;
      uint num2 = zipSegmentedStream != null ? zipSegmentedStream.CurrentSegment : 0U;
      long num3 = EndOfCentralDirectory - StartOfCentralDirectory;
      int entryCount = ZipOutput.CountEntries(entries);
      bool flag = zip64 == Zip64Option.Always || entryCount >= (int) ushort.MaxValue || num3 > (long) uint.MaxValue || StartOfCentralDirectory > (long) uint.MaxValue;
      byte[] buffer1;
      if (flag)
      {
        if (zip64 == Zip64Option.Default)
        {
          if (new StackFrame(1).GetMethod().DeclaringType == typeof (ZipFile))
            throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipFile.UseZip64WhenSaving property.");
          throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipOutputStream.EnableZip64 property.");
        }
        byte[] buffer2 = ZipOutput.GenZip64EndOfCentralDirectory(StartOfCentralDirectory, EndOfCentralDirectory, entryCount, numSegments);
        buffer1 = ZipOutput.GenCentralDirectoryFooter(StartOfCentralDirectory, EndOfCentralDirectory, zip64, entryCount, comment, container);
        if ((int) num2 != 0)
        {
          uint segment = zipSegmentedStream.ComputeSegment(buffer2.Length + buffer1.Length);
          int destinationIndex1 = 16;
          Array.Copy((Array) BitConverter.GetBytes(segment), 0, (Array) buffer2, destinationIndex1, 4);
          int destinationIndex2 = destinationIndex1 + 4;
          Array.Copy((Array) BitConverter.GetBytes(segment), 0, (Array) buffer2, destinationIndex2, 4);
          int destinationIndex3 = 60;
          Array.Copy((Array) BitConverter.GetBytes(segment), 0, (Array) buffer2, destinationIndex3, 4);
          int destinationIndex4 = destinationIndex3 + 4 + 8;
          Array.Copy((Array) BitConverter.GetBytes(segment), 0, (Array) buffer2, destinationIndex4, 4);
        }
        s.Write(buffer2, 0, buffer2.Length);
      }
      else
        buffer1 = ZipOutput.GenCentralDirectoryFooter(StartOfCentralDirectory, EndOfCentralDirectory, zip64, entryCount, comment, container);
      if ((int) num2 != 0)
      {
        ushort num4 = (ushort) zipSegmentedStream.ComputeSegment(buffer1.Length);
        int destinationIndex1 = 4;
        Array.Copy((Array) BitConverter.GetBytes(num4), 0, (Array) buffer1, destinationIndex1, 2);
        int destinationIndex2 = destinationIndex1 + 2;
        Array.Copy((Array) BitConverter.GetBytes(num4), 0, (Array) buffer1, destinationIndex2, 2);
        int num5 = destinationIndex2 + 2;
      }
      s.Write(buffer1, 0, buffer1.Length);
      if (zipSegmentedStream != null)
        zipSegmentedStream.ContiguousWrite = false;
      return flag;
    }

    private static Encoding GetEncoding(ZipContainer container, string t)
    {
      switch (container.AlternateEncodingUsage)
      {
        case ZipOption.Default:
          return container.DefaultEncoding;
        case ZipOption.Always:
          return container.AlternateEncoding;
        default:
          Encoding defaultEncoding = container.DefaultEncoding;
          if (t == null)
            return defaultEncoding;
          byte[] bytes = defaultEncoding.GetBytes(t);
          if (defaultEncoding.GetString(bytes, 0, bytes.Length).Equals(t))
            return defaultEncoding;
          return container.AlternateEncoding;
      }
    }

    private static byte[] GenCentralDirectoryFooter(long StartOfCentralDirectory, long EndOfCentralDirectory, Zip64Option zip64, int entryCount, string comment, ZipContainer container)
    {
      Encoding encoding = ZipOutput.GetEncoding(container, comment);
      int num1 = 22;
      byte[] numArray1 = (byte[]) null;
      short num2 = (short) 0;
      if (comment != null && comment.Length != 0)
      {
        numArray1 = encoding.GetBytes(comment);
        num2 = (short) numArray1.Length;
      }
      byte[] numArray2 = new byte[num1 + (int) num2];
      int destinationIndex = 0;
      Array.Copy((Array) BitConverter.GetBytes(101010256U), 0, (Array) numArray2, destinationIndex, 4);
      int num3 = destinationIndex + 4;
      byte[] numArray3 = numArray2;
      int index1 = num3;
      int num4 = 1;
      int num5 = index1 + num4;
      int num6 = 0;
      numArray3[index1] = (byte) num6;
      byte[] numArray4 = numArray2;
      int index2 = num5;
      int num7 = 1;
      int num8 = index2 + num7;
      int num9 = 0;
      numArray4[index2] = (byte) num9;
      byte[] numArray5 = numArray2;
      int index3 = num8;
      int num10 = 1;
      int num11 = index3 + num10;
      int num12 = 0;
      numArray5[index3] = (byte) num12;
      byte[] numArray6 = numArray2;
      int index4 = num11;
      int num13 = 1;
      int num14 = index4 + num13;
      int num15 = 0;
      numArray6[index4] = (byte) num15;
      if (entryCount >= (int) ushort.MaxValue || zip64 == Zip64Option.Always)
      {
        for (int index5 = 0; index5 < 4; ++index5)
          numArray2[num14++] = byte.MaxValue;
      }
      else
      {
        byte[] numArray7 = numArray2;
        int index5 = num14;
        int num16 = 1;
        int num17 = index5 + num16;
        int num18 = (int) (byte) (entryCount & (int) byte.MaxValue);
        numArray7[index5] = (byte) num18;
        byte[] numArray8 = numArray2;
        int index6 = num17;
        int num19 = 1;
        int num20 = index6 + num19;
        int num21 = (int) (byte) ((entryCount & 65280) >> 8);
        numArray8[index6] = (byte) num21;
        byte[] numArray9 = numArray2;
        int index7 = num20;
        int num22 = 1;
        int num23 = index7 + num22;
        int num24 = (int) (byte) (entryCount & (int) byte.MaxValue);
        numArray9[index7] = (byte) num24;
        byte[] numArray10 = numArray2;
        int index8 = num23;
        int num25 = 1;
        num14 = index8 + num25;
        int num26 = (int) (byte) ((entryCount & 65280) >> 8);
        numArray10[index8] = (byte) num26;
      }
      long num27 = EndOfCentralDirectory - StartOfCentralDirectory;
      if (num27 >= (long) uint.MaxValue || StartOfCentralDirectory >= (long) uint.MaxValue)
      {
        for (int index5 = 0; index5 < 8; ++index5)
          numArray2[num14++] = byte.MaxValue;
      }
      else
      {
        byte[] numArray7 = numArray2;
        int index5 = num14;
        int num16 = 1;
        int num17 = index5 + num16;
        int num18 = (int) (byte) ((ulong) num27 & (ulong) byte.MaxValue);
        numArray7[index5] = (byte) num18;
        byte[] numArray8 = numArray2;
        int index6 = num17;
        int num19 = 1;
        int num20 = index6 + num19;
        int num21 = (int) (byte) ((num27 & 65280L) >> 8);
        numArray8[index6] = (byte) num21;
        byte[] numArray9 = numArray2;
        int index7 = num20;
        int num22 = 1;
        int num23 = index7 + num22;
        int num24 = (int) (byte) ((num27 & 16711680L) >> 16);
        numArray9[index7] = (byte) num24;
        byte[] numArray10 = numArray2;
        int index8 = num23;
        int num25 = 1;
        int num26 = index8 + num25;
        int num28 = (int) (byte) ((num27 & 4278190080L) >> 24);
        numArray10[index8] = (byte) num28;
        byte[] numArray11 = numArray2;
        int index9 = num26;
        int num29 = 1;
        int num30 = index9 + num29;
        int num31 = (int) (byte) ((ulong) StartOfCentralDirectory & (ulong) byte.MaxValue);
        numArray11[index9] = (byte) num31;
        byte[] numArray12 = numArray2;
        int index10 = num30;
        int num32 = 1;
        int num33 = index10 + num32;
        int num34 = (int) (byte) ((StartOfCentralDirectory & 65280L) >> 8);
        numArray12[index10] = (byte) num34;
        byte[] numArray13 = numArray2;
        int index11 = num33;
        int num35 = 1;
        int num36 = index11 + num35;
        int num37 = (int) (byte) ((StartOfCentralDirectory & 16711680L) >> 16);
        numArray13[index11] = (byte) num37;
        byte[] numArray14 = numArray2;
        int index12 = num36;
        int num38 = 1;
        num14 = index12 + num38;
        int num39 = (int) (byte) ((StartOfCentralDirectory & 4278190080L) >> 24);
        numArray14[index12] = (byte) num39;
      }
      int num40;
      if (comment == null || comment.Length == 0)
      {
        byte[] numArray7 = numArray2;
        int index5 = num14;
        int num16 = 1;
        int num17 = index5 + num16;
        int num18 = 0;
        numArray7[index5] = (byte) num18;
        byte[] numArray8 = numArray2;
        int index6 = num17;
        int num19 = 1;
        num40 = index6 + num19;
        int num20 = 0;
        numArray8[index6] = (byte) num20;
      }
      else
      {
        if ((int) num2 + num14 + 2 > numArray2.Length)
          num2 = (short) (numArray2.Length - num14 - 2);
        byte[] numArray7 = numArray2;
        int index5 = num14;
        int num16 = 1;
        int num17 = index5 + num16;
        int num18 = (int) (byte) ((uint) num2 & (uint) byte.MaxValue);
        numArray7[index5] = (byte) num18;
        byte[] numArray8 = numArray2;
        int index6 = num17;
        int num19 = 1;
        int num20 = index6 + num19;
        int num21 = (int) (byte) (((int) num2 & 65280) >> 8);
        numArray8[index6] = (byte) num21;
        if ((int) num2 != 0)
        {
          int index7;
          for (index7 = 0; index7 < (int) num2 && num20 + index7 < numArray2.Length; ++index7)
            numArray2[num20 + index7] = numArray1[index7];
          num40 = num20 + index7;
        }
      }
      return numArray2;
    }

    private static byte[] GenZip64EndOfCentralDirectory(long StartOfCentralDirectory, long EndOfCentralDirectory, int entryCount, uint numSegments)
    {
      byte[] numArray1 = new byte[76];
      int destinationIndex1 = 0;
      Array.Copy((Array) BitConverter.GetBytes(101075792U), 0, (Array) numArray1, destinationIndex1, 4);
      int destinationIndex2 = destinationIndex1 + 4;
      Array.Copy((Array) BitConverter.GetBytes(44L), 0, (Array) numArray1, destinationIndex2, 8);
      int num1 = destinationIndex2 + 8;
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2;
      int num4 = 45;
      numArray2[index1] = (byte) num4;
      byte[] numArray3 = numArray1;
      int index2 = num3;
      int num5 = 1;
      int num6 = index2 + num5;
      int num7 = 0;
      numArray3[index2] = (byte) num7;
      byte[] numArray4 = numArray1;
      int index3 = num6;
      int num8 = 1;
      int num9 = index3 + num8;
      int num10 = 45;
      numArray4[index3] = (byte) num10;
      byte[] numArray5 = numArray1;
      int index4 = num9;
      int num11 = 1;
      int destinationIndex3 = index4 + num11;
      int num12 = 0;
      numArray5[index4] = (byte) num12;
      for (int index5 = 0; index5 < 8; ++index5)
        numArray1[destinationIndex3++] = (byte) 0;
      long num13 = (long) entryCount;
      Array.Copy((Array) BitConverter.GetBytes(num13), 0, (Array) numArray1, destinationIndex3, 8);
      int destinationIndex4 = destinationIndex3 + 8;
      Array.Copy((Array) BitConverter.GetBytes(num13), 0, (Array) numArray1, destinationIndex4, 8);
      int destinationIndex5 = destinationIndex4 + 8;
      Array.Copy((Array) BitConverter.GetBytes(EndOfCentralDirectory - StartOfCentralDirectory), 0, (Array) numArray1, destinationIndex5, 8);
      int destinationIndex6 = destinationIndex5 + 8;
      Array.Copy((Array) BitConverter.GetBytes(StartOfCentralDirectory), 0, (Array) numArray1, destinationIndex6, 8);
      int destinationIndex7 = destinationIndex6 + 8;
      Array.Copy((Array) BitConverter.GetBytes(117853008U), 0, (Array) numArray1, destinationIndex7, 4);
      int destinationIndex8 = destinationIndex7 + 4;
      Array.Copy((Array) BitConverter.GetBytes((int) numSegments == 0 ? 0U : numSegments - 1U), 0, (Array) numArray1, destinationIndex8, 4);
      int destinationIndex9 = destinationIndex8 + 4;
      Array.Copy((Array) BitConverter.GetBytes(EndOfCentralDirectory), 0, (Array) numArray1, destinationIndex9, 8);
      int destinationIndex10 = destinationIndex9 + 8;
      Array.Copy((Array) BitConverter.GetBytes(numSegments), 0, (Array) numArray1, destinationIndex10, 4);
      int num14 = destinationIndex10 + 4;
      return numArray1;
    }

    private static int CountEntries(ICollection<ZipEntry> _entries)
    {
      int num = 0;
      foreach (ZipEntry zipEntry in (IEnumerable<ZipEntry>) _entries)
      {
        if (zipEntry.IncludedInMostRecentSave)
          ++num;
      }
      return num;
    }
  }
}
