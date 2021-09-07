
// Type: ICSharpCode.SharpZipLib.Zip.ExtendedUnixData


// Hacked by SystemAce

using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class ExtendedUnixData : ITaggedData
  {
    private DateTime _modificationTime = new DateTime(1970, 1, 1);
    private DateTime _lastAccessTime = new DateTime(1970, 1, 1);
    private DateTime _createTime = new DateTime(1970, 1, 1);
    private ExtendedUnixData.Flags _flags;

    public short TagID
    {
      get
      {
        return (short) 21589;
      }
    }

    public DateTime ModificationTime
    {
      get
      {
        return this._modificationTime;
      }
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._flags |= ExtendedUnixData.Flags.ModificationTime;
        this._modificationTime = value;
      }
    }

    public DateTime AccessTime
    {
      get
      {
        return this._lastAccessTime;
      }
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._flags |= ExtendedUnixData.Flags.AccessTime;
        this._lastAccessTime = value;
      }
    }

    public DateTime CreateTime
    {
      get
      {
        return this._createTime;
      }
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._flags |= ExtendedUnixData.Flags.CreateTime;
        this._createTime = value;
      }
    }

    private ExtendedUnixData.Flags Include
    {
      get
      {
        return this._flags;
      }
      set
      {
        this._flags = value;
      }
    }

    public void SetData(byte[] data, int index, int count)
    {
      using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
      {
        using (ZipHelperStream zipHelperStream = new ZipHelperStream((Stream) memoryStream))
        {
          this._flags = (ExtendedUnixData.Flags) zipHelperStream.ReadByte();
          if ((this._flags & ExtendedUnixData.Flags.ModificationTime) != (ExtendedUnixData.Flags) 0 && count >= 5)
            this._modificationTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, zipHelperStream.ReadLEInt(), 0)).ToLocalTime();
          if ((this._flags & ExtendedUnixData.Flags.AccessTime) != (ExtendedUnixData.Flags) 0)
            this._lastAccessTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, zipHelperStream.ReadLEInt(), 0)).ToLocalTime();
          if ((this._flags & ExtendedUnixData.Flags.CreateTime) == (ExtendedUnixData.Flags) 0)
            return;
          this._createTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, zipHelperStream.ReadLEInt(), 0)).ToLocalTime();
        }
      }
    }

    public byte[] GetData()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (ZipHelperStream zipHelperStream = new ZipHelperStream((Stream) memoryStream))
        {
          zipHelperStream.IsStreamOwner = false;
          zipHelperStream.WriteByte((byte) this._flags);
          if ((this._flags & ExtendedUnixData.Flags.ModificationTime) != (ExtendedUnixData.Flags) 0)
          {
            int num = (int) (this._modificationTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(num);
          }
          if ((this._flags & ExtendedUnixData.Flags.AccessTime) != (ExtendedUnixData.Flags) 0)
          {
            int num = (int) (this._lastAccessTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(num);
          }
          if ((this._flags & ExtendedUnixData.Flags.CreateTime) != (ExtendedUnixData.Flags) 0)
          {
            int num = (int) (this._createTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(num);
          }
          return memoryStream.ToArray();
        }
      }
    }

    public static bool IsValidValue(DateTime value)
    {
      if (!(value >= new DateTime(1901, 12, 13, 20, 45, 52)))
        return value <= new DateTime(2038, 1, 19, 3, 14, 7);
      return true;
    }

    [System.Flags]
    public enum Flags : byte
    {
      ModificationTime = (byte) 1,
      AccessTime = (byte) 2,
      CreateTime = (byte) 4,
    }
  }
}
