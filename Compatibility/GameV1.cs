
using Newtonsoft.Json;
using System.Collections.Generic;


namespace TrainerManager.Compatibility;

public class GameV1
{
  [JsonIgnore]
  public string Location { get; set; }

  public string Guid { get; set; }

  public string Title { get; set; }

  public string IconFile { get; set; }

  public uint Color { get; set; }

  public int? SteamId { get; set; }

  public string WindowsPackageFamily { get; set; }

  public List<string> RequiredFiles { get; set; }

  public List<string> PossibleLocations { get; set; }

  public List<string> PossibleLocationsDev { get; set; }

  public List<string> PossibleLocationsRegistry { get; set; }

  public Dictionary<string, Dictionary<string, uint>> KnownVersions { get; set; }

  public ToolV1 Tool { get; set; }
}
