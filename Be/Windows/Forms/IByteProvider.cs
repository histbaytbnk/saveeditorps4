
// Type: Be.Windows.Forms.IByteProvider


// Hacked by SystemAce

using System;

namespace Be.Windows.Forms
{
  public interface IByteProvider
  {
    long Length { get; }

    event EventHandler LengthChanged;

    event EventHandler<ByteProviderChanged> Changed;

    byte ReadByte(long index);

    void WriteByte(long index, byte value, bool noEvt = false);

    void InsertBytes(long index, byte[] bs);

    void DeleteBytes(long index, long length);

    bool HasChanges();

    void ApplyChanges();

    bool SupportsWriteByte();

    bool SupportsInsertBytes();

    bool SupportsDeleteBytes();
  }
}
