
using Newtonsoft.Json;


namespace TrainerManager.Compatibility;

public class RemoteProject
{
  [JsonProperty("developerId")]
  public string DeveloperId { get; set; }

  [JsonProperty("gameId")]
  public string GameId { get; set; }

  [JsonProperty("x64")]
  public bool X64 { get; set; }
}
