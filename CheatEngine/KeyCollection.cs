
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class KeyCollection
{
  [JsonProperty("Key")]
  public List<int> Entries { get; set; }
}
