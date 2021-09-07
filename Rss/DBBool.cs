
// Type: Rss.DBBool


// Hacked by SystemAce

using System;

namespace Rss
{
  public struct DBBool
  {
    public static readonly DBBool Null = new DBBool(0);
    public static readonly DBBool False = new DBBool(-1);
    public static readonly DBBool True = new DBBool(1);
    private sbyte value;

    public bool IsNull
    {
      get
      {
        return (int) this.value == 0;
      }
    }

    public bool IsFalse
    {
      get
      {
        return (int) this.value < 0;
      }
    }

    public bool IsTrue
    {
      get
      {
        return (int) this.value > 0;
      }
    }

    private DBBool(int value)
    {
      this.value = (sbyte) value;
    }

    public static implicit operator DBBool(bool x)
    {
      if (!x)
        return DBBool.False;
      return DBBool.True;
    }

    public static explicit operator bool(DBBool x)
    {
      if ((int) x.value == 0)
        throw new InvalidOperationException();
      return (int) x.value > 0;
    }

    public static DBBool operator ==(DBBool x, DBBool y)
    {
      if ((int) x.value == 0 || (int) y.value == 0)
        return DBBool.Null;
      if ((int) x.value != (int) y.value)
        return DBBool.False;
      return DBBool.True;
    }

    public static DBBool operator !=(DBBool x, DBBool y)
    {
      if ((int) x.value == 0 || (int) y.value == 0)
        return DBBool.Null;
      if ((int) x.value == (int) y.value)
        return DBBool.False;
      return DBBool.True;
    }

    public static DBBool operator !(DBBool x)
    {
      return new DBBool((int) -x.value);
    }

    public static DBBool operator &(DBBool x, DBBool y)
    {
      return new DBBool((int) x.value < (int) y.value ? (int) x.value : (int) y.value);
    }

    public static DBBool operator |(DBBool x, DBBool y)
    {
      return new DBBool((int) x.value > (int) y.value ? (int) x.value : (int) y.value);
    }

    public static bool operator true(DBBool x)
    {
      return (int) x.value > 0;
    }

    public static bool operator false(DBBool x)
    {
      return (int) x.value < 0;
    }

    public override bool Equals(object o)
    {
      try
      {
        return (bool) (this == (DBBool) o);
      }
      catch
      {
        return false;
      }
    }

    public override int GetHashCode()
    {
      return (int) this.value;
    }

    public override string ToString()
    {
      switch (this.value)
      {
        case (sbyte) -1:
          return "false";
        case (sbyte) 0:
          return "DBBool.Null";
        case (sbyte) 1:
          return "true";
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
