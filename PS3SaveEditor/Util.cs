
// Type: PS3SaveEditor.Util


// Hacked by SystemAce

using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  internal class Util
  {
    public static string PRODUCT_NAME = "Save Wizard for PS4 MAX";
    public static string[] SERVERS = new string[2]
    {
      "ps4ws.savewizard.net:8082/ps4auth",
      "ps4ws.savewizard.net:8082/ps4auth"
    };
    public static string[] AUTH_SERVERS = new string[2]
    {
      "ps4ws.savewizard.net:8082/ps4auth",
      "ps4ws.savewizard.net:8082/ps4auth"
    };
    public static int CURRENT_AUTH_SERVER = new Random().Next(0, 10) % Util.AUTH_SERVERS.Length;
    public static int CURRENT_SERVER = new Random().Next(0, 10) % Util.SERVERS.Length;
    private static string SessionToken;

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public static string GetBackupLocation()
    {
      string registryValue = Util.GetRegistryValue("Location");
      if (!string.IsNullOrEmpty(registryValue))
      {
        Directory.CreateDirectory(registryValue);
        return registryValue;
      }
      string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + (object) Path.DirectorySeparatorChar + "Save Wizard for PS4 MAX" + (string) (object) Path.DirectorySeparatorChar + "PS4 Saves Backup";
      Directory.CreateDirectory(path);
      return path;
    }

    internal static string GetUserId()
    {
      return Util.GetRegistryValue("User");
    }

    internal static void ChangeServer()
    {
      if (Util.CURRENT_SERVER == 0)
        Util.CURRENT_SERVER = 1;
      else
        Util.CURRENT_SERVER = 0;
    }

    internal static void ChangeAuthServer()
    {
      if (Util.CURRENT_AUTH_SERVER == 0)
        Util.CURRENT_AUTH_SERVER = 1;
      else
        Util.CURRENT_AUTH_SERVER = 0;
    }

    internal static string GetRegistryValue(string key)
    {
      RegistryKey registryKey1 = Registry.CurrentUser;
      RegistryKey registryKey2 = registryKey1.OpenSubKey(Util.GetRegistryBase());
      if (registryKey2 == null)
        return (string) null;
      try
      {
        string str = registryKey2.GetValue(key) as string;
        registryKey2.Close();
        registryKey1.Close();
        return str;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    internal static void DeleteRegistryValue(string key)
    {
      RegistryKey registryKey1 = Registry.CurrentUser;
      RegistryKey registryKey2 = registryKey1.OpenSubKey(Util.GetRegistryBase(), true);
      if (registryKey2 == null)
        return;
      try
      {
        registryKey2.DeleteValue(key);
      }
      catch
      {
      }
      registryKey2.Close();
      registryKey1.Close();
    }

    internal static bool IsMatch(string a, string pattern)
    {
      return Regex.IsMatch(a, "^" + pattern + "$");
    }

    internal static void SetRegistryValue(string key, string value)
    {
      RegistryKey registryKey = Registry.CurrentUser;
      RegistryKey subKey = registryKey.CreateSubKey(Util.GetRegistryBase());
      if (subKey == null)
        return;
      subKey.SetValue(key, (object) value);
      subKey.Close();
      registryKey.Close();
    }

    internal static string GetAdapterName(bool blackListed = false)
    {
      if (blackListed)
        return (string) null;
      return Util.GetRegistryValue("Adapter");
    }

    internal static string GetUID(bool blackListed = false, bool register = false)
    {
      string uidFromWmi = Util.GetUIDFromWMI(Environment.ExpandEnvironmentVariables("%SYSTEMDRIVE%"));
      return uidFromWmi.Substring(uidFromWmi.IndexOf('{') + 1, uidFromWmi.IndexOf('}') - uidFromWmi.IndexOf('{') - 1);
    }

    internal static string GetSerial()
    {
      return Util.GetRegistryValue("Serial");
    }

    private static void zipFile_ExtractProgress(object sender, ExtractProgressEventArgs e)
    {
    }

    internal static string GetHtaccessUser()
    {
      return Program.HTACCESS_USER[Util.CURRENT_SERVER];
    }

    internal static string GetHtaccessPwd()
    {
      return Program.HTACCESS_PWD[Util.CURRENT_SERVER];
    }

    internal static NetworkCredential GetNetworkCredential()
    {
      return new NetworkCredential(Util.GetHtaccessUser(), Util.GetHtaccessPwd());
    }

    internal static string GetBaseUrl()
    {
      return "http://" + Util.SERVERS[Util.CURRENT_SERVER];
    }

    internal static string GetAuthBaseUrl()
    {
      return "http://" + Util.AUTH_SERVERS[Util.CURRENT_AUTH_SERVER];
    }

    internal static void SetAuthToken(string Token)
    {
      Util.SessionToken = Token;
    }

    internal static string GetAuthToken()
    {
      return Util.SessionToken;
    }

    internal static string GetUserAgent()
    {
      return "Save Wizard for PS4 Max " + AboutBox1.AssemblyVersion;
    }

    private static string GetUIDFromWMI(string sysDrive)
    {
      try
      {
        ManagementObjectCollection objectCollection = new ManagementObjectSearcher("SELECT * \r\n                                     FROM   Win32_Volume\r\n                                     WHERE  DriveLetter = '" + sysDrive + "'").Get();
        string str = (string) null;
        foreach (ManagementObject managementObject in objectCollection)
        {
          if (str == null)
          {
            object propertyValue = managementObject.GetPropertyValue("DeviceID");
            if (propertyValue != null)
              str = propertyValue.ToString();
            str.Substring(str.IndexOf('{') + 1).TrimEnd('\\').TrimEnd('}');
          }
          else
            break;
        }
        return str;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(Util.PRODUCT_NAME + " can not start. Please make sure Windows Management Instrumentation service is running.");
      }
      return (string) null;
    }

    internal static void ClearTemp()
    {
      try
      {
        foreach (string path in Directory.GetFiles(Util.GetTempFolder()))
        {
          if (path.IndexOf("Log.txt") <= 0)
            System.IO.File.Delete(path);
        }
      }
      catch (Exception ex)
      {
      }
    }

    internal static string GetTempFolder()
    {
      string path = Path.Combine(Path.GetTempPath(), "SWPS4MAX");
      Directory.CreateDirectory(path);
      return path + "/";
    }

    internal static string GetRegistryBase()
    {
      return "SOFTWARE\\Save Special\\Save Special";
    }

    internal static bool IsTrialMode()
    {
      return false;
    }

    internal static string GetRegion(Dictionary<int, string> regionMap, int p, string exregions)
    {
      string str = "";
      foreach (int index in regionMap.Keys)
      {
        if ((p & index) > 0 && exregions.IndexOf(regionMap[index]) < 0)
          str = str + "[" + regionMap[index] + "]";
      }
      return str;
    }

    internal static byte[] GetPSNId(string saveFolder)
    {
      return Encoding.UTF8.GetBytes(MainForm.GetParamInfo(Path.Combine(saveFolder, "PARAM.SFO"), "ACCOUNT_ID"));
    }

    internal static bool GetCache(string hash)
    {
      try
      {
        WebClientEx webClientEx = new WebClientEx();
        webClientEx.Headers[HttpRequestHeader.UserAgent] = Util.GetUserAgent();
        webClientEx.Credentials = (ICredentials) Util.GetNetworkCredential();
        return Encoding.ASCII.GetString(webClientEx.UploadData(Util.GetBaseUrl() + "/request_cache?token=" + Util.GetAuthToken(), Encoding.ASCII.GetBytes("{\"pfs\":\"" + hash + "\"}"))).IndexOf("true") > 0;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    internal static string GetHash(byte[] buf)
    {
      return BitConverter.ToString(MD5.Create().ComputeHash(buf)).Replace("-", "").ToLower();
    }

    internal static string GetHash(string file)
    {
      using (FileStream fileStream = System.IO.File.OpenRead(file))
        return BitConverter.ToString(MD5.Create().ComputeHash((Stream) fileStream)).Replace("-", "").ToLower();
    }

    internal static string GetCacheFolder(game game, string curCacheFolder)
    {
      string path1 = Path.Combine(Path.Combine(Util.GetBackupLocation(), "cache"), game.id);
      if (string.IsNullOrEmpty(curCacheFolder))
        return path1;
      return Path.Combine(path1, curCacheFolder);
    }
  }
}
