
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class Hotkeys
{
  [JsonProperty("Hotkey")]
  public List<Hotkey> Entries { get; set; }
}
