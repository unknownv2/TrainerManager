
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace TrainerManager;

public sealed class InputEditor : UserControl
{
  private bool _lostFocus = true;
  private string _lastHotkey;
  private bool _settingHotkey;
  private IContainer components;
  private PropertyGrid propertyGrid;

  public CheatInput Input { get; }

  public InputEditor(CheatInput input)
  {
    this.Input = input;
    this.InitializeComponent();
    this.LoadType(input.Type);
    this.Load += (EventHandler) ((s, e) => this.SetLabelColumnWidth());
    this.propertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(this.SelectedGridItemChanged);
  }

  private bool IsHotkeySelected
  {
    get => !this._lostFocus && this.propertyGrid.SelectedGridItem?.Label == "Keys";
  }

  private void SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
  {
    GridItem newSelection = e.NewSelection;
    if (newSelection == null || newSelection.GridItemType != GridItemType.Property || newSelection.Label != "Keys")
      return;
    IContainerControl containerControl = this.propertyGrid.GetContainerControl();
    if (containerControl == null)
      return;
    if (!(containerControl.ActiveControl?.Controls[0] is TextBox control))
      control = containerControl.ActiveControl?.Controls[1] as TextBox;
    TextBox textBox = control;
    if (textBox == null)
      return;
    this._lostFocus = false;
    if (textBox.Tag != null)
      return;
    textBox.Tag = (object) true;
    textBox.ShortcutsEnabled = false;
    textBox.TextChanged += new EventHandler(this.OnTextChanged);
    textBox.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
    textBox.KeyDown += new KeyEventHandler(this.OnHotkeyDown);
    textBox.LostFocus += (EventHandler) ((s, ev) => this._lostFocus = true);
  }

  private void OnHotkeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsHotkeySelected)
      return;
    ((TextBoxBase) sender).SelectionLength = 0;
    e.Handled = true;
  }

  private void OnTextChanged(object sender, EventArgs e)
  {
    if (!this.IsHotkeySelected || this._lastHotkey == null)
      return;
    TextBox textBox = (TextBox) sender;
    if (this._settingHotkey)
      return;
    this._settingHotkey = true;
    textBox.Text = this._lastHotkey;
    this._settingHotkey = false;
  }

  private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
  {
    if (!this.IsHotkeySelected)
      return;
    if (!(this.propertyGrid.SelectedGridItem.Parent.Value is HotkeyData hotkeyData))
    {
      this._lastHotkey = (string) null;
    }
    else
    {
      int[] hotkey = KeyConvert.FromKeyboardEvent((object) e, Control.MouseButtons);
      hotkeyData.SetHotkey(hotkey);
      string str = KeyConvert.ToString(hotkey);
      if (str != this._lastHotkey)
        Main.RefreshHotkeyOverview();
      this._lastHotkey = str;
      this.OnTextChanged(sender, (EventArgs) e);
    }
  }

  private void SetLabelColumnWidth()
  {
        /*
        Control control = (Control)this.propertyGrid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object)this.propertyGrid);
        (control.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) control, new object[1]
    {
      (object) 125
    }))*/;
  }

  public void LoadType(string type)
  {
    InputData inputData;
    switch (type)
    {
      case "toggle":
        inputData = (InputData) new ToggleInputData(this);
        break;
      case "command":
        inputData = (InputData) new CommandInputData(this);
        break;
      case "numeric":
        inputData = (InputData) new NumericInputData(this);
        break;
      case "selection":
        inputData = (InputData) new SelectionInputData(this);
        break;
      default:
        throw new ArgumentException($"Unknown input type '{type}'.");
    }
    this.Input.Type = type;
    this.propertyGrid.SelectedObject = (object) inputData;
    if (type == "toggle" || type == "command")
    {
      GridItem gridItem1 = this.propertyGrid.SelectedGridItem;
      while (gridItem1.Parent != null)
        gridItem1 = gridItem1.Parent;
      GridItem gridItem2 = gridItem1.GridItems.OfType<GridItem>().FirstOrDefault<GridItem>((Func<GridItem, bool>) (i => i.Label == "Hotkey"));
      if (gridItem2 != null)
        gridItem2.Expanded = true;
    }
    if (this.Parent == null)
      return;
    ((TabControl) this.Parent.Parent).SelectedTab.Text = type[0].ToString().ToUpper() + type.Substring(1);
  }

  public CheatInput Save()
  {
    this.Input.Target = this.Input.Target.Trim();
    if (string.IsNullOrEmpty(this.Input.Target))
      throw new Exception("Target cannot be empty.");
    switch (this.Input.Type)
    {
      case "toggle":
        this.ResizeHotkeys(1);
        this.Input.Args = new InputArgs();
        break;
      case "command":
        this.ResizeHotkeys(1);
        this.Input.Args = new InputArgs()
        {
          Value = this.Input.Args.Value
        };
        break;
      case "numeric":
        this.ResizeHotkeys(2);
        this.Input.Args = new InputArgs()
        {
          Min = this.Input.Args.Min,
          Max = this.Input.Args.Max,
          Step = this.Input.Args.Step,
          PresentAsSlider = this.Input.Args.PresentAsSlider,
          Defer = this.Input.Args.Defer
        };
        break;
      case "selection":
        this.ResizeHotkeys(2);
        this.Input.Args = new InputArgs()
        {
          Defer = this.Input.Args.Defer,
          Options = this.Input.Args.Options
        };
        break;
    }
    this.LoadType(this.Input.Type);
    return this.Input;
  }

  private void ResizeHotkeys(int size)
  {
    while (this.Input.Hotkeys.Count < size)
      this.Input.Hotkeys.Add((HotkeyInput) null);
    while (this.Input.Hotkeys.Count > size)
      this.Input.Hotkeys.RemoveAt(this.Input.Hotkeys.Count - 1);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.propertyGrid = new PropertyGrid();
    this.SuspendLayout();
    this.propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.propertyGrid.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.propertyGrid.HelpVisible = false;
    this.propertyGrid.LineColor = SystemColors.Window;
    this.propertyGrid.Location = new Point(0, 0);
    this.propertyGrid.Name = "propertyGrid";
    this.propertyGrid.PropertySort = PropertySort.NoSort;
    this.propertyGrid.Size = new Size(257, 200);
    this.propertyGrid.TabIndex = 28;
    this.propertyGrid.ToolbarVisible = false;
    this.propertyGrid.ViewBorderColor = SystemColors.Window;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.propertyGrid);
    this.Font = new Font("Consolas", 7.8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.Name = nameof (InputEditor);
    this.Padding = new Padding(4, 5, 4, 5);
    this.Size = new Size(257, 200);
    this.ResumeLayout(false);
  }
}
