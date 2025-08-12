
using System;
using System.ComponentModel;


namespace TrainerManager;

internal class NumericInputData : InputData
{
  public NumericInputData(InputEditor editor)
    : base(editor, 2)
  {
    InputArgs args1 = this.Input.Args;
    double? nullable1 = this.Input.Args.Min;
    double? nullable2 = new double?(nullable1 ?? 0.0);
    args1.Min = nullable2;
    InputArgs args2 = this.Input.Args;
    nullable1 = this.Input.Args.Max;
    double? nullable3 = new double?(nullable1 ?? 100.0);
    args2.Max = nullable3;
    InputArgs args3 = this.Input.Args;
    nullable1 = this.Input.Args.Step;
    double? nullable4 = new double?(nullable1 ?? 10.0);
    args3.Step = nullable4;
    InputArgs args4 = this.Input.Args;
    bool? nullable5 = this.Input.Args.Defer;
    bool? nullable6 = new bool?((nullable5 ?? true) != false);
    args4.Defer = nullable6;
    InputArgs args5 = this.Input.Args;
    nullable5 = this.Input.Args.PresentAsSlider;
    bool? nullable7 = new bool?((nullable5 ?? true) != false);
    args5.PresentAsSlider = nullable7;
  }

  public override InputType Type
  {
    get => InputType.Numeric;
    set => this.TypeChanged(value);
  }

  public override string Target
  {
    get => this.Input.Target;
    set => this.SetTarget(value);
  }

  [DisplayName("Minimum")]
  [Description("The minimum allowed value.")]
  public double Minimum
  {
    get => this.Input.Args.Min.Value;
    set
    {
      this.Input.Args.Min = value < this.Maximum ? new double?(value) : throw new ArgumentException("Minimum value must be less than the maximum.");
    }
  }

  [DisplayName("Maximum")]
  [Description("The maximum allowed value.")]
  public double Maximum
  {
    get => this.Input.Args.Max.Value;
    set
    {
      this.Input.Args.Max = value > this.Minimum ? new double?(value) : throw new ArgumentException("Maximum value must be greater than the minimum.");
    }
  }

  [DisplayName("Step")]
  [Description("The number to increase or decrease the value by.")]
  public double Step
  {
    get => this.Input.Args.Step.Value;
    set
    {
      double num = Math.Abs(value) >= 0.001 ? value : throw new Exception("Step cannot be less than 0.001.");
      double? max = this.Input.Args.Max;
      double? min = this.Input.Args.Min;
      double? nullable = max.HasValue & min.HasValue ? new double?(max.GetValueOrDefault() - min.GetValueOrDefault()) : new double?();
      double valueOrDefault = nullable.GetValueOrDefault();
      if ((num > valueOrDefault ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        throw new Exception("Step value cannot be less than the difference between min and max.");
      this.Input.Args.Step = new double?(value);
    }
  }

  [DisplayName("Inc Hotkey")]
  [Description("The hotkey used to increase the value.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Increase => new HotkeyData(this.Input, 0);

  [DisplayName("Dec Hotkey")]
  [Description("The hotkey used to decrease the value.")]
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public HotkeyData Decrease => new HotkeyData(this.Input, 1);

  [DisplayName("Display")]
  [Description("The type of control this input should be presented as in the remote.")]
  public NumericInputType Display
  {
    get
    {
      return (this.Input.Args.PresentAsSlider ?? true) == false ? NumericInputType.Input : NumericInputType.Slider;
    }
    set => this.Input.Args.PresentAsSlider = new bool?(value == NumericInputType.Slider);
  }

  [DisplayName("Auto Set")]
  [Description("True if the value should be set when the slider or numeric input value changes. Otherwise there will be a button that the user must press.")]
  public bool AutoSet
  {
    get => !this.Input.Args.Defer.Value;
    set => this.Input.Args.Defer = new bool?(!value);
  }

  public override string Name
  {
    get => this.Input.Name;
    set => this.SetName(value);
  }
}
