
// Type: PS3SaveEditor.file


// Hacked by SystemAce

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("file")]
  public class file
  {
    public internals internals;
    public string type;
    public string textmode;

    public string filename { get; set; }

    public string id { get; set; }

    public string title { get; set; }

    public string dependency { get; set; }

    public string Option { get; set; }

    public string altname { get; set; }

    public string ucfilename { get; set; }

    public cheats cheats { get; set; }

    [XmlIgnore]
    public List<cheat> Cheats
    {
      get
      {
        return this.cheats._cheats;
      }
      set
      {
        this.cheats._cheats = value;
      }
    }

    [XmlIgnore]
    public List<group> groups
    {
      get
      {
        return this.cheats.groups;
      }
      set
      {
        this.cheats.groups = value;
      }
    }

    public int TotalCheats
    {
      get
      {
        return this.cheats.TotalCheats;
      }
    }

    public string VisibleFileName
    {
      get
      {
        if (!string.IsNullOrEmpty(this.altname))
          return string.Format("{0} ({1})", (object) this.altname, (object) this.filename);
        return this.filename;
      }
    }

    public int TextMode
    {
      get
      {
        switch (this.textmode)
        {
          case "":
            return 0;
          case "utf-8":
            return 1;
          case "ascii":
            return 2;
          case "utf-16":
            return 3;
          case null:
            return 0;
          default:
            return 0;
        }
      }
    }

    public bool IsHidden
    {
      get
      {
        return this.type == "hidden";
      }
    }

    public file()
    {
      this.cheats = new cheats();
    }

    public List<cheat> GetAllCheats()
    {
      List<cheat> list = new List<cheat>();
      list.AddRange((IEnumerable<cheat>) this.Cheats);
      foreach (group group in this.groups)
        list.AddRange((IEnumerable<cheat>) group.GetGroupCheats());
      return list;
    }

    public string GetDependencyFile(container gameFolder, string folder)
    {
      if (string.IsNullOrEmpty(this.dependency))
        return (string) null;
      foreach (file file1 in gameFolder.files._files)
      {
        if (file1.id == this.dependency)
        {
          string path2 = file1.GetSaveFile(folder);
          if (path2 == null)
          {
            foreach (file file2 in gameFolder.files._files)
            {
              if (file2.id == file1.dependency)
                path2 = file2.filename;
            }
          }
          if (path2 != null)
            return Path.Combine(folder, path2);
        }
      }
      return (string) null;
    }

    internal static file Copy(file gameFile)
    {
      file file = new file();
      file.filename = gameFile.filename;
      file.dependency = gameFile.dependency;
      file.title = gameFile.title;
      file.id = gameFile.id;
      file.Option = gameFile.Option;
      file.altname = gameFile.altname;
      if (gameFile.internals != null)
      {
        file.internals = new internals();
        foreach (file gameFile1 in gameFile.internals.files)
          file.internals.files.Add(file.Copy(gameFile1));
      }
      file.cheats = new cheats();
      foreach (group g in gameFile.groups)
        file.groups.Add(group.Copy(g));
      file.textmode = gameFile.textmode;
      file.type = gameFile.type;
      file.ucfilename = gameFile.ucfilename;
      foreach (cheat cheat in gameFile.Cheats)
        file.Cheats.Add(cheat.Copy(cheat));
      return file;
    }

    internal string GetSaveFile(string saveFolder)
    {
      string[] files = Directory.GetFiles(saveFolder, this.filename);
      if (files.Length > 0)
        return Path.GetFileName(files[0]);
      return (string) null;
    }

    internal List<string> GetSaveFiles(string saveFolder)
    {
      string[] files = Directory.GetFiles(saveFolder, this.filename);
      if (files.Length <= 0)
        return (List<string>) null;
      List<string> list = new List<string>((IEnumerable<string>) files);
      list.Sort();
      return list;
    }

    internal static file GetGameFile(container gameFolder, string folder, string file)
    {
      foreach (file file1 in gameFolder.files._files)
      {
        if (file1.filename == file || Util.IsMatch(file, file1.filename))
          return file1;
      }
      return (file) null;
    }

    internal cheat GetCheat(string cd)
    {
      foreach (cheat cheat in this.Cheats)
      {
        if (cd == cheat.name)
          return cheat;
      }
      foreach (group group in this.groups)
      {
        cheat cheat = group.GetCheat(cd);
        if (cheat != null)
          return cheat;
      }
      return (cheat) null;
    }

    public file GetParent(container gamefolder)
    {
      foreach (file file1 in gamefolder.files._files)
      {
        if (file1.id == this.id)
          return (file) null;
        if (file1.internals != null)
        {
          foreach (file file2 in file1.internals.files)
          {
            if (file2.id == this.id)
              return file1;
          }
        }
      }
      return (file) null;
    }
  }
}
