
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager;

public class CheatInput
{
  [JsonProperty("type")]
  public string Type { get; set; }

  [JsonProperty("target")]
  public string Target { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("hotkeys")]
  public List<HotkeyInput> Hotkeys { get; set; }

  [JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
  public InputArgs Args { get; set; }

  public override string ToString() => this.Target;
}
