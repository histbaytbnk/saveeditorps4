
// Type: ICSharpCode.SharpZipLib.Encryption.PkzipClassicDecryptCryptoTransform


// Hacked by SystemAce

using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
  internal class PkzipClassicDecryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
  {
    public bool CanReuseTransform
    {
      get
      {
        return true;
      }
    }

    public int InputBlockSize
    {
      get
      {
        return 1;
      }
    }

    public int OutputBlockSize
    {
      get
      {
        return 1;
      }
    }

    public bool CanTransformMultipleBlocks
    {
      get
      {
        return true;
      }
    }

    internal PkzipClassicDecryptCryptoTransform(byte[] keyBlock)
    {
      this.SetKeys(keyBlock);
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
      byte[] outputBuffer = new byte[inputCount];
      this.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);
      return outputBuffer;
    }

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
      for (int index = inputOffset; index < inputOffset + inputCount; ++index)
      {
        byte ch = (byte) ((uint) inputBuffer[index] ^ (uint) this.TransformByte());
        outputBuffer[outputOffset++] = ch;
        this.UpdateKeys(ch);
      }
      return inputCount;
    }

    public void Dispose()
    {
      this.Reset();
    }
  }
}
