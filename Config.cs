
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager;

public class Config
{
  [JsonProperty("repos")]
  public List<string> Repos { get; set; }
}
