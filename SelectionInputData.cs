
using System.Collections.Generic;
using System.ComponentModel;


namespace TrainerManager;

internal class SelectionInputData : InputData
{
  public SelectionInputData(InputEditor editor)
    : base(editor, 2)
  {
    this.Input.Args.Defer = new bool?((this.Input.Args.Defer ?? true) != false);
    this.Input.Args.Options = this.Input.Args.Options ?? new List<string>();
  }

  public override InputType Type
  {
    get => InputType.Selection;
    set => this.TypeChanged(value);
  }

  public override string Target
  {
    get => this.Input.Target;
    set => this.SetTarget(value);
  }

  [DisplayName("Auto Set")]
  [Description("True if the value should be set when the slider or numeric input value changes. Otherwise there will be a button that the user must press.")]
  public bool AutoSet
  {
    get => !this.Input.Args.Defer.Value;
    set => this.Input.Args.Defer = new bool?(!value);
  }

  [DisplayName("Options")]
  [Description("The user-selectable options. The target variable is set to the 0-based index of the user's selection.")]
  public List<string> Options => this.Input.Args.Options;

  [DisplayName("Next Hotkey")]
  [Description("The hotkey used to select the next option.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Next => new HotkeyData(this.Input, 0);

  [DisplayName("Prev Hotkey")]
  [Description("The hotkey used to select the previous option.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Previous => new HotkeyData(this.Input, 1);

  public override string Name
  {
    get => this.Input.Name;
    set => this.SetName(value);
  }
}
