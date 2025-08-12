
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace TrainerManager.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (TrainerManager.Properties.Resources.resourceMan == null)
        TrainerManager.Properties.Resources.resourceMan = new ResourceManager("TrainerManager.Properties.Resources", typeof (TrainerManager.Properties.Resources).Assembly);
      return TrainerManager.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => TrainerManager.Properties.Resources.resourceCulture;
    set => TrainerManager.Properties.Resources.resourceCulture = value;
  }
}
