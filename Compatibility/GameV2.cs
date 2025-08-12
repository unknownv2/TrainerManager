

namespace TrainerManager.Compatibility;

public class GameV2
{
  public RemoteGame RemoteGame { get; set; }

  public Game LocalGame { get; set; }

  public bool Incomplete { get; set; }
}
