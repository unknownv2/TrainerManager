

namespace TrainerManager.CheatEngine;

public class CheatCodeEntry
{
  public string Description { get; set; }

  public string Address { get; set; }

  public string ModuleName { get; set; }

  public string ModuleNameOffset { get; set; }

  public ByteSequence Before { get; set; }

  public ByteSequence Actual { get; set; }

  public ByteSequence After { get; set; }
}
