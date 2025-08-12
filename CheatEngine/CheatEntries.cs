
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class CheatEntries
{
  [JsonProperty("CheatEntry")]
  public List<CheatEntry> Entries { get; set; }
}
