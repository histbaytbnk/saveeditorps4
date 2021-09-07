
// Type: PS3SaveEditor.Program


// Hacked by SystemAce

using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  internal static class Program
  {
    public static string[] HTACCESS_USER = new string[2]
    {
      "savewizard_1",
      "savewizard_1"
    };
    public static string[] HTACCESS_PWD = new string[2]
    {
      "Wd2l#@vqjun)3K",
      "Wd2l#@vqjun)3K"
    };
    private static Form mainForm;

    [STAThread]
    private static void Main()
    {
      SingleInstanceApplication instanceApplication = new SingleInstanceApplication();
    //  instanceApplication.Startup += new StartupEventHandler(Program.app_Startup);
   //   instanceApplication.StartupNextInstance += new StartupNextInstanceEventHandler(Program.OnAppStartupNextInstance);
      Program.mainForm = (Form) new MainForm3();
      instanceApplication.Run(Program.mainForm);
    }

    private static void app_Startup(object sender, StartupEventArgs e)
    {
    }

    private static void OnAppStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
    {
      if (Program.mainForm.WindowState == FormWindowState.Minimized)
        Program.mainForm.WindowState = FormWindowState.Normal;
      //Program.mainForm.Activate();
    }
  }
}
