
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace TrainerManager;

public class Game
{
  [JsonIgnore]
  public string Location { get; set; }

  [JsonIgnore]
  public string Title => Path.GetFileName(this.Location);

  [JsonProperty("x64")]
  public bool X64 { get; set; }

  [JsonProperty("launchArgs")]
  public string LaunchArgs { get; set; }

  [JsonProperty("modules")]
  public List<string> Modules { get; set; }

  [JsonProperty("directories")]
  public List<string> Directories { get; set; }

  [JsonProperty("registryLocations")]
  public List<string> RegistryLocations { get; set; }

  [JsonProperty("cheats")]
  public List<Cheat> Cheats { get; set; }

  public override string ToString() => this.Title;
}
