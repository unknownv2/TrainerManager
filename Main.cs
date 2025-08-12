
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrainerManager.CheatEngine;


namespace TrainerManager;

public class Main : Form
{
  private static Main _instance;
  private readonly Config _config;
  private bool _hasDuplicateHotkeys;
  private IContainer components;
  private TextBox txtModules;
  private Label lblModules;
  private ToolTip toolTip;
  private TextBox txtLocations;
  private Label lblLocations;
  private Label lblLaunchArgs;
  private TextBox txtLaunchArgs;
  private CheckBox ckX64;
  private FlowLayoutPanel panelCheats;
  private Button btnSave;
  private Button btnClose;
  private ContextMenuStrip contextMenuStrip;
  private ToolStripMenuItem deleteCheatToolStripMenuItem;
  private ListView listHotkeys;
  private ColumnHeader colHotkey;
  private ColumnHeader colName;
  private Button btnImportCheatTable;
  private Button btnPull;
  private ComboBox cbGames;
  private Label lblTitle;
  private Panel panelGame;

  public Main(Config config)
  {
    this._config = config;
    Main._instance = this;
    this.InitializeComponent();
    this.RefreshGames();
  }

  internal static void RefreshHotkeyOverview() => Main._instance?.RefreshHotkeyOverviewInstance();

  private void RefreshHotkeyOverviewInstance()
  {
    Game selectedGame = this.GetSelectedGame();
    if (selectedGame == null)
      return;
    this.listHotkeys.BeginUpdate();
    this.listHotkeys.Items.Clear();
    List<string> stringList = new List<string>();
    HashSet<string> stringSet = new HashSet<string>();
    foreach (Cheat cheat in selectedGame.Cheats)
    {
      foreach (CheatInput input in cheat.Inputs)
      {
        int hotkeySize = Main.GetHotkeySize(input);
        for (int index = 0; index < hotkeySize; ++index)
        {
          if (input.Hotkeys[index] != null)
          {
            string text = KeyConvert.ToString(input.Hotkeys[index].Keys);
            ListViewItem listViewItem = new ListViewItem(text);
            if (!stringSet.Add(text))
              stringList.Add(text);
            listViewItem.SubItems.Add(input.Name ?? cheat.Name);
            this.listHotkeys.Items.Add(listViewItem);
          }
        }
      }
    }
    this._hasDuplicateHotkeys = stringList.Count != 0;
    foreach (string str in stringList)
    {
      string hotkey = str;
      foreach (ListViewItem listViewItem in this.listHotkeys.Items.Cast<ListViewItem>().Where<ListViewItem>((Func<ListViewItem, bool>) (i => i.Text == hotkey)))
        listViewItem.ForeColor = Color.Red;
    }
    this.listHotkeys.Sort();
    this.listHotkeys.EndUpdate();
  }

  private static int GetHotkeySize(CheatInput input)
  {
    switch (input.Type)
    {
      case "toggle":
      case "command":
        return 1;
      case "numeric":
      case "selection":
        return 2;
      default:
        return 0;
    }
  }

  private Game GetSelectedGame()
  {
    return this.cbGames.SelectedItem is ComboBoxItem selectedItem ? (Game) selectedItem.Value : (Game) (object) null;
  }

  private void RefreshGames()
  {
    this.cbGames.BeginUpdate();
    this.cbGames.Items.Clear();
    foreach (Game game in this._config.Repos.SelectMany<string, Game>(new Func<string, IEnumerable<Game>>(Main.EnumerateGamesInRepo)))
      this.cbGames.Items.Add((object) new ComboBoxItem(game.Title, (object) game));
    this.cbGames.Sorted = true;
    this.cbGames.EndUpdate();
    this.EnableView(false);
  }

  private static IEnumerable<Game> EnumerateGamesInRepo(string repoDir)
  {
    if (Directory.Exists(Path.Combine(repoDir, "Games")))
    {
      foreach (string enumerateDirectory in Directory.EnumerateDirectories(Path.Combine(repoDir, "Games")))
      {
        string path = Path.Combine(enumerateDirectory, "Trainer.json");
        if (File.Exists(path))
        {
          Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(path, Encoding.UTF8));
          game.Location = enumerateDirectory;
          yield return game;
        }
      }
    }
  }

  private void LoadGameIntoView(Game game)
  {
    this.ckX64.Checked = game.X64;
    this.txtModules.Lines = game.Modules.ToArray();
    this.txtLaunchArgs.Text = game.LaunchArgs;
    this.txtLocations.Lines = game.Directories.Concat<string>((IEnumerable<string>) game.RegistryLocations).ToArray<string>();
    this.panelCheats.Controls.Clear();
    this.panelCheats.Controls.AddRange(game.Cheats.Select<Cheat, CheatEditor>(new Func<Cheat, CheatEditor>(this.MakeCheatEditor)).Cast<Control>().ToArray<Control>());
    Main.RefreshHotkeyOverview();
    this.EnableView(true);
  }

  private CheatEditor MakeCheatEditor(Cheat cheat)
  {
    CheatEditor cheatEditor = new CheatEditor(cheat);
    cheatEditor.ContextMenuStrip = this.contextMenuStrip;
    cheatEditor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    return cheatEditor;
  }

  private void EnableView(bool enable)
  {
    this.panelGame.Enabled = enable;
    this.panelCheats.Enabled = enable;
    this.btnImportCheatTable.Enabled = enable;
    this.btnSave.Enabled = enable;
  }

  private void btnClose_Click(object sender, EventArgs e) => this.Close();

  private bool Save()
  {
    if (this.GetSelectedGame() == null)
      return true;
    try
    {
      Game selectedGame = this.GetSelectedGame();
      selectedGame.X64 = this.ckX64.Checked;
      selectedGame.Modules = ((IEnumerable<string>) this.txtModules.Lines).Select<string, string>((Func<string, string>) (m => m.Trim())).Where<string>((Func<string, bool>) (m => m.Length != 0)).ToList<string>();
      selectedGame.Directories = ((IEnumerable<string>) this.txtLocations.Lines).Select<string, string>((Func<string, string>) (t => t.Trim())).Where<string>((Func<string, bool>) (t => t.Length != 0 && !t.StartsWith("HKEY_"))).ToList<string>();
      selectedGame.RegistryLocations = ((IEnumerable<string>) this.txtLocations.Lines).Select<string, string>((Func<string, string>) (t => t.Trim())).Where<string>((Func<string, bool>) (t => t.Length != 0 && t.StartsWith("HKEY_"))).ToList<string>();
      selectedGame.LaunchArgs = this.txtLaunchArgs.Text.Trim().Length == 0 ? (string) null : this.txtLaunchArgs.Text.Trim();
      selectedGame.Cheats = new List<Cheat>(this.panelCheats.Controls.OfType<CheatEditor>().Select<CheatEditor, Cheat>((Func<CheatEditor, Cheat>) (p => p.Save())));
      if (selectedGame.Modules.Count == 0)
        throw new Exception("You must declare at least 1 module (exe/dll)!");
      if (this._hasDuplicateHotkeys)
        throw new Exception("Make sure all hotkeys are unique!");
      File.WriteAllText(Path.Combine(selectedGame.Location, "Trainer.json"), JsonConvert.SerializeObject((object) selectedGame, Formatting.Indented));
      return true;
    }
    catch (Exception ex)
    {
      int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      return false;
    }
  }

  private void btnSave_Click(object sender, EventArgs e) => this.Save();

  private void deleteCheatToolStripMenuItem_Click(object sender, EventArgs e)
  {
    if (!(this.contextMenuStrip.SourceControl is CheatEditor sourceControl) || MessageBox.Show($"Are you sure you want to delete the {sourceControl.CurrentName} cheat?", "Delete cheat?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) != DialogResult.Yes)
      return;
    this.GetSelectedGame().Cheats.Remove(sourceControl.Cheat);
    sourceControl.Parent.Controls.Remove((Control) sourceControl);
    Main.RefreshHotkeyOverview();
  }

  private void panelCheats_DoubleClick(object sender, EventArgs e)
  {
    Game selectedGame = this.GetSelectedGame();
    Cheat cheat = new Cheat()
    {
      Name = "Cheat " + (object) (selectedGame.Cheats.Count + 1),
      Inputs = new List<CheatInput>()
    };
    selectedGame.Cheats.Add(cheat);
    CheatEditor activeControl = this.MakeCheatEditor(cheat);
    this.panelCheats.Controls.Add((Control) activeControl);
    this.panelCheats.ScrollControlIntoView((Control) activeControl);
    Main.RefreshHotkeyOverview();
  }

  private void btnImportCheatTable_Click(object sender, EventArgs e)
  {
    OpenFileDialog openFileDialog1 = new OpenFileDialog();
    openFileDialog1.CheckFileExists = true;
    openFileDialog1.Multiselect = false;
    openFileDialog1.Filter = "Cheat Table|*.CT";
    openFileDialog1.Title = "Select a Cheat Table";
    OpenFileDialog openFileDialog2 = openFileDialog1;
    if (openFileDialog2.ShowDialog() != DialogResult.OK)
      return;
    CheatTable cheatTable;
    try
    {
      cheatTable = CheatTable.FromFile(openFileDialog2.FileName);
    }
    catch
    {
      int num = (int) MessageBox.Show("Invalid cheat table!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      return;
    }
    Game selectedGame = this.GetSelectedGame();
    List<Cheat> importedCheats = new List<Cheat>();
    List<string> stringList = new List<string>();
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
    foreach (CheatEntry entry in cheatTable.CheatEntries.Entries)
    {
      string description = entry.Description;
      char[] chArray = new char[2]{ ' ', '"' };
      entry.Description = description.Trim(chArray);
    }
    foreach (CheatEntry entry in cheatTable.CheatEntries.Entries)
    {
      CheatEntry cheatEntry = entry;
      string name = cheatEntry.Description;
      string lowerInvariant = name.ToLowerInvariant();
      if (lowerInvariant == "activate")
        stringList.Add(cheatEntry.AssemblerScript);
      else if (cheatEntry.VariableType == "Auto Assembler Script" && cheatEntry.HotKeys != null && cheatEntry.HotKeys.Entries.Count != 0)
      {
        if (cheatEntry.HotKeys.Entries.Count == 1 && cheatEntry.HotKeys.Entries[0].Action == "Toggle Activation")
        {
          string key = lowerInvariant.Replace(" ", "_");
          dictionary2.Add(key, cheatEntry.AssemblerScript);
          importedCheats.Add(new Cheat()
          {
            Name = name,
            Inputs = new List<CheatInput>()
            {
              new CheatInput()
              {
                Type = "toggle",
                Target = key,
                Hotkeys = new List<HotkeyInput>()
                {
                  new HotkeyInput()
                  {
                    Keys = cheatEntry.HotKeys.Entries[0].Keys.Entries.ToArray()
                  }
                },
                Args = new InputArgs()
              }
            }
          });
        }
        else if (cheatEntry.HotKeys.Entries.Count == 1 && cheatEntry.HotKeys.Entries[0].Action == "Activate")
        {
          CheatEntry cheatEntry1 = cheatTable.CheatEntries.Entries.FirstOrDefault<CheatEntry>((Func<CheatEntry, bool>) (c => c != cheatEntry && c.Description == name));
          if (cheatEntry1 != null && !(cheatEntry1.VariableType == "Auto Assembler Script") && cheatEntry1.HotKeys != null && cheatEntry1.HotKeys.Entries.Count == 1 && cheatEntry1.HotKeys.Entries[0].Action == "Set Value")
          {
            string key = lowerInvariant.Replace(" ", "_");
            importedCheats.Add(new Cheat()
            {
              Name = name,
              Inputs = new List<CheatInput>()
              {
                new CheatInput()
                {
                  Type = "command",
                  Target = key,
                  Hotkeys = new List<HotkeyInput>()
                  {
                    new HotkeyInput()
                    {
                      Keys = cheatEntry.HotKeys.Entries[0].Keys.Entries.ToArray()
                    }
                  }
                }
              }
            });
            dictionary1.Add(key, cheatEntry.AssemblerScript);
          }
        }
      }
    }
    CheatTableCheatMerger tableCheatMerger = new CheatTableCheatMerger(selectedGame.Cheats, importedCheats);
    if (tableCheatMerger.ShowDialog((IWin32Window) this) != DialogResult.OK)
      return;
    List<Cheat> cheats = tableCheatMerger.Cheats;
    CheatTableVersionSelector tableVersionSelector = new CheatTableVersionSelector(selectedGame);
    if (tableVersionSelector.ShowDialog((IWin32Window) this) != DialogResult.OK)
      return;
    GameVersion selectedVersion = tableVersionSelector.SelectedVersion;
    StringBuilder sb = new StringBuilder("#include \"TrainerLib.h\"");
    sb.AppendLine();
    sb.AppendLine();
    StringBuilder stringBuilder = new StringBuilder();
    if (selectedVersion.IsUnknown)
      stringBuilder.Append("SetupUnknownVersion()\r\n");
    else
      stringBuilder.AppendFormat("SetupVersion({0})\r\n", (object) selectedVersion.Timestamp);
    stringBuilder.AppendLine("{");
    for (int index = 0; index < stringList.Count; ++index)
    {
      this.AppendAaScript(sb, "activateScript" + (object) index, stringList[index]);
      stringBuilder.AppendFormat("\tAssembler << activateScript{0};\r\n", (object) index);
    }
    if (stringList.Count != 0)
      stringBuilder.AppendLine();
    foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
    {
      string name = $"execute{keyValuePair.Key.ToPascalCase()}Script";
      this.AppendAaScript(sb, name, keyValuePair.Value);
      stringBuilder.AppendFormat("\tHandlers[\"{0}\"] += {1};\r\n", (object) keyValuePair.Key, (object) name);
    }
    if (dictionary1.Count != 0)
      stringBuilder.AppendLine();
    foreach (KeyValuePair<string, string> keyValuePair in dictionary2)
    {
      string name = $"toggle{keyValuePair.Key.ToPascalCase()}Script";
      this.AppendAaScript(sb, name, keyValuePair.Value);
      stringBuilder.AppendFormat("\tHandlers[\"{0}\"] += {1};\r\n", (object) keyValuePair.Key, (object) name);
    }
    stringBuilder.AppendLine("}");
    sb.Append((object) stringBuilder);
    string path1 = Path.Combine(selectedGame.Location, selectedVersion.Filename);
    bool flag = File.Exists(path1);
    File.WriteAllText(path1, sb.ToString(), Encoding.UTF8);
    if (!flag)
    {
      string path2 = Path.Combine(selectedGame.Location, "Trainer.vcxproj");
      string str1 = File.ReadAllText(path2);
      int startIndex1 = str1.IndexOf("\r\n", str1.IndexOf("<ClCompile Include=", StringComparison.Ordinal), StringComparison.Ordinal);
      File.WriteAllText(path2, str1.Insert(startIndex1, $"\r\n    <ClCompile Include=\"{selectedVersion.Filename}\" />"), Encoding.UTF8);
      string path3 = Path.Combine(selectedGame.Location, "Trainer.vcxproj.filters");
      string str2 = File.ReadAllText(path3);
      int startIndex2 = str2.IndexOf("\r\n", str2.IndexOf("</ClCompile>", StringComparison.Ordinal), StringComparison.Ordinal);
      File.WriteAllText(path3, str2.Insert(startIndex2, $"\r\n    <ClCompile Include=\"{selectedVersion.Filename}\">\r\n      <Filter>Source Files</Filter>\r\n    </ClCompile>"), Encoding.UTF8);
    }
    selectedGame.Cheats = cheats;
    if (this.txtModules.Text.Trim().Length == 0 && cheatTable.CheatCodes != null)
    {
      selectedGame.Modules = new List<string>();
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string str in cheatTable.CheatCodes.Entries.Where<CheatCodeEntry>((Func<CheatCodeEntry, bool>) (c => c.ModuleName != null)).Select<CheatCodeEntry, string>((Func<CheatCodeEntry, string>) (c => c.ModuleName)))
      {
        string lowerInvariant = str.ToLowerInvariant();
        if (lowerInvariant.EndsWith(".exe") && stringSet.Add(lowerInvariant))
          selectedGame.Modules.Add(str);
      }
    }
    this.LoadGameIntoView(selectedGame);
  }

  private void AppendAaScript(StringBuilder sb, string name, string script)
  {
    sb.AppendFormat("auto {0} = R\"(\r\n", (object) name);
    sb.Append(Main.FixUpAaScript(script));
    sb.AppendLine();
    sb.AppendLine(")\";");
    sb.AppendLine();
  }

  private static string FixUpAaScript(string script)
  {
    List<string> values = new List<string>();
    bool flag = false;
    foreach (string str in ((IEnumerable<string>) script.Trim().Split(new string[1]
    {
      "\r\n"
    }, StringSplitOptions.None)).Select<string, string>((Func<string, string>) (line => line.Trim())))
    {
      if (flag)
        flag = str.Length == 0 || str[0] != '}';
      else if (str.Length != 0 && str[0] == '{')
        flag = true;
      else if (str.Length < 2 || !str.StartsWith("//"))
      {
        int length = str.IndexOf("//", StringComparison.Ordinal);
        values.Add("\t" + (length == -1 ? str : str.Substring(0, length)));
      }
    }
    while (values.Count > 0 && values[0].Length == 1)
      values.RemoveAt(0);
    while (values.Count > 0 && values[values.Count - 1].Length == 1)
      values.RemoveAt(values.Count - 1);
    return string.Join("\r\n", (IEnumerable<string>) values);
  }

  private void PullChanges()
  {
    foreach (string repo in this._config.Repos)
    {
      try
      {
        new GitRepo(repo).Pull();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }
  }

  private void btnPull_Click(object sender, EventArgs e)
  {
    if (this.GetSelectedGame() != null)
    {
      switch (MessageBox.Show("Do you want to save your current changes?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
      {
        case DialogResult.Yes:
          if (!this.Save())
            return;
          break;
        case DialogResult.No:
          break;
        default:
          return;
      }
    }
    this.PullChanges();
    this.RefreshGames();
  }

  private void cbGames_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.LoadGameIntoView(this.GetSelectedGame());
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Main));
    this.txtModules = new TextBox();
    this.ckX64 = new CheckBox();
    this.lblLaunchArgs = new Label();
    this.txtLaunchArgs = new TextBox();
    this.txtLocations = new TextBox();
    this.lblLocations = new Label();
    this.lblModules = new Label();
    this.toolTip = new ToolTip(this.components);
    this.listHotkeys = new ListView();
    this.colHotkey = new ColumnHeader();
    this.colName = new ColumnHeader();
    this.panelCheats = new FlowLayoutPanel();
    this.btnSave = new Button();
    this.btnClose = new Button();
    this.contextMenuStrip = new ContextMenuStrip(this.components);
    this.deleteCheatToolStripMenuItem = new ToolStripMenuItem();
    this.btnImportCheatTable = new Button();
    this.btnPull = new Button();
    this.cbGames = new ComboBox();
    this.lblTitle = new Label();
    this.panelGame = new Panel();
    this.contextMenuStrip.SuspendLayout();
    this.panelGame.SuspendLayout();
    this.SuspendLayout();
    this.txtModules.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.txtModules.Location = new Point(18, 331);
    this.txtModules.Margin = new Padding(9, 9, 3, 9);
    this.txtModules.Multiline = true;
    this.txtModules.Name = "txtModules";
    this.txtModules.ScrollBars = ScrollBars.Vertical;
    this.txtModules.Size = new Size(243, 62);
    this.txtModules.TabIndex = 1;
    this.txtModules.WordWrap = false;
    this.ckX64.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.ckX64.AutoSize = true;
    this.ckX64.Location = new Point(214, 525);
    this.ckX64.Margin = new Padding(9, 3, 9, 3);
    this.ckX64.Name = "ckX64";
    this.ckX64.Size = new Size(54, 21);
    this.ckX64.TabIndex = 3;
    this.ckX64.Text = "x64";
    this.ckX64.UseVisualStyleBackColor = true;
    this.lblLaunchArgs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
    this.lblLaunchArgs.AutoSize = true;
    this.lblLaunchArgs.Location = new Point(15, 498);
    this.lblLaunchArgs.Margin = new Padding(9, 0, 3, 0);
    this.lblLaunchArgs.Name = "lblLaunchArgs";
    this.lblLaunchArgs.Size = new Size(144 /*0x90*/, 17);
    this.lblLaunchArgs.TabIndex = 10;
    this.lblLaunchArgs.Text = "Launch Arguments:";
    this.toolTip.SetToolTip((Control) this.lblLaunchArgs, "Optional command-line arguments that will be passed to the game upon launch.");
    this.txtLaunchArgs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.txtLaunchArgs.Location = new Point(18, 524);
    this.txtLaunchArgs.Margin = new Padding(9);
    this.txtLaunchArgs.Name = "txtLaunchArgs";
    this.txtLaunchArgs.Size = new Size(180, 23);
    this.txtLaunchArgs.TabIndex = 2;
    this.txtLocations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.txtLocations.Location = new Point(18, 427);
    this.txtLocations.Margin = new Padding(9);
    this.txtLocations.Multiline = true;
    this.txtLocations.Name = "txtLocations";
    this.txtLocations.ScrollBars = ScrollBars.Vertical;
    this.txtLocations.Size = new Size(243, 62);
    this.txtLocations.TabIndex = 4;
    this.lblLocations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
    this.lblLocations.AutoSize = true;
    this.lblLocations.Location = new Point(15, 402);
    this.lblLocations.Margin = new Padding(9, 0, 3, 0);
    this.lblLocations.Name = "lblLocations";
    this.lblLocations.Size = new Size(88, 17);
    this.lblLocations.TabIndex = 5;
    this.lblLocations.Text = "Locations:";
    this.toolTip.SetToolTip((Control) this.lblLocations, componentResourceManager.GetString("lblLocations.ToolTip"));
    this.lblModules.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
    this.lblModules.AutoSize = true;
    this.lblModules.Location = new Point(18, 305);
    this.lblModules.Margin = new Padding(9, 7, 3, 0);
    this.lblModules.Name = "lblModules";
    this.lblModules.Size = new Size(72, 17);
    this.lblModules.TabIndex = 4;
    this.lblModules.Text = "Modules:";
    this.toolTip.SetToolTip((Control) this.lblModules, componentResourceManager.GetString("lblModules.ToolTip"));
    this.toolTip.ToolTipIcon = ToolTipIcon.Info;
    this.toolTip.ToolTipTitle = "Info";
    this.listHotkeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.listHotkeys.BorderStyle = BorderStyle.FixedSingle;
    this.listHotkeys.Columns.AddRange(new ColumnHeader[2]
    {
      this.colHotkey,
      this.colName
    });
    this.listHotkeys.FullRowSelect = true;
    this.listHotkeys.GridLines = true;
    this.listHotkeys.HeaderStyle = ColumnHeaderStyle.Nonclickable;
    this.listHotkeys.Location = new Point(-1, -1);
    this.listHotkeys.Margin = new Padding(0, 0, 0, 9);
    this.listHotkeys.MultiSelect = false;
    this.listHotkeys.Name = "listHotkeys";
    this.listHotkeys.Size = new Size(280, 293);
    this.listHotkeys.Sorting = SortOrder.Ascending;
    this.listHotkeys.TabIndex = 2;
    this.listHotkeys.UseCompatibleStateImageBehavior = false;
    this.listHotkeys.View = View.Details;
    this.colHotkey.Text = "Hotkey";
    this.colHotkey.Width = 101;
    this.colName.Text = "Cheat";
    this.colName.Width = 150;
    this.panelCheats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.panelCheats.AutoScroll = true;
    this.panelCheats.BorderStyle = BorderStyle.FixedSingle;
    this.panelCheats.Location = new Point(289, 62);
    this.panelCheats.Margin = new Padding(9);
    this.panelCheats.Name = "panelCheats";
    this.panelCheats.Size = new Size(644, 565);
    this.panelCheats.TabIndex = 0;
    this.panelCheats.DoubleClick += new EventHandler(this.panelCheats_DoubleClick);
    this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.btnSave.Location = new Point(650, 639);
    this.btnSave.Name = "btnSave";
    this.btnSave.Size = new Size(137, 33);
    this.btnSave.TabIndex = 4;
    this.btnSave.Text = "&Save";
    this.btnSave.UseVisualStyleBackColor = true;
    this.btnSave.Click += new EventHandler(this.btnSave_Click);
    this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.btnClose.Location = new Point(793, 639);
    this.btnClose.Name = "btnClose";
    this.btnClose.Size = new Size(129, 33);
    this.btnClose.TabIndex = 5;
    this.btnClose.Text = "Close";
    this.btnClose.UseVisualStyleBackColor = true;
    this.btnClose.Click += new EventHandler(this.btnClose_Click);
    this.contextMenuStrip.ImageScalingSize = new Size(20, 20);
    this.contextMenuStrip.Items.AddRange(new ToolStripItem[1]
    {
      (ToolStripItem) this.deleteCheatToolStripMenuItem
    });
    this.contextMenuStrip.Name = "contextMenuStrip";
    this.contextMenuStrip.Size = new Size(171, 30);
    this.deleteCheatToolStripMenuItem.AccessibleRole = AccessibleRole.ButtonDropDownGrid;
    this.deleteCheatToolStripMenuItem.Name = "deleteCheatToolStripMenuItem";
    this.deleteCheatToolStripMenuItem.Size = new Size(170, 26);
    this.deleteCheatToolStripMenuItem.Text = "Delete Cheat";
    this.deleteCheatToolStripMenuItem.Click += new EventHandler(this.deleteCheatToolStripMenuItem_Click);
    this.btnImportCheatTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
    this.btnImportCheatTable.Location = new Point(10, 639);
    this.btnImportCheatTable.Name = "btnImportCheatTable";
    this.btnImportCheatTable.Size = new Size(280, 33);
    this.btnImportCheatTable.TabIndex = 6;
    this.btnImportCheatTable.Text = "Import Cheat Table...";
    this.btnImportCheatTable.UseVisualStyleBackColor = true;
    this.btnImportCheatTable.Click += new EventHandler(this.btnImportCheatTable_Click);
    this.btnPull.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.btnPull.Location = new Point(750, 11);
    this.btnPull.Name = "btnPull";
    this.btnPull.Size = new Size(172, 38);
    this.btnPull.TabIndex = 7;
    this.btnPull.Text = "Pull Changes";
    this.btnPull.UseVisualStyleBackColor = true;
    this.btnPull.Click += new EventHandler(this.btnPull_Click);
    this.cbGames.DropDownStyle = ComboBoxStyle.DropDownList;
    this.cbGames.DropDownWidth = 350;
    this.cbGames.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.cbGames.FormattingEnabled = true;
    this.cbGames.Location = new Point(10, 18);
    this.cbGames.MaxDropDownItems = 20;
    this.cbGames.Name = "cbGames";
    this.cbGames.Size = new Size(280, 26);
    this.cbGames.TabIndex = 8;
    this.cbGames.SelectedIndexChanged += new EventHandler(this.cbGames_SelectedIndexChanged);
    this.lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    this.lblTitle.Font = new Font("Consolas", 10.2f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.lblTitle.Location = new Point(10, 8);
    this.lblTitle.Name = "lblTitle";
    this.lblTitle.Size = new Size(911, 44);
    this.lblTitle.TabIndex = 12;
    this.lblTitle.Text = "Infinity Trainer Manager";
    this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
    this.panelGame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
    this.panelGame.BorderStyle = BorderStyle.FixedSingle;
    this.panelGame.Controls.Add((Control) this.txtModules);
    this.panelGame.Controls.Add((Control) this.ckX64);
    this.panelGame.Controls.Add((Control) this.lblModules);
    this.panelGame.Controls.Add((Control) this.listHotkeys);
    this.panelGame.Controls.Add((Control) this.txtLaunchArgs);
    this.panelGame.Controls.Add((Control) this.txtLocations);
    this.panelGame.Controls.Add((Control) this.lblLocations);
    this.panelGame.Controls.Add((Control) this.lblLaunchArgs);
    this.panelGame.Location = new Point(10, 62);
    this.panelGame.Margin = new Padding(9);
    this.panelGame.Name = "panelGame";
    this.panelGame.Padding = new Padding(9);
    this.panelGame.Size = new Size(280, 565);
    this.panelGame.TabIndex = 11;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(932, 683);
    this.Controls.Add((Control) this.panelGame);
    this.Controls.Add((Control) this.panelCheats);
    this.Controls.Add((Control) this.cbGames);
    this.Controls.Add((Control) this.btnPull);
    this.Controls.Add((Control) this.btnClose);
    this.Controls.Add((Control) this.btnSave);
    this.Controls.Add((Control) this.btnImportCheatTable);
    this.Controls.Add((Control) this.lblTitle);
    this.Font = new Font("Consolas", 7.8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.MinimumSize = new Size(850, 500);
    this.Name = nameof (Main);
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Trainer Manager";
    this.contextMenuStrip.ResumeLayout(false);
    this.panelGame.ResumeLayout(false);
    this.panelGame.PerformLayout();
    this.ResumeLayout(false);
  }
}
