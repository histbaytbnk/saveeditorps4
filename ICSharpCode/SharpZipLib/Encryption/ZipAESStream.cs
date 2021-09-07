
// Type: ICSharpCode.SharpZipLib.Encryption.ZipAESStream


// Hacked by SystemAce

using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
  internal class ZipAESStream : CryptoStream
  {
    private const int AUTH_CODE_LENGTH = 10;
    private const int CRYPTO_BLOCK_SIZE = 16;
    private Stream _stream;
    private ZipAESTransform _transform;
    private byte[] _slideBuffer;
    private int _slideBufStartPos;
    private int _slideBufFreePos;
    private int _blockAndAuth;

    public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode)
      : base(stream, (ICryptoTransform) transform, mode)
    {
      this._stream = stream;
      this._transform = transform;
      this._slideBuffer = new byte[1024];
      this._blockAndAuth = 26;
      if (mode != CryptoStreamMode.Read)
        throw new Exception("ZipAESStream only for read");
    }

    public override int Read(byte[] outBuffer, int offset, int count)
    {
      int num1 = 0;
      while (num1 < count)
      {
        int count1 = this._blockAndAuth - (this._slideBufFreePos - this._slideBufStartPos);
        if (this._slideBuffer.Length - this._slideBufFreePos < count1)
        {
          int index1 = 0;
          int index2 = this._slideBufStartPos;
          while (index2 < this._slideBufFreePos)
          {
            this._slideBuffer[index1] = this._slideBuffer[index2];
            ++index2;
            ++index1;
          }
          this._slideBufFreePos -= this._slideBufStartPos;
          this._slideBufStartPos = 0;
        }
        this._slideBufFreePos += this._stream.Read(this._slideBuffer, this._slideBufFreePos, count1);
        int num2 = this._slideBufFreePos - this._slideBufStartPos;
        if (num2 >= this._blockAndAuth)
        {
          this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, 16, outBuffer, offset);
          num1 += 16;
          offset += 16;
          this._slideBufStartPos += 16;
        }
        else
        {
          if (num2 > 10)
          {
            int inputCount = num2 - 10;
            this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, inputCount, outBuffer, offset);
            num1 += inputCount;
            this._slideBufStartPos += inputCount;
          }
          else if (num2 < 10)
            throw new Exception("Internal error missed auth code");
          byte[] authCode = this._transform.GetAuthCode();
          for (int index = 0; index < 10; ++index)
          {
            if ((int) authCode[index] != (int) this._slideBuffer[this._slideBufStartPos + index])
              throw new Exception("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\nThe file may be damaged.");
          }
          break;
        }
      }
      return num1;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
  }
}
