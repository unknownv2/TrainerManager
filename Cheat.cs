
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrainerManager;

public class Cheat
{
  [JsonProperty("uuid")]
  public string Uuid { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("description")]
  public string Description { get; set; }

  [JsonProperty("inputs")]
  public List<CheatInput> Inputs { get; set; }

  public override string ToString() => this.Name;
}
