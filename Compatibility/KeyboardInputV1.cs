
using System.Collections.Generic;


namespace TrainerManager.Compatibility;

public class KeyboardInputV1
{
  public int DefaultKey { get; set; }

  public string DisplayName { get; set; }

  public List<TransformationV1> Transformations { get; set; }
}
