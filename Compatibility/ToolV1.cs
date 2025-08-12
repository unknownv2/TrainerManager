

using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace TrainerManager.Compatibility;

public class ToolV1
{
  private static Dictionary<string, string> InputNameLookup = new Dictionary<string, string>()
  {
    {
      "Set Morning Time",
      "Morning"
    },
    {
      "Set Noon Time",
      "Noon"
    },
    {
      "Set Afternoon Time",
      "Afternoon"
    },
    {
      "Set Evening Time",
      "Evening"
    },
    {
      "Set Night Time",
      "Night"
    },
    {
      "+12 Hours",
      "+12 Hours"
    },
    {
      "-12 Hours",
      "-12 Hours"
    }
  };
  private static Dictionary<string, string> CheatNameLookup = new Dictionary<string, string>()
  {
    {
      "Set Morning Time",
      "Set Time"
    },
    {
      "+12 Hours",
      "Time"
    }
  };

  [JsonIgnore]
  public GameV1 Game { get; set; }

  [JsonIgnore]
  public string Location { get; set; }

  public string Guid { get; set; }

  public string Name { get; set; }

  public string Description { get; set; }

  public int AccessLevel { get; set; }

  public string Notes { get; set; }

  public bool NotesRequireRead { get; set; }

  public string Version { get; set; }

  public List<string> Changes { get; set; }

  public List<string> Features { get; set; }

  public List<string> SupportedGameVersions { get; set; }

  public TrainerV1 Trainer { get; set; }

  public GameV2 ConvertTrainer(string outDir)
  {
    this.DeleteDir("Debug");
    this.DeleteDir("Debug-Obj");
    this.DeleteDir("Release");
    this.DeleteDir("Release-Obj");
    this.DeleteDir(".vs");
    foreach (string path in ((IEnumerable<string>) Directory.GetFiles(this.Location, "*.vc.db")).Concat<string>((IEnumerable<string>) Directory.GetFiles(this.Location, "*.sdf")))
      File.Delete(path);
    Directory.CreateDirectory(outDir);
    FileSystem.CopyDirectory(this.Location, outDir, true);
    string path1 = Path.Combine(outDir, "Tool.vcxproj");
    string contents = File.ReadAllText(path1).Replace("..\\..\\TrainerLib\\x64\\TrainerLib.lib;", "").Replace("..\\..\\TrainerLib\\x86\\TrainerLib.lib;", "").Replace("<AdditionalIncludeDirectories>..\\..\\TrainerLib", "<AdditionalIncludeDirectories>..\\..\\Include").Replace("\"$(ProjectDir)..\\..\\Utilities\\TrainerUnloader.exe\" free \"$(TargetPath)\"", "").Replace("<OutDir>$(Configuration)\\</OutDir>", "<OutDir>Build\\</OutDir>").Replace("<TargetName>Tool</TargetName>", "<TargetName>Trainer</TargetName>").Replace("$(Configuration)-Obj\\", "Temp\\");
    File.Delete(path1);
    File.WriteAllText(Path.Combine(outDir, "Trainer.vcxproj"), contents, Encoding.UTF8);
    string str1 = Path.Combine(outDir, "Tool.vcxproj.filters");
    if (File.Exists(str1))
      File.Move(str1, Path.Combine(outDir, "Trainer.vcxproj.filters"));
    string str2 = Path.Combine(outDir, "Tool.vcxproj.user");
    if (File.Exists(str2))
      File.Move(str2, Path.Combine(outDir, "Trainer.vcxproj.user"));
    foreach (string path2 in ((IEnumerable<string>) Directory.GetFiles(outDir, "*.cpp", System.IO.SearchOption.AllDirectories)).Concat<string>((IEnumerable<string>) Directory.GetFiles(outDir, "*.h", System.IO.SearchOption.AllDirectories)).Concat<string>((IEnumerable<string>) Directory.GetFiles(outDir, "*.cc", System.IO.SearchOption.AllDirectories)))
    {
      StringBuilder stringBuilder = new StringBuilder(File.ReadAllText(path2));
      stringBuilder.Replace("#include \"MinHook.h\"\r\n", "");
      stringBuilder.Replace("#include <MinHook.h>\r\n", "");
      stringBuilder.Replace("#include \"CheatEngine.h\"\r\n", "");
      stringBuilder.Replace("#include <CheatEngine.h>\r\n", "");
      stringBuilder.Replace("#include \"GenericFunctionTypes.h\"\r\n", "");
      stringBuilder.Replace("#include <GenericFunctionTypes.h>\r\n", "");
      stringBuilder.Replace("#include \"TrainerLib.h\"\r\n", "#define TRAINERLIB_V1\r\n#include \"TrainerLib.h\"\r\n");
      stringBuilder.Replace("#include <TrainerLib.h>\r\n", "#define TRAINERLIB_V1\r\n#include \"TrainerLib.h\"\r\n");
      foreach (KeyValuePair<string, Dictionary<string, uint>> keyValuePair in this.Game.KnownVersions.Where<KeyValuePair<string, Dictionary<string, uint>>>((Func<KeyValuePair<string, Dictionary<string, uint>>, bool>) (version => version.Value.Values.Count != 0)))
        stringBuilder.Replace($"\"{keyValuePair.Key}\"", $"\"{keyValuePair.Value.Values.First<uint>()}\" /* {keyValuePair.Key} */");
      string str3 = stringBuilder.ToString();
      foreach (KeyValuePair<string, ModV1> mod in this.Trainer.Mods)
      {
        string str4;
        switch (mod.Value.Type)
        {
          case 10:
            str4 = "_f";
            break;
          case 11:
            str4 = "_d";
            break;
          default:
            continue;
        }
        str3 = Regex.Replace(str3, $"([\\[(),]\\s*){mod.Key}(\\s*[\\],)])", $"$1{mod.Key}{str4}$2");
      }
      File.WriteAllText(path2, str3, Encoding.UTF8);
    }
    foreach (string file in Directory.GetFiles(outDir, "*.sln"))
      File.WriteAllText(file, File.ReadAllText(file).Replace("Tool.vcxproj", "Trainer.vcxproj"));
    GameV2 gameV2 = this.ConvertTrainer();
    File.Delete(Path.Combine(outDir, "Tool.json"));
    File.WriteAllText(Path.Combine(outDir, "Trainer.json"), JsonConvert.SerializeObject((object) gameV2.LocalGame, Formatting.Indented));
    return gameV2;
  }

  private void DeleteDir(string rel)
  {
    string path = Path.Combine(this.Location, rel);
    if (!Directory.Exists(path))
      return;
    Directory.Delete(path, true);
  }

  public GameV2 ConvertTrainer()
  {
    RemoteGame remoteGame = new RemoteGame();
    remoteGame.Title = this.Game.Title;
    remoteGame.SteamId = this.Game.SteamId;
    remoteGame.GogId = new int?();
    remoteGame.WindowsPfn = string.IsNullOrWhiteSpace(this.Game.WindowsPackageFamily) ? (string) null : this.Game.WindowsPackageFamily;
    remoteGame.ReleaseDate = (string) null;
    remoteGame.ThumbnailUrl = (string) null;
    TrainerManager.Game game1 = new TrainerManager.Game();
    game1.Location = this.Game.Location;
    game1.X64 = File.ReadAllText(Path.Combine(this.Location, "Tool.vcxproj")).Contains("Release|x64");
    game1.LaunchArgs = string.IsNullOrWhiteSpace(this.Trainer.ExecutionArgs) ? (string) null : this.Trainer.ExecutionArgs;
    game1.Modules = new List<string>()
    {
      this.Trainer.ExePath
    };
    if (!string.IsNullOrWhiteSpace(this.Trainer.InjectionTarget) && !string.Equals(this.Trainer.InjectionTarget, this.Trainer.ExePath, StringComparison.InvariantCultureIgnoreCase))
      game1.Modules.Add(this.Trainer.InjectionTarget);
    game1.Directories = this.Game.PossibleLocations ?? new List<string>();
    TrainerManager.Game game2 = game1;
    List<string> locationsRegistry = this.Game.PossibleLocationsRegistry;
    List<string> stringList = (locationsRegistry != null ? locationsRegistry.Select<string, string>((Func<string, string>) (r =>
    {
      int startIndex = r.LastIndexOf(':');
      return r.Remove(startIndex, 1).Insert(startIndex, "\\");
    })).ToList<string>() : (List<string>) null) ?? new List<string>();
    game2.RegistryLocations = stringList;
    bool flag = false;
    game1.Cheats = new List<Cheat>();
    Dictionary<string, List<CheatInput>> dictionary1 = new Dictionary<string, List<CheatInput>>(this.Trainer.Mods.Count);
    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
    foreach (RemoteInputV1 remoteInputV1 in this.Trainer.RemoteGroups.SelectMany<RemoteGroupV1, RemoteInputV1>((Func<RemoteGroupV1, IEnumerable<RemoteInputV1>>) (g => (IEnumerable<RemoteInputV1>) g.Inputs)))
    {
      CheatInput cheatInput = new CheatInput()
      {
        Target = remoteInputV1.ModId,
        Type = ToolV1.ConvertRemoteInputType(remoteInputV1.Type),
        Hotkeys = new List<HotkeyInput>(),
        Args = new InputArgs()
      };
      cheatInput.Args.Options = remoteInputV1.Options;
      cheatInput.Args.Step = remoteInputV1.Step;
      ModV1 mod = this.Trainer.Mods[remoteInputV1.ModId];
      InputArgs args1 = cheatInput.Args;
      long? nullable1 = mod.Minimum;
      double? nullable2 = nullable1.HasValue ? new double?((double) nullable1.GetValueOrDefault()) : new double?();
      args1.Min = nullable2;
      InputArgs args2 = cheatInput.Args;
      nullable1 = mod.Maximum;
      double? nullable3 = nullable1.HasValue ? new double?((double) nullable1.GetValueOrDefault()) : new double?();
      args2.Max = nullable3;
      if (cheatInput.Type == "numeric")
        cheatInput.Args.PresentAsSlider = new bool?(remoteInputV1.Type == 4);
      if (cheatInput.Type == "numeric" || cheatInput.Type == "selection")
        cheatInput.Args.Defer = new bool?(false);
      List<CheatInput> cheatInputList = dictionary1.ContainsKey(remoteInputV1.ModId) ? dictionary1[remoteInputV1.ModId] : new List<CheatInput>();
      cheatInputList.Add(cheatInput);
      dictionary1[remoteInputV1.ModId] = cheatInputList;
      dictionary2[remoteInputV1.ModId] = remoteInputV1.DisplayName;
      dictionary3[remoteInputV1.ModId] = remoteInputV1.Description;
    }
    foreach (KeyboardInputV1 keyboardInput in this.Trainer.KeyboardInputs)
    {
      if (keyboardInput.Transformations.Count > 1)
      {
        flag = true;
      }
      else
      {
        TransformationV1 transformation = keyboardInput.Transformations[0];
        int[] array = ((IEnumerable<int>) new int[4]
        {
          keyboardInput.DefaultKey & (int) byte.MaxValue,
          keyboardInput.DefaultKey >>> 8 & (int) byte.MaxValue,
          keyboardInput.DefaultKey >>> 16 /*0x10*/ & (int) byte.MaxValue,
          keyboardInput.DefaultKey >>> 24 & (int) byte.MaxValue
        }).Where<int>((Func<int, bool>) (k => k != 0)).ToArray<int>();
        string hotkeyName = ToolV1.InputNameLookup.ContainsKey(keyboardInput.DisplayName) ? ToolV1.InputNameLookup[keyboardInput.DisplayName] : (string) null;
        List<CheatInput> cheatInputList;
        if (!dictionary1.TryGetValue(transformation.ModId, out cheatInputList))
        {
          CheatInput input = new CheatInput();
          input.Type = new string[5]
          {
            "command",
            "command",
            "numeric",
            "numeric",
            "toggle"
          }[transformation.Type];
          input.Target = transformation.ModId;
          input.Hotkeys = new List<HotkeyInput>();
          input.Args = new InputArgs();
          ToolV1.FillInput(input, transformation, keyboardInput, array, hotkeyName);
          dictionary2[transformation.ModId] = keyboardInput.DisplayName.Replace("Toggle ", "");
          dictionary1[transformation.ModId] = new List<CheatInput>()
          {
            input
          };
        }
        else
        {
          foreach (CheatInput input in cheatInputList)
            ToolV1.FillInput(input, transformation, keyboardInput, array, hotkeyName);
        }
      }
    }
    foreach (KeyValuePair<string, List<CheatInput>> keyValuePair in dictionary1)
    {
      Cheat cheat = new Cheat();
      cheat.Uuid = RandomString.Make();
      cheat.Inputs = keyValuePair.Value;
      cheat.Name = dictionary2.ContainsKey(keyValuePair.Key) ? dictionary2[keyValuePair.Key] : Regex.Replace(keyValuePair.Key.Replace("_", " "), "(^\\w)|(\\s\\w)", (MatchEvaluator) (m => m.Value.ToUpper()));
      if (ToolV1.CheatNameLookup.ContainsKey(cheat.Name))
        cheat.Name = ToolV1.CheatNameLookup[cheat.Name];
      cheat.Description = dictionary3.ContainsKey(keyValuePair.Key) ? dictionary3[keyValuePair.Key] : (string) null;
      if (string.IsNullOrWhiteSpace(cheat.Description))
        cheat.Description = (string) null;
      foreach (CheatInput input in cheat.Inputs)
      {
        for (int index = 0; index < input.Hotkeys.Count; ++index)
        {
          if (input.Hotkeys[index] != null)
          {
            if (input.Hotkeys[index].Keys == null)
              input.Hotkeys[index] = (HotkeyInput) null;
            else if (input.Hotkeys[index].Name != null)
            {
              input.Hotkeys[index].Name = input.Hotkeys[index].Name.Replace(" " + cheat.Name, "");
              if (input.Type == "toggle" && input.Hotkeys[index].Name == "Toggle")
                input.Hotkeys[index].Name = (string) null;
              else if (input.Type == "numeric")
              {
                if (index == 0 && input.Hotkeys[index].Name == "Increase")
                  input.Hotkeys[index].Name = (string) null;
                else if (index == 1 && input.Hotkeys[index].Name == "Decrease")
                  input.Hotkeys[index].Name = (string) null;
              }
            }
          }
        }
        int num = input.Type == "toggle" || input.Type == "command" ? 1 : 2;
        while (input.Hotkeys.Count > num)
          input.Hotkeys.RemoveAt(input.Hotkeys.Count - 1);
        while (input.Hotkeys.Count < num)
          input.Hotkeys.Add((HotkeyInput) null);
      }
      game1.Cheats.Add(cheat);
    }
    return new GameV2()
    {
      LocalGame = game1,
      RemoteGame = remoteGame,
      Incomplete = flag
    };
  }

  private static void FillInput(
    CheatInput input,
    TransformationV1 transformation,
    KeyboardInputV1 inputV1,
    int[] hotkey,
    string hotkeyName)
  {
    if (input.Type == "toggle" && transformation.Type == 4)
      input.Hotkeys.Add(new HotkeyInput()
      {
        Name = hotkeyName,
        Keys = hotkey
      });
    else if (input.Type == "command" && transformation.Type == 0)
      input.Hotkeys.Add(new HotkeyInput()
      {
        Name = hotkeyName,
        Keys = hotkey
      });
    else if (input.Type == "command" && transformation.Type == 1)
    {
      input.Args.Value = new double?(double.Parse(transformation.Arg));
      if (hotkeyName == null)
      {
        string lowerInvariant = inputV1.DisplayName.ToLowerInvariant();
        double? nullable;
        if (lowerInvariant.Contains("reset ") || lowerInvariant.Contains("default "))
          hotkeyName = "Reset";
        else if (lowerInvariant.Contains("minimum ") || lowerInvariant.Contains("min "))
          hotkeyName = "Minimum";
        else if (lowerInvariant.Contains("maximum ") || lowerInvariant.Contains("max ") || lowerInvariant.Contains("super "))
        {
          hotkeyName = "Maximum";
        }
        else
        {
          nullable = input.Args.Value;
          double num = 0.0;
          if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
            hotkeyName = "Zero";
        }
        nullable = input.Args.Value;
        string source = nullable.ToString();
        if (source.All<char>((Func<char, bool>) (v => v == '9')) || source[0] == '1' && source.Substring(1).All<char>((Func<char, bool>) (v => v == '0')))
          hotkeyName = "Maximum";
      }
      input.Hotkeys.Add(new HotkeyInput()
      {
        Name = hotkeyName,
        Keys = hotkey
      });
    }
    else
    {
      if (!(input.Type == "numeric"))
        return;
      if (input.Hotkeys.Count == 0)
        input.Hotkeys = new List<HotkeyInput>()
        {
          new HotkeyInput(),
          new HotkeyInput()
        };
      if (transformation.Type == 2)
      {
        input.Hotkeys[0].Name = inputV1.DisplayName;
        input.Hotkeys[0].Keys = hotkey;
      }
      else
      {
        if (transformation.Type != 3)
          return;
        input.Hotkeys[1].Name = inputV1.DisplayName;
        input.Hotkeys[1].Keys = hotkey;
      }
    }
  }

  private static string ConvertRemoteInputType(int type)
  {
    return new string[6]
    {
      "command",
      "toggle",
      "selection",
      null,
      "numeric",
      "numeric"
    }[type];
  }
}
