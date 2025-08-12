
using System.Collections.Generic;


namespace TrainerManager.Compatibility;

public class RemoteInputV1
{
  public int Type { get; set; }

  public string ModId { get; set; }

  public string DisplayName { get; set; }

  public string Description { get; set; }

  public double? Step { get; set; }

  public List<string> Options { get; set; }
}
