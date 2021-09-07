
// Type: ICSharpCode.SharpZipLib.Zip.NTTaggedData


// Hacked by SystemAce

using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
  public class NTTaggedData : ITaggedData
  {
    private DateTime _lastAccessTime = DateTime.FromFileTime(0L);
    private DateTime _lastModificationTime = DateTime.FromFileTime(0L);
    private DateTime _createTime = DateTime.FromFileTime(0L);

    public short TagID
    {
      get
      {
        return (short) 10;
      }
    }

    public DateTime LastModificationTime
    {
      get
      {
        return this._lastModificationTime;
      }
      set
      {
        if (!NTTaggedData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._lastModificationTime = value;
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
        if (!NTTaggedData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._createTime = value;
      }
    }

    public DateTime LastAccessTime
    {
      get
      {
        return this._lastAccessTime;
      }
      set
      {
        if (!NTTaggedData.IsValidValue(value))
          throw new ArgumentOutOfRangeException("value");
        this._lastAccessTime = value;
      }
    }

    public void SetData(byte[] data, int index, int count)
    {
      using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
      {
        using (ZipHelperStream zipHelperStream = new ZipHelperStream((Stream) memoryStream))
        {
          zipHelperStream.ReadLEInt();
          while (zipHelperStream.Position < zipHelperStream.Length)
          {
            int num1 = zipHelperStream.ReadLEShort();
            int num2 = zipHelperStream.ReadLEShort();
            if (num1 == 1)
            {
              if (num2 < 24)
                break;
              this._lastModificationTime = DateTime.FromFileTime(zipHelperStream.ReadLELong());
              this._lastAccessTime = DateTime.FromFileTime(zipHelperStream.ReadLELong());
              this._createTime = DateTime.FromFileTime(zipHelperStream.ReadLELong());
              break;
            }
            zipHelperStream.Seek((long) num2, SeekOrigin.Current);
          }
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
          zipHelperStream.WriteLEInt(0);
          zipHelperStream.WriteLEShort(1);
          zipHelperStream.WriteLEShort(24);
          zipHelperStream.WriteLELong(this._lastModificationTime.ToFileTime());
          zipHelperStream.WriteLELong(this._lastAccessTime.ToFileTime());
          zipHelperStream.WriteLELong(this._createTime.ToFileTime());
          return memoryStream.ToArray();
        }
      }
    }

    public static bool IsValidValue(DateTime value)
    {
      bool flag = true;
      try
      {
        value.ToFileTimeUtc();
      }
      catch
      {
        flag = false;
      }
      return flag;
    }
  }
}
