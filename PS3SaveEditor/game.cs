
// Type: PS3SaveEditor.game


// Hacked by SystemAce

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PS3SaveEditor
{
  [XmlRoot("game", Namespace = "")]
  public class game
  {
    public bool LocalCheatExists;

    public string id { get; set; }

    public int acts { get; set; }

    public string notes { get; set; }

    public string diskcode { get; set; }

    public string aliasid { get; set; }

    public string name { get; set; }

    public string version { get; set; }

    public aliases aliases { get; set; }

    public containers containers { get; set; }

    public int region { get; set; }

    public string Client { get; set; }

    public long updated { get; set; }

    public string LocalSaveFolder { get; set; }

    [XmlIgnore]
    public ZipEntry PFSZipEntry { get; set; }

    [XmlIgnore]
    public ZipEntry BinZipEntry { get; set; }

    [XmlIgnore]
    public ZipFile ZipFile { get; set; }

    public string PSN_ID
    {
      get
      {
        try
        {
          if (this.LocalSaveFolder != null)
            return Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(this.LocalSaveFolder)));
        }
        catch
        {
        }
        return (string) null;
      }
    }

    public game()
    {
      this.containers = new containers();
    }

    public override string ToString()
    {
      return this.ToString(false, this.GetSaveFiles());
    }

    public int GetCheatsCount()
    {
      int num = 0;
      foreach (container container in this.containers._containers)
        num += container.GetCheatsCount();
      return num;
    }

    public string ToString(List<string> selectedSaveFiles, string mode = "decrypt")
    {
      this.GetTargetGameFolder();
      List<string> containerFiles = this.GetContainerFiles();
      string str1 = string.Format("<game id=\"{0}\" mode=\"{1}\"><key><name>{2}</name></key><pfs><name>{3}</name></pfs><files>", (object) this.id, (object) mode, (object) Path.GetFileName(containerFiles[0]), (object) Path.GetFileName(containerFiles[1]));
      List<string> list = new List<string>();
      foreach (string path in selectedSaveFiles)
      {
        list.Add(Path.GetFileName(path));
        str1 = !(mode == "encrypt") ? str1 + "<file><name>" + Path.GetFileName(path) + "</name></file>" : str1 + "<file><name>" + Path.GetFileName(path).Replace("_file_", "") + "</name></file>";
      }
      string str2;
      return str2 = str1 + "</files></game>";
    }

    public string ToString(bool bSelectedCheatFilesOnly, List<string> lstSaveFiles)
    {
      container targetGameFolder = this.GetTargetGameFolder();
      List<string> containerFiles = this.GetContainerFiles();
      string str1 = string.Format("<game id=\"{0}\" mode=\"{1}\"><key><name>{2}</name></key><pfs><name>{3}</name></pfs><files>", (object) this.id, (object) "patch", (object) Path.GetFileName(containerFiles[0]), (object) Path.GetFileName(containerFiles[1]));
      this.GetSaveFiles();
      if (targetGameFolder != null)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (string file1 in lstSaveFiles)
        {
          file gameFile = file.GetGameFile(targetGameFolder, this.LocalSaveFolder, file1);
          if (gameFile != null)
          {
            bool flag = false;
            if (!bSelectedCheatFilesOnly)
            {
              flag = true;
            }
            else
            {
              for (int index = 0; index < gameFile.Cheats.Count; ++index)
              {
                if (gameFile.Cheats[index].Selected)
                  flag = true;
              }
              if (gameFile.groups != null)
              {
                foreach (group group in gameFile.groups)
                {
                  if (group.CheatsSelected)
                    flag = true;
                }
              }
            }
            if (flag)
            {
              string str2 = file1;
              if (dictionary.ContainsKey(str2))
              {
                str1 = str1.Replace("<file><fileid>" + gameFile.id + "</fileid><name>" + str2 + "</name></file>", "");
                dictionary.Remove(str2);
              }
              if (!dictionary.ContainsKey(str2) && gameFile.GetParent(targetGameFolder) == null)
              {
                str1 += "<file>";
                str1 = str1 + "<name>" + str2 + "</name>";
                dictionary.Add(str2, gameFile.id);
                if (gameFile.GetAllCheats().Count > 0)
                {
                  str1 += "<cheats>";
                  foreach (cheat cheat1 in gameFile.Cheats)
                  {
                    if (cheat1.Selected)
                    {
                      string str3 = str1;
                      cheat cheat2 = cheat1;
                      int? quickmode = targetGameFolder.quickmode;
                      int num = quickmode.GetValueOrDefault() <= 0 ? 0 : (quickmode.HasValue ? 1 : 0);
                      string str4 = cheat2.ToString(num != 0);
                      str1 = str3 + str4;
                    }
                  }
                  if (gameFile.groups != null)
                  {
                    foreach (group group in gameFile.groups)
                      str1 += group.SelectedCheats;
                  }
                  str1 += "</cheats>";
                }
                str1 += "</file>";
              }
              if (gameFile.GetParent(targetGameFolder) != null)
              {
                file parent = gameFile.GetParent(targetGameFolder);
                if (parent.internals != null)
                {
                  foreach (file file2 in parent.internals.files)
                  {
                    if (!dictionary.ContainsValue(file2.id))
                    {
                      if (file1.IndexOf(file2.filename) > 0)
                      {
                        str1 += "<file>";
                        str1 = str1 + "<fileid>" + gameFile.id + "</fileid>";
                        str1 = str1 + "<name>" + Path.GetFileName(str2) + "</name>";
                        dictionary.Add(Path.GetFileName(str2), gameFile.id);
                        if (gameFile.Cheats.Count > 0)
                        {
                          str1 += "<cheats>";
                          foreach (cheat cheat1 in gameFile.Cheats)
                          {
                            string str3 = str1;
                            cheat cheat2 = cheat1;
                            int? quickmode = targetGameFolder.quickmode;
                            int num = quickmode.GetValueOrDefault() <= 0 ? 0 : (quickmode.HasValue ? 1 : 0);
                            string str4 = cheat2.ToString(num != 0);
                            str1 = str3 + str4;
                          }
                          str1 += "</cheats>";
                        }
                        str1 += "</file>";
                      }
                      else
                      {
                        string path = Path.Combine(this.LocalSaveFolder, file2.filename);
                        str1 = str1 + "<file><fileid>" + file2.id + "</fileid>";
                        str1 = str1 + "<name>" + file2.filename + "</name></file>";
                        dictionary.Add(Path.GetFileName(path), file2.id);
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      string str5;
      return str5 = str1.Replace("<cheats></cheats>", "") + "</files></game>";
    }

    public List<string> GetContainerFiles()
    {
      if (!Directory.Exists(Path.GetDirectoryName(this.LocalSaveFolder)))
        return (List<string>) null;
      List<string> list = new List<string>();
      this.GetTargetGameFolder();
      list.Add(this.LocalSaveFolder);
      list.Add(this.LocalSaveFolder.Substring(0, this.LocalSaveFolder.Length - 4));
      return list;
    }

    public container GetTargetGameFolder()
    {
      container container1 = (container) null;
      if (!Directory.Exists(Path.GetDirectoryName(this.LocalSaveFolder)))
        return (container) null;
      foreach (container container2 in this.containers._containers)
      {
        if ((Path.GetFileNameWithoutExtension(this.LocalSaveFolder) == container2.pfs || Util.IsMatch(Path.GetFileNameWithoutExtension(this.LocalSaveFolder), container2.pfs)) && File.Exists(this.LocalSaveFolder))
        {
          container1 = container2;
          break;
        }
      }
      return container1;
    }

    internal static game Copy(game gameItem)
    {
      game game = new game();
      game.id = gameItem.id;
      game.notes = gameItem.notes;
      game.name = gameItem.name;
      game.acts = gameItem.acts;
      game.diskcode = gameItem.diskcode;
      game.aliasid = gameItem.aliasid;
      game.updated = gameItem.updated;
      game.version = gameItem.version;
      game.region = gameItem.region;
      if (gameItem.aliases != null)
        game.aliases = aliases.Copy(gameItem.aliases);
      foreach (container folder in gameItem.containers._containers)
        game.containers._containers.Add(container.Copy(folder));
      game.Client = gameItem.Client;
      game.LocalCheatExists = gameItem.LocalCheatExists;
      game.LocalSaveFolder = gameItem.LocalSaveFolder;
      return game;
    }

    internal int GetCheatCount()
    {
      int num = 0;
      foreach (container container in this.containers._containers)
      {
        if (container != null)
        {
          foreach (file file1 in container.files._files)
          {
            num += file1.Cheats.Count;
            if (file1.internals != null)
            {
              foreach (file file2 in file1.internals.files)
                num += file2.TotalCheats;
            }
            foreach (group group in file1.groups)
              num += group.TotalCheats;
          }
        }
      }
      return num;
    }

    internal List<cheat> GetAllCheats()
    {
      List<cheat> list = new List<cheat>();
      foreach (container container in this.containers._containers)
      {
        foreach (file file1 in container.files._files)
        {
          list.AddRange((IEnumerable<cheat>) file1.Cheats);
          if (file1.internals != null)
          {
            foreach (file file2 in file1.internals.files)
              list.AddRange((IEnumerable<cheat>) file2.Cheats);
          }
          foreach (group group in file1.groups)
            list.AddRange((IEnumerable<cheat>) group.GetAllCheats());
        }
      }
      return list;
    }

    internal List<string> GetSaveFiles()
    {
      return this.GetSaveFiles(false);
    }

    internal List<string> GetSaveFiles(bool bOnlySelectedCheats)
    {
      List<string> list = new List<string>();
      container targetGameFolder = this.GetTargetGameFolder();
      bool flag = false;
      if (targetGameFolder != null)
      {
        foreach (file file in targetGameFolder.files._files)
          list.Add(file.filename);
      }
      if (flag)
        list.Clear();
      return list;
    }

    internal List<cheat> GetCheats(string saveFolder, string savefile)
    {
      List<cheat> list1 = new List<cheat>();
      foreach (container container in this.containers._containers)
      {
        string[] files1 = Directory.GetFiles(Path.GetDirectoryName(saveFolder));
        List<string> list2 = new List<string>();
        foreach (string path in files1)
        {
          if (Path.GetFileName(path) == container.pfs || Util.IsMatch(Path.GetFileName(path), container.pfs))
            list2.Add(path);
        }
        if (files1.Length > 0 && list2.IndexOf(saveFolder) >= 0)
        {
          foreach (file file in container.files._files)
          {
            string[] files2 = Directory.GetFiles(Util.GetTempFolder(), "*");
            List<string> list3 = new List<string>();
            foreach (string a in files2)
            {
              if (a == file.filename || Util.IsMatch(a, file.filename))
                list3.Add(a);
            }
            foreach (string path in list3.ToArray())
            {
              if (savefile == Path.GetFileName(path) && (file.filename == Path.GetFileName(path) || Util.IsMatch(Path.GetFileName(savefile), file.filename)))
              {
                list1.AddRange((IEnumerable<cheat>) file.Cheats);
                foreach (group group in file.groups)
                {
                  List<cheat> cheats = group.GetCheats();
                  if (cheats != null)
                    list1.AddRange((IEnumerable<cheat>) cheats);
                }
                return list1;
              }
            }
          }
        }
      }
      return (List<cheat>) null;
    }

    internal file GetGameFile(container folder, string savefile)
    {
      if (savefile == null)
        return folder.files._files[0];
      foreach (file file in folder.files._files)
      {
        if (savefile == file.filename || Util.IsMatch(savefile, file.filename))
          return file;
      }
      foreach (file file in folder.files._files)
      {
        foreach (string path in Directory.GetFiles(Util.GetTempFolder(), "*"))
        {
          if (Path.GetFileName(path) == file.filename || Util.IsMatch(Path.GetFileName(path), file.filename))
            return file;
        }
      }
      return (file) null;
    }

    internal bool IsAlias(string gameCode, out string saveId)
    {
      if (this.aliases != null)
      {
        foreach (alias alias in this.aliases._aliases)
        {
          if (gameCode.IndexOf(alias.id) >= 0)
          {
            saveId = alias.id;
            return true;
          }
        }
      }
      saveId = (string) null;
      return false;
    }

    internal bool IsSupported(Dictionary<string, List<game>> m_dictLocalSaves, out string saveID)
    {
      if (this.aliases != null)
      {
        foreach (alias alias in this.aliases._aliases)
        {
          if (m_dictLocalSaves.ContainsKey(alias.id))
          {
            saveID = alias.id;
            return true;
          }
        }
      }
      saveID = (string) null;
      return false;
    }

    internal List<alias> GetAllAliases(bool bAsc = true)
    {
      List<alias> list = new List<alias>();
      list.Add(new alias()
      {
        id = this.id,
        name = this.name,
        region = this.region,
        acts = this.acts
      });
      if (this.aliases != null && this.aliases._aliases != null && this.aliases._aliases.Count > 0)
        list.AddRange((IEnumerable<alias>) this.aliases._aliases);
      list.Sort((Comparison<alias>) ((a1, a2) => a1.id.CompareTo(a2.id)));
      if (!bAsc)
        list.Reverse();
      return list;
    }

    internal cheat GetCheat(string id, string title)
    {
      foreach (container container in this.containers._containers)
      {
        foreach (file file in container.files._files)
        {
          foreach (cheat cheat in file.GetAllCheats())
          {
            if (cheat.id == id && cheat.name == title)
              return cheat;
          }
        }
      }
      return (cheat) null;
    }
  }
}
