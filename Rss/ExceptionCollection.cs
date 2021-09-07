
// Type: Rss.ExceptionCollection


// Hacked by SystemAce

using System;
using System.Collections;

namespace Rss
{
  public class ExceptionCollection : CollectionBase
  {
    private Exception lastException;

    public Exception this[int index]
    {
      get
      {
        return (Exception) this.List[index];
      }
      set
      {
        this.List[index] = (object) value;
      }
    }

    public Exception LastException
    {
      get
      {
        return this.lastException;
      }
    }

    public int Add(Exception exception)
    {
      foreach (Exception exception1 in (IEnumerable) this.List)
      {
        if (exception1.Message == exception.Message)
          return -1;
      }
      this.lastException = exception;
      return this.List.Add((object) exception);
    }

    public bool Contains(Exception exception)
    {
      return this.List.Contains((object) exception);
    }

    public void CopyTo(Exception[] array, int index)
    {
      this.List.CopyTo((Array) array, index);
    }

    public int IndexOf(Exception exception)
    {
      return this.List.IndexOf((object) exception);
    }

    public void Insert(int index, Exception exception)
    {
      this.List.Insert(index, (object) exception);
    }

    public void Remove(Exception exception)
    {
      this.List.Remove((object) exception);
    }
  }
}
