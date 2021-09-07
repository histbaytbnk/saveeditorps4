
// Type: PS3SaveEditor.SingleInstanceApplication


// Hacked by SystemAce

using Microsoft.VisualBasic.ApplicationServices;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace PS3SaveEditor
{
  public class SingleInstanceApplication : WindowsFormsApplicationBase
  {
    public SingleInstanceApplication(AuthenticationMode mode)
      : base(mode)
    {
      this.InitializeAppProperties();
    }

    public SingleInstanceApplication()
    {
      this.InitializeAppProperties();
    }

    protected virtual void InitializeAppProperties()
    {
      this.IsSingleInstance = true;
      this.EnableVisualStyles = true;
    }

    public virtual void Run(Form mainForm)
    {
      this.MainForm = mainForm;
      this.Run(this.CommandLineArgs);
    }

    private void Run(ReadOnlyCollection<string> commandLineArgs)
    {
      this.Run((string[]) new ArrayList((ICollection) commandLineArgs).ToArray(typeof (string)));
    }
  }
}
