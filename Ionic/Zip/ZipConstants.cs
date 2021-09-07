
// Type: Ionic.Zip.ZipConstants


// Hacked by SystemAce

namespace Ionic.Zip
{
  internal static class ZipConstants
  {
    public const uint PackedToRemovableMedia = 808471376U;
    public const uint Zip64EndOfCentralDirectoryRecordSignature = 101075792U;
    public const uint Zip64EndOfCentralDirectoryLocatorSignature = 117853008U;
    public const uint EndOfCentralDirectorySignature = 101010256U;
    public const int ZipEntrySignature = 67324752;
    public const int ZipEntryDataDescriptorSignature = 134695760;
    public const int SplitArchiveSignature = 134695760;
    public const int ZipDirEntrySignature = 33639248;
    public const int AesKeySize = 192;
    public const int AesBlockSize = 128;
    public const ushort AesAlgId128 = (ushort) 26126;
    public const ushort AesAlgId192 = (ushort) 26127;
    public const ushort AesAlgId256 = (ushort) 26128;
  }
}
