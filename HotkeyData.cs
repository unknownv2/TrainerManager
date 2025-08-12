
using System.ComponentModel;


namespace TrainerManager;

internal class HotkeyData
{
  private readonly CheatInput _input;
  private readonly int _index;
  private string _lastName;

  public HotkeyData(CheatInput input, int index)
  {
    this._input = input;
    this._index = index;
    this._lastName = input.Hotkeys[index]?.Name;
  }

  [DisplayName("Name")]
  [Description("An optional custom display name for the hotkey.")]
  public string Name
  {
    get => this._lastName;
    set
    {
      value = value?.Trim();
      this._lastName = string.IsNullOrEmpty(value) ? (string) null : value;
      if (this._input.Hotkeys[this._index] == null)
        return;
      this._input.Hotkeys[this._index].Name = this._lastName;
    }
  }

  [DisplayName("Keys")]
  [Description("The hotkey keys.")]
  public string Keys
  {
    get
    {
      return this._input.Hotkeys[this._index] != null ? KeyConvert.ToString(this._input.Hotkeys[this._index].Keys) : (string) null;
    }
    set
    {
    }
  }

  public void SetHotkey(int[] hotkey)
  {
    if (hotkey == null)
      this._input.Hotkeys[this._index] = (HotkeyInput) null;
    else if (this._input.Hotkeys[this._index] == null)
      this._input.Hotkeys[this._index] = new HotkeyInput()
      {
        Name = this._lastName,
        Keys = hotkey
      };
    else
      this._input.Hotkeys[this._index].Keys = hotkey;
  }

  public override string ToString() => this.Keys ?? "Not Set";
}
