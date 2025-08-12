
using Newtonsoft.Json;


namespace TrainerManager.Compatibility;

public class RemoteGame
{
  [JsonProperty("title")]
  public string Title { get; set; }

  [JsonProperty("releaseDate")]
  public string ReleaseDate { get; set; }

  [JsonProperty("steamId", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public int? SteamId { get; set; }

  [JsonProperty("windowsPfn", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public string WindowsPfn { get; set; }

  [JsonProperty("gogId", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public int? GogId { get; set; }

  [JsonProperty("thumbnailUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public string ThumbnailUrl { get; set; }
}
