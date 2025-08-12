
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager;

public class InputArgs
{
  [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
  public double? Value { get; set; }

  [JsonProperty("presentAsSlider", NullValueHandling = NullValueHandling.Ignore)]
  public bool? PresentAsSlider { get; set; }

  [JsonProperty("defer", NullValueHandling = NullValueHandling.Ignore)]
  public bool? Defer { get; set; }

  [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
  public double? Min { get; set; }

  [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
  public double? Max { get; set; }

  [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
  public double? Step { get; set; }

  [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
  public List<string> Options { get; set; }
}
