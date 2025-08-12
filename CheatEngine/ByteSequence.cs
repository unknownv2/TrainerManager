
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class ByteSequence
{
  [JsonProperty("Byte")]
  public List<string> Entries { get; set; }
}
