
using System.Collections.Generic;


namespace TrainerManager.Compatibility;

public class RemoteGroupV1
{
  public string Title { get; set; }

  public bool ExpandedByDefault { get; set; }

  public List<RemoteInputV1> Inputs { get; set; }
}
