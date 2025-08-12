
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TrainerManager;

public class HotkeyInput
{
  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("keys")]
  public int[] Keys { get; set; }

  public override string ToString()
  {
    return string.Join(", ", ((IEnumerable<int>) this.Keys).Select<int, string>((Func<int, string>) (k => k.ToString())));
  }
}
