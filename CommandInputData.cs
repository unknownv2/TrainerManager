

using System.ComponentModel;


namespace TrainerManager;

internal class CommandInputData(InputEditor editor) : InputData(editor, 1)
{
  public override InputType Type
  {
    get => InputType.Command;
    set => this.TypeChanged(value);
  }

  public override string Target
  {
    get => this.Input.Target;
    set => this.SetTarget(value);
  }

  [DisplayName("Hotkey")]
  [Description("The hotkey that triggers the command in Infinity.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Hotkey => new HotkeyData(this.Input, 0);

  [DisplayName("Value")]
  [Description("An optional value to set the variable to. Otheriwse a random number will be used.")]
  public double? Value
  {
    get => this.Input.Args.Value;
    set => this.Input.Args.Value = value;
  }

  public override string Name
  {
    get => this.Input.Name;
    set => this.SetName(value);
  }
}
