
using System;


namespace TrainerManager.CheatEngine;

public class GameVersion
{
  public static readonly GameVersion Unknown = new GameVersion()
  {
    Filename = "Unknown.cpp",
    Name = nameof (Unknown),
    Timestamp = uint.MaxValue
  };
  public string Filename;
  public string Name;
  public uint Timestamp;

  public bool IsUnknown => this.Timestamp == uint.MaxValue;

  public override string ToString()
  {
    if (this.IsUnknown)
      return "Unknown Version";
    return $"{(object) this.Timestamp} ({this.Name})";
  }

  public static GameVersion FromTimestamp(uint timestamp)
  {
    GameVersion gameVersion = new GameVersion()
    {
      Timestamp = timestamp,
      Name = DateTimeOffset.FromUnixTimeSeconds((long) timestamp).DateTime.ToString("yyyy-MM-dd")
    };
    gameVersion.Filename = $"{gameVersion.Name} - {(object) timestamp}.cpp";
    return gameVersion;
  }
}
