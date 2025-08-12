
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.CheatEngine;

public class CheatCodes
{
  [JsonProperty("CodeEntry")]
  public List<CheatCodeEntry> Entries;
}
