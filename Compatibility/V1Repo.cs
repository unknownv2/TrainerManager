
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace TrainerManager.Compatibility;

public class V1Repo
{
  public static int GameIdIndex = 1;

  public string Location { get; set; }

  public List<GameV1> Games { get; set; }

  public V1Repo(string dir) => this.Location = dir;

  public void Load()
  {
    this.Games = new List<GameV1>();
    foreach (string str in Directory.EnumerateDirectories(this.Location).Where<string>((Func<string, bool>) (d => System.IO.File.Exists(Path.Combine(d, "Game.json")))))
    {
      GameV1 gameV1 = JsonConvert.DeserializeObject<GameV1>(System.IO.File.ReadAllText(Path.Combine(str, "Game.json")));
      gameV1.Location = str;
      string path1 = Directory.EnumerateDirectories(str).FirstOrDefault<string>((Func<string, bool>) (d => System.IO.File.Exists(Path.Combine(d, "Tool.json"))));
      if (path1 != null)
      {
        gameV1.Tool = JsonConvert.DeserializeObject<ToolV1>(System.IO.File.ReadAllText(Path.Combine(path1, "Tool.json")));
        gameV1.Tool.Game = gameV1;
        gameV1.Tool.Location = path1;
        this.Games.Add(gameV1);
      }
    }
  }

  public List<GameV2> ConvertAll(string outDir)
  {
    string gameDir = Path.Combine(outDir, "Games");
    if (Directory.Exists(gameDir))
      Directory.Delete(gameDir, true);
    List<GameV2> list1 = this.Games.Select<GameV1, GameV2>((Func<GameV1, GameV2>) (game => game.Tool.ConvertTrainer(Path.Combine(gameDir, V1Repo.Asciify(game.Title))))).ToList<GameV2>();
    RemoteGame[] array = list1.Select<GameV2, RemoteGame>((Func<GameV2, RemoteGame>) (g => g.RemoteGame)).ToArray<RemoteGame>();
    string str = Path.Combine(outDir, "Meta.json");
    Dictionary<string, GameMeta> meta = GameMeta.Read(str);
    V1Repo.FillMissingData(list1, meta);
    System.IO.File.WriteAllText(str, JsonConvert.SerializeObject((object) meta, Formatting.Indented));
    System.IO.File.WriteAllText(Path.Combine(outDir, "Games.json"), JsonConvert.SerializeObject((object) array, Formatting.Indented));
    StringBuilder stringBuilder = new StringBuilder();
    foreach (GameV2 gameV2 in list1.Where<GameV2>((Func<GameV2, bool>) (g => g.Incomplete)))
      stringBuilder.AppendLine($"{gameV2.RemoteGame.Title}");
    System.IO.File.WriteAllText(Path.Combine(outDir, "Incomplete.txt"), stringBuilder.ToString());
    string developerId = System.IO.File.ReadAllText(Path.Combine(outDir, "DeveloperId.txt")).Trim();
    List<RemoteProject> list2 = list1.Select<GameV2, RemoteProject>((Func<GameV2, RemoteProject>) (game => new RemoteProject()
    {
      DeveloperId = developerId,
      GameId = V1Repo.GameIdIndex++.ToString(),
      X64 = game.LocalGame.X64
    })).ToList<RemoteProject>();
    System.IO.File.WriteAllText(Path.Combine(outDir, "Projects.json"), JsonConvert.SerializeObject((object) list2, Formatting.Indented));
    return list1;
  }

  private static void FillMissingData(List<GameV2> games, Dictionary<string, GameMeta> meta)
  {
    List<string> list = meta.Keys.ToList<string>();
    List<string> gameTitles = games.Select<GameV2, string>((Func<GameV2, string>) (g => g.RemoteGame.Title)).ToList<string>();
    Func<string, bool> predicate = (Func<string, bool>) (metaKey => !gameTitles.Contains(metaKey));
    foreach (string key in list.Where<string>(predicate))
      meta.Remove(key);
    foreach (KeyValuePair<string, GameMeta> keyValuePair in meta)
    {
      KeyValuePair<string, GameMeta> metaGame = keyValuePair;
      GameV2 gameV2 = games.Find((Predicate<GameV2>) (f => f.RemoteGame.Title == metaGame.Key));
      if (gameV2.RemoteGame.ReleaseDate == null && metaGame.Value.ReleaseDate != null)
        gameV2.RemoteGame.ReleaseDate = metaGame.Value.ReleaseDate;
      if (gameV2.RemoteGame.ThumbnailUrl == null && metaGame.Value.ThumbnailUrl != null)
        gameV2.RemoteGame.ThumbnailUrl = metaGame.Value.ThumbnailUrl;
    }
    foreach (GameV2 gameV2 in games.Where<GameV2>((Func<GameV2, bool>) (g => g.RemoteGame.ReleaseDate == null && g.RemoteGame.SteamId.HasValue)))
    {
      WebResponse response = WebRequest.CreateHttp("https://store.steampowered.com/api/appdetails?appids=" + (object) gameV2.RemoteGame.SteamId).GetResponse();
      StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      response.Close();
      object obj1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(end)[gameV2.RemoteGame.SteamId.ToString()];
      Thread.Sleep(500);
    }
  }

  private static string Asciify(string str)
  {
    char[] source = new char[9]
    {
      '<',
      '>',
      ':',
      '"',
      '/',
      '\\',
      '|',
      '?',
      '*'
    };
    str = Regex.Replace(str, "[^\\u0000-\\u007F]+", "");
    return ((IEnumerable<char>) source).Aggregate<char, string>(str, (Func<string, char, string>) ((current, badChar) => current.Replace(badChar.ToString(), "")));
  }
}
