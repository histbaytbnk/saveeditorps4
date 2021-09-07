
// Type: PS3SaveEditor.group


// Hacked by SystemAce

using System.Collections.Generic;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  public class group
  {
    public string name { get; set; }

    public string note { get; set; }

    public string options { get; set; }

    [XmlElement("cheat")]
    public List<cheat> cheats { get; set; }

    [XmlElement(ElementName = "group")]
    public List<group> _group { get; set; }

    public int TotalCheats
    {
      get
      {
        int num = 0;
        if (this.cheats != null)
          num = this.cheats.Count;
        if (this._group != null)
        {
          foreach (group group in this._group)
            num += group.TotalCheats;
        }
        return num;
      }
    }

    public bool CheatsSelected
    {
      get
      {
        foreach (cheat cheat in this.cheats)
        {
          if (cheat.Selected)
            return true;
        }
        if (this._group != null)
        {
          foreach (group group in this._group)
          {
            if (group.CheatsSelected)
              return true;
          }
        }
        return false;
      }
    }

    public string SelectedCheats
    {
      get
      {
        string str = "";
        foreach (cheat cheat in this.cheats)
        {
          if (cheat.Selected)
            str = str + "<id>" + cheat.id + "</id>";
        }
        if (this._group != null)
        {
          foreach (group group in this._group)
            str += group.SelectedCheats;
        }
        return str;
      }
    }

    public group()
    {
      this.cheats = new List<cheat>();
    }

    public List<cheat> GetAllCheats()
    {
      List<cheat> list = new List<cheat>();
      if (this._group != null)
      {
        foreach (group group in this._group)
          list.AddRange((IEnumerable<cheat>) group.cheats);
      }
      list.AddRange((IEnumerable<cheat>) this.cheats);
      return list;
    }

    internal static group Copy(group g)
    {
      group group = new group();
      group.name = g.name;
      group.note = g.note;
      group.options = g.options;
      if (g._group != null)
      {
        group._group = new List<group>();
        foreach (group g1 in g._group)
          group._group.Add(group.Copy(g1));
      }
      foreach (cheat cheat in g.cheats)
        group.cheats.Add(cheat.Copy(cheat));
      return group;
    }

    public cheat GetCheat(string cd)
    {
      foreach (cheat cheat in this.cheats)
      {
        if (cd == cheat.name)
          return cheat;
      }
      if (this._group != null)
      {
        foreach (group group in this._group)
        {
          cheat cheat = group.GetCheat(cd);
          if (cheat != null)
            return cheat;
        }
      }
      return (cheat) null;
    }

    internal int GetCheatsCount()
    {
      int count = this.cheats.Count;
      if (this._group != null)
      {
        using (List<group>.Enumerator enumerator = this._group.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            group current = enumerator.Current;
            return count + current.GetCheatsCount();
          }
        }
      }
      return count;
    }

    internal List<cheat> GetCheats()
    {
      List<cheat> list = new List<cheat>();
      list.AddRange((IEnumerable<cheat>) list);
      if (this._group != null)
      {
        foreach (group group in this._group)
          list.AddRange((IEnumerable<cheat>) group.GetCheats());
      }
      return list;
    }

    internal List<cheat> GetGroupCheats()
    {
      List<cheat> list = new List<cheat>();
      list.AddRange((IEnumerable<cheat>) this.cheats);
      if (this._group != null)
      {
        foreach (group group in this._group)
          list.AddRange((IEnumerable<cheat>) group.GetGroupCheats());
      }
      return list;
    }
  }
}
