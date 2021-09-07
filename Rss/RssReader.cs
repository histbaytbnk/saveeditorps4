
// Type: Rss.RssReader


// Hacked by SystemAce

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Rss
{
  public class RssReader
  {
    private Stack xmlNodeStack = new Stack();
    private StringBuilder elementText = new StringBuilder();
    private ExceptionCollection exceptions = new ExceptionCollection();
    private XmlTextReader reader;
    private bool wroteChannel;
    private RssVersion rssVersion;
    private RssTextInput textInput;
    private RssImage image;
    private RssCloud cloud;
    private RssChannel channel;
    private RssSource source;
    private RssEnclosure enclosure;
    private RssGuid guid;
    private RssCategory category;
    private RssItem item;

    public ExceptionCollection Exceptions
    {
      get
      {
        return this.exceptions;
      }
    }

    public RssVersion Version
    {
      get
      {
        return this.rssVersion;
      }
    }

    public RssReader(string url)
    {
      try
      {
        this.reader = new XmlTextReader(url);
        this.InitReader();
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Unable to retrieve file containing the RSS data.", ex);
      }
    }

    public RssReader(TextReader textReader)
    {
      try
      {
        this.reader = new XmlTextReader(textReader);
        this.InitReader();
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Unable to retrieve file containing the RSS data.", ex);
      }
    }

    public RssReader(Stream stream)
    {
      try
      {
        this.reader = new XmlTextReader(stream);
        this.InitReader();
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Unable to retrieve file containing the RSS data.", ex);
      }
    }

    private void InitReader()
    {
      this.reader.WhitespaceHandling = WhitespaceHandling.None;
      this.reader.XmlResolver = (XmlResolver) null;
    }

    public RssElement Read()
    {
      bool flag1 = false;
      RssElement rssElement = (RssElement) null;
      int num1 = -1;
      int num2 = -1;
      if (this.reader == null)
        throw new InvalidOperationException("RssReader has been closed, and can not be read.");
      do
      {
        bool flag2 = true;
        try
        {
          flag1 = this.reader.Read();
        }
        catch (EndOfStreamException ex)
        {
          throw new EndOfStreamException("Unable to read an RssElement. Reached the end of the stream.", (Exception) ex);
        }
        catch (XmlException ex)
        {
          if ((num1 != -1 || num2 != -1) && (this.reader.LineNumber == num1 && this.reader.LinePosition == num2))
            throw this.exceptions.LastException;
          num1 = this.reader.LineNumber;
          num2 = this.reader.LinePosition;
          this.exceptions.Add((Exception) ex);
        }
        if (flag1)
        {
          string str1 = this.reader.Name.ToLower();
          switch (this.reader.NodeType)
          {
            case XmlNodeType.Element:
              if (!this.reader.IsEmptyElement)
              {
                this.elementText = new StringBuilder();
                switch (str1)
                {
                  case "item":
                    if (!this.wroteChannel)
                    {
                      this.wroteChannel = true;
                      rssElement = (RssElement) this.channel;
                      flag1 = false;
                    }
                    this.item = new RssItem();
                    this.channel.Items.Add(this.item);
                    break;
                  case "source":
                    this.source = new RssSource();
                    this.item.Source = this.source;
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      switch (this.reader.Name.ToLower())
                      {
                        case "url":
                          try
                          {
                            this.source.Url = new Uri(this.reader.Value);
                            break;
                          }
                          catch (Exception ex)
                          {
                            this.exceptions.Add(ex);
                            break;
                          }
                      }
                    }
                    break;
                  case "enclosure":
                    this.enclosure = new RssEnclosure();
                    this.item.Enclosure = this.enclosure;
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      switch (this.reader.Name.ToLower())
                      {
                        case "url":
                          try
                          {
                            this.enclosure.Url = new Uri(this.reader.Value);
                            break;
                          }
                          catch (Exception ex)
                          {
                            this.exceptions.Add(ex);
                            break;
                          }
                        case "length":
                          try
                          {
                            this.enclosure.Length = int.Parse(this.reader.Value);
                            break;
                          }
                          catch (Exception ex)
                          {
                            this.exceptions.Add(ex);
                            break;
                          }
                        case "type":
                          this.enclosure.Type = this.reader.Value;
                          break;
                      }
                    }
                    break;
                  case "guid":
                    this.guid = new RssGuid();
                    this.item.Guid = this.guid;
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      switch (this.reader.Name.ToLower())
                      {
                        case "ispermalink":
                          try
                          {
                            this.guid.PermaLink = (DBBool) bool.Parse(this.reader.Value);
                            break;
                          }
                          catch (Exception ex)
                          {
                            this.exceptions.Add(ex);
                            break;
                          }
                      }
                    }
                    break;
                  case "category":
                    this.category = new RssCategory();
                    if ((string) this.xmlNodeStack.Peek() == "channel")
                      this.channel.Categories.Add(this.category);
                    else
                      this.item.Categories.Add(this.category);
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      switch (this.reader.Name.ToLower())
                      {
                        case "url":
                        case "domain":
                          this.category.Domain = this.reader.Value;
                          break;
                      }
                    }
                    break;
                  case "channel":
                    this.channel = new RssChannel();
                    this.textInput = (RssTextInput) null;
                    this.image = (RssImage) null;
                    this.cloud = (RssCloud) null;
                    this.source = (RssSource) null;
                    this.enclosure = (RssEnclosure) null;
                    this.category = (RssCategory) null;
                    this.item = (RssItem) null;
                    break;
                  case "image":
                    this.image = new RssImage();
                    this.channel.Image = this.image;
                    break;
                  case "textinput":
                    this.textInput = new RssTextInput();
                    this.channel.TextInput = this.textInput;
                    break;
                  case "cloud":
                    flag2 = false;
                    this.cloud = new RssCloud();
                    this.channel.Cloud = this.cloud;
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      switch (this.reader.Name.ToLower())
                      {
                        case "domain":
                          this.cloud.Domain = this.reader.Value;
                          break;
                        case "port":
                          try
                          {
                            this.cloud.Port = (int) ushort.Parse(this.reader.Value);
                            break;
                          }
                          catch (Exception ex)
                          {
                            this.exceptions.Add(ex);
                            break;
                          }
                        case "path":
                          this.cloud.Path = this.reader.Value;
                          break;
                        case "registerprocedure":
                          this.cloud.RegisterProcedure = this.reader.Value;
                          break;
                        case "protocol":
                          switch (this.reader.Value.ToLower())
                          {
                            case "xml-rpc":
                              this.cloud.Protocol = RssCloudProtocol.XmlRpc;
                              continue;
                            case "soap":
                              this.cloud.Protocol = RssCloudProtocol.Soap;
                              continue;
                            case "http-post":
                              this.cloud.Protocol = RssCloudProtocol.HttpPost;
                              continue;
                            default:
                              this.cloud.Protocol = RssCloudProtocol.Empty;
                              continue;
                          }
                      }
                    }
                    break;
                  case "rss":
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      if (this.reader.Name.ToLower() == "version")
                      {
                        switch (this.reader.Value)
                        {
                          case "0.91":
                            this.rssVersion = RssVersion.RSS091;
                            continue;
                          case "0.92":
                            this.rssVersion = RssVersion.RSS092;
                            continue;
                          case "2.0":
                            this.rssVersion = RssVersion.RSS20;
                            continue;
                          default:
                            this.rssVersion = RssVersion.NotSupported;
                            continue;
                        }
                      }
                    }
                    break;
                  case "rdf":
                    for (int i = 0; i < this.reader.AttributeCount; ++i)
                    {
                      this.reader.MoveToAttribute(i);
                      if (this.reader.Name.ToLower() == "version")
                      {
                        switch (this.reader.Value)
                        {
                          case "0.90":
                            this.rssVersion = RssVersion.RSS090;
                            continue;
                          case "1.0":
                            this.rssVersion = RssVersion.RSS10;
                            continue;
                          default:
                            this.rssVersion = RssVersion.NotSupported;
                            continue;
                        }
                      }
                    }
                    break;
                }
                if (flag2)
                {
                  this.xmlNodeStack.Push((object) str1);
                  break;
                }
                break;
              }
              break;
            case XmlNodeType.Text:
              this.elementText.Append(this.reader.Value);
              break;
            case XmlNodeType.CDATA:
              this.elementText.Append(this.reader.Value);
              break;
          
          }
        }
      }
      while (flag1);
      return rssElement;
    }

    public void Close()
    {
      this.textInput = (RssTextInput) null;
      this.image = (RssImage) null;
      this.cloud = (RssCloud) null;
      this.channel = (RssChannel) null;
      this.source = (RssSource) null;
      this.enclosure = (RssEnclosure) null;
      this.category = (RssCategory) null;
      this.item = (RssItem) null;
      if (this.reader != null)
      {
        this.reader.Close();
        this.reader = (XmlTextReader) null;
      }
      this.elementText = (StringBuilder) null;
      this.xmlNodeStack = (Stack) null;
    }
  }
}
