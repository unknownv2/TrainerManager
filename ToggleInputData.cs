
using System.ComponentModel;


namespace TrainerManager;

internal class ToggleInputData(InputEditor editor) : InputData(editor, 1)
{
  public override InputType Type
  {
    get => InputType.Toggle;
    set => this.TypeChanged(value);
  }

  public override string Target
  {
    get => this.Input.Target;
    set => this.SetTarget(value);
  }

  [DisplayName("Hotkey")]
  [Description("The hotkey that triggers the toggle in Infinity.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Hotkey => new HotkeyData(this.Input, 0);

  public override string Name
  {
    get => this.Input.Name;
    set => this.SetName(value);
  }
}
