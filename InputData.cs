
using System;
using System.ComponentModel;
using System.Linq;


namespace TrainerManager;

internal abstract class InputData
{
  private readonly InputEditor _editor;

  internal CheatInput Input { get; }

  protected InputData(InputEditor editor, int hotkeyCount)
  {
    this._editor = editor;
    this.Input = editor.Input;
    while (this.Input.Hotkeys.Count < hotkeyCount)
      this.Input.Hotkeys.Add((HotkeyInput) null);
  }

  [DisplayName("Input Type")]
  [Description("The type of this input.")]
  public abstract InputType Type { get; set; }

  [DisplayName("Target")]
  [Description("The name of the variable this input manipulates.")]
  public abstract string Target { get; set; }

  [DisplayName("Alt Name")]
  [Description("An alternate name for the input in the remote.")]
  public abstract string Name { get; set; }

  protected void SetTarget(string value)
  {
    value = value?.Trim();
    if (string.IsNullOrWhiteSpace(value))
      throw new ArgumentException("Invalid target name.");
    this.Input.Target = !value.Any<char>((Func<char, bool>) (c => (c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9') && c != '_' && c != '-')) ? value : throw new ArgumentException("Target must be alphanumeric including - and _.");
  }

  protected void SetName(string value)
  {
    value = value?.Trim();
    this.Input.Name = string.IsNullOrWhiteSpace(value) ? (string) null : value;
  }

  protected void TypeChanged(InputType type)
  {
    this._editor.LoadType(type.ToString().ToLower());
    Main.RefreshHotkeyOverview();
  }
}
