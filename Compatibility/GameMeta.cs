
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace TrainerManager.Compatibility;

public class GameMeta
{
  [JsonProperty("releaseDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public string ReleaseDate { get; set; }

  [JsonProperty("thumbnailUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
  public string ThumbnailUrl { get; set; }

  public static Dictionary<string, GameMeta> Read(string file)
  {
    return !File.Exists(file) ? new Dictionary<string, GameMeta>() : JsonConvert.DeserializeObject<Dictionary<string, GameMeta>>(File.ReadAllText(file));
  }
}
