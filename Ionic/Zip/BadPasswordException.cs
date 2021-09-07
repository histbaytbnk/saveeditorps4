﻿
// Type: Ionic.Zip.BadPasswordException


// Hacked by SystemAce

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Ionic.Zip
{
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000B")]
  [Serializable]
  public class BadPasswordException : ZipException
  {
    public BadPasswordException()
    {
    }

    public BadPasswordException(string message)
      : base(message)
    {
    }

    public BadPasswordException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected BadPasswordException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
