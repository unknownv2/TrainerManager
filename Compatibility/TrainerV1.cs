
using System.Collections.Generic;


namespace TrainerManager.Compatibility;

public class TrainerV1
{
  public string InjectionTarget { get; set; }

  public string ExePath { get; set; }

  public string ExecutionArgs { get; set; }

  public Dictionary<string, ModV1> Mods { get; set; }

  public List<KeyboardInputV1> KeyboardInputs { get; set; }

  public List<RemoteGroupV1> RemoteGroups { get; set; }
}
