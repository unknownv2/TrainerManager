
using Newtonsoft.Json;


namespace TrainerManager.CheatEngine;

public class CheatEntry
{
  [JsonProperty("ID")]
  public int Id { get; set; }

  public string Description { get; set; }

  public object LastState { get; set; }

  public string VariableType { get; set; }

  public string Address { get; set; }

  public OffsetSequence Offsets { get; set; }

  public string AssemblerScript { get; set; }

  public Hotkeys HotKeys { get; set; }
}
