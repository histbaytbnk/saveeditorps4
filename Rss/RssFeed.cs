
// Type: Rss.RssFeed


// Hacked by SystemAce

using PS3SaveEditor;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace Rss
{
  public class RssFeed
  {
    private RssChannelCollection channels = new RssChannelCollection();
    private RssModuleCollection modules = new RssModuleCollection();
    private DateTime lastModified = RssDefault.DateTime;
    private string etag = "";
    private string url = "";
    private ExceptionCollection exceptions;
    private RssVersion rssVersion;
    private bool cached;
    private Encoding encoding;

    public RssChannelCollection Channels
    {
      get
      {
        return this.channels;
      }
    }

    public RssModuleCollection Modules
    {
      get
      {
        return this.modules;
      }
    }

    public ExceptionCollection Exceptions
    {
      get
      {
        if (this.exceptions != null)
          return this.exceptions;
        return new ExceptionCollection();
      }
    }

    public RssVersion Version
    {
      get
      {
        return this.rssVersion;
      }
      set
      {
        this.rssVersion = value;
      }
    }

    public string ETag
    {
      get
      {
        return this.etag;
      }
    }

    public DateTime LastModified
    {
      get
      {
        return this.lastModified;
      }
    }

    public bool Cached
    {
      get
      {
        return this.cached;
      }
    }

    public string Url
    {
      get
      {
        return this.url;
      }
    }

    public Encoding Encoding
    {
      get
      {
        return this.encoding;
      }
      set
      {
        this.encoding = value;
      }
    }

    public RssFeed()
    {
    }

    public RssFeed(Encoding encoding)
    {
      this.encoding = encoding;
    }

    public override string ToString()
    {
      return this.url;
    }

    public static RssFeed Read(string url)
    {
      return RssFeed.read(url, (HttpWebRequest) null, (RssFeed) null);
    }

    public static RssFeed Read(HttpWebRequest Request)
    {
      return RssFeed.read(Request.RequestUri.ToString(), Request, (RssFeed) null);
    }

    public static RssFeed Read(RssFeed oldFeed)
    {
      return RssFeed.read(oldFeed.url, (HttpWebRequest) null, oldFeed);
    }

    public static RssFeed Read(HttpWebRequest Request, RssFeed oldFeed)
    {
      return RssFeed.read(oldFeed.url, Request, oldFeed);
    }

    private static RssFeed read(string url, HttpWebRequest request, RssFeed oldFeed)
    {
      RssFeed rssFeed = new RssFeed();
      Stream stream = (Stream) null;
      Uri requestUri = new Uri(url);
      rssFeed.url = url;
      switch (requestUri.Scheme)
      {
        case "file":
          rssFeed.lastModified = System.IO.File.GetLastWriteTime(url);
          if (oldFeed != null && rssFeed.LastModified == oldFeed.LastModified)
          {
            oldFeed.cached = true;
            return oldFeed;
          }
          stream = (Stream) new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
          break;
        case "https":
        case "http":
          if (request == null)
            request = (HttpWebRequest) WebRequest.Create(requestUri);
          request.Credentials = (ICredentials) Util.GetNetworkCredential();
          string str = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Util.GetHtaccessUser() + ":" + Util.GetHtaccessPwd()));
          request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
          request.Headers.Add("Authorization", str);
          request.UserAgent = Util.GetUserAgent();
          request.PreAuthenticate = true;
          if (oldFeed != null)
          {
            request.IfModifiedSince = oldFeed.LastModified;
            request.Headers.Add("If-None-Match", oldFeed.ETag);
          }
          try
          {
            HttpWebResponse httpWebResponse = (HttpWebResponse) request.GetResponse();
            rssFeed.lastModified = httpWebResponse.LastModified;
            rssFeed.etag = httpWebResponse.Headers["ETag"];
            try
            {
              if (httpWebResponse.ContentEncoding != "")
                rssFeed.encoding = Encoding.GetEncoding(httpWebResponse.ContentEncoding);
            }
            catch
            {
            }
            stream = httpWebResponse.GetResponseStream();
            break;
          }
          catch (WebException ex)
          {
            if (oldFeed == null)
              throw ex;
            oldFeed.cached = true;
            return oldFeed;
          }
      }
      if (stream == null)
        throw new ApplicationException("Not a valid Url");
      RssReader rssReader = (RssReader) null;
      try
      {
        rssReader = new RssReader(stream);
        RssElement rssElement;
        do
        {
          rssElement = rssReader.Read();
          if (rssElement is RssChannel)
            rssFeed.Channels.Add((RssChannel) rssElement);
        }
        while (rssElement != null);
        rssFeed.rssVersion = rssReader.Version;
      }
      finally
      {
        rssFeed.exceptions = rssReader.Exceptions;
        rssReader.Close();
      }
      return rssFeed;
    }

    public void Write(Stream stream)
    {
      this.write(this.encoding != null ? new RssWriter(stream, this.encoding) : new RssWriter(stream));
    }

    public void Write(string fileName)
    {
      this.write(new RssWriter(fileName));
    }

    private void write(RssWriter writer)
    {
      try
      {
        if (this.channels.Count == 0)
          throw new InvalidOperationException("Feed must contain at least one channel.");
        writer.Version = this.rssVersion;
        writer.Modules = this.modules;
        foreach (RssChannel channel in (CollectionBase) this.channels)
        {
          if (channel.Items.Count == 0)
            throw new InvalidOperationException("Channel must contain at least one item.");
          writer.Write(channel);
        }
      }
      finally
      {
        if (writer != null)
          writer.Close();
      }
    }
  }
}
