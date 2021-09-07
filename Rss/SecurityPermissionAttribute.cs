
// Type: Rss.SecurityPermissionAttribute


// Hacked by SystemAce

using System;

namespace Rss
{
  internal class SecurityPermissionAttribute : Attribute
  {
    public bool Execution;

    public SecurityPermissionAttribute(SecurityAction securityAction)
    {
    }
  }
}
