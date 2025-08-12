
using Newtonsoft.Json;


namespace TrainerManager.CheatEngine;

public class Hotkey
{
  [JsonProperty("ID")]
  public int Id { get; set; }

  public string Action { get; set; }

  public KeyCollection Keys { get; set; }
}
