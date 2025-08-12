
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class OffsetSequence
{
  [JsonProperty("Offset")]
  public List<string> Entries { get; set; }
}
