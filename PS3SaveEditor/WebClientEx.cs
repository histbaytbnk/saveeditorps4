
// Type: PS3SaveEditor.WebClientEx


// Hacked by SystemAce

using System;
using System.Net;
using System.Net.Security;
using System.Text;

namespace PS3SaveEditor
{
  internal class WebClientEx : WebClient
  {
    protected override WebRequest GetWebRequest(Uri address)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) base.GetWebRequest(address);
      httpWebRequest.UserAgent = Util.GetUserAgent();
      httpWebRequest.PreAuthenticate = true;
      string str = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Util.GetHtaccessUser() + ":" + Util.GetHtaccessPwd()));
      httpWebRequest.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
      httpWebRequest.Headers.Add("Authorization", str);
      return (WebRequest) httpWebRequest;
    }
  }
}
