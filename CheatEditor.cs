
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace TrainerManager;

public sealed class CheatEditor : UserControl
{
  private IContainer components;
  private ContextMenuStrip contextMenuStripInputs;
  private ToolStripMenuItem btnDeleteInput;
  private Button btnUp;
  private Button btnDown;
  private TabControl inputTabs;
  private TextBox txtDescription;
  private TextBox txtName;

  public Cheat Cheat { get; }

  public string CurrentName => this.txtName.Text.Trim();

  public CheatEditor(Cheat cheat)
  {
    this.Cheat = cheat;
    this.InitializeComponent();
    this.Refresh();
  }

  private static CheatInput MakeInput()
  {
    return new CheatInput()
    {
      Type = "toggle",
      Target = "",
      Hotkeys = new List<HotkeyInput>()
      {
        new HotkeyInput() { Keys = KeyConvert.F1 }
      },
      Args = new InputArgs()
    };
  }

  public override void Refresh()
  {
    this.txtName.Text = this.Cheat.Name;
    this.txtDescription.Text = this.Cheat.Description;
    this.inputTabs.TabPages.Clear();
    if (this.Cheat.Inputs.Count == 0)
      this.Cheat.Inputs.Add(new CheatInput()
      {
        Type = "toggle",
        Target = "",
        Hotkeys = new List<HotkeyInput>(),
        Args = new InputArgs()
      });
    foreach (CheatInput input in this.Cheat.Inputs)
      this.inputTabs.TabPages.Add(CheatEditor.MakeInputTab(input));
    base.Refresh();
  }

  private static TabPage MakeInputTab(CheatInput input)
  {
    TabPage tabPage = new TabPage(input.Type[0].ToString().ToUpper() + input.Type.Substring(1));
    Control.ControlCollection controls = tabPage.Controls;
    InputEditor inputEditor = new InputEditor(input);
    inputEditor.Dock = DockStyle.Fill;
    controls.Add((Control) inputEditor);
    return tabPage;
  }

  public Cheat Save()
  {
    if (this.Cheat.Uuid == null || !string.Equals(this.txtName.Text, this.Cheat.Name, StringComparison.InvariantCultureIgnoreCase))
      this.Cheat.Uuid = RandomString.Make();
    this.Cheat.Name = this.txtName.Text.Trim();
    this.Cheat.Description = this.txtDescription.Text.Trim().Length == 0 ? (string) null : this.txtDescription.Text.Trim();
    if (this.Cheat.Name.Length == 0)
      throw new Exception("One of your cheat names is blank!");
    this.Cheat.Inputs = this.inputTabs.TabPages.OfType<TabPage>().SelectMany<TabPage, InputEditor>((Func<TabPage, IEnumerable<InputEditor>>) (t => t.Controls.OfType<InputEditor>())).Select<InputEditor, CheatInput>((Func<InputEditor, CheatInput>) (i => i.Save())).ToList<CheatInput>();
    return this.Cheat;
  }

  private void btnDeleteInput_Click(object sender, EventArgs e)
  {
    InputEditor control = (InputEditor) this.inputTabs.SelectedTab.Controls[0];
    if (this.inputTabs.TabCount == 1)
    {
      int num = (int) MessageBox.Show("You can't delete the cheat's only input.", "Can't Delete", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }
    else
    {
      if (MessageBox.Show("Are you sure you want to delete the currently selected input?", "Delete input?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) != DialogResult.Yes)
        return;
      this.Cheat.Inputs.Remove(control.Input);
      this.inputTabs.TabPages.Remove(this.inputTabs.SelectedTab);
      this.inputTabs.SelectTab(this.inputTabs.TabPages[0]);
      Main.RefreshHotkeyOverview();
    }
  }

  private void CheatEditor_DoubleClick(object sender, EventArgs e)
  {
    CheatInput input = CheatEditor.MakeInput();
    this.Cheat.Inputs.Add(input);
    TabPage tabPage = CheatEditor.MakeInputTab(input);
    this.inputTabs.TabPages.Add(tabPage);
    this.inputTabs.SelectedTab = tabPage;
    Main.RefreshHotkeyOverview();
  }

  private void txtName_TextChanged(object sender, EventArgs e)
  {
    if (this.Parent == null)
      return;
    this.Cheat.Name = this.txtName.Text.Trim().Length != 0 ? this.txtName.Text.Trim() : "Unnamed Cheat";
    Main.RefreshHotkeyOverview();
  }

  private void btnUp_Click(object sender, EventArgs e)
  {
    int newIndex = this.Parent.Controls.IndexOf((Control) this) - 1;
    if (newIndex < 0)
      return;
    this.Parent.Controls.SetChildIndex((Control) this, newIndex);
  }

  private void btnDown_Click(object sender, EventArgs e)
  {
    int newIndex = this.Parent.Controls.IndexOf((Control) this) + 1;
    if (newIndex >= this.Parent.Controls.Count)
      return;
    this.Parent.Controls.SetChildIndex((Control) this, newIndex);
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
    this.contextMenuStripInputs = new ContextMenuStrip(this.components);
    this.btnDeleteInput = new ToolStripMenuItem();
    this.btnUp = new Button();
    this.btnDown = new Button();
    this.inputTabs = new TabControl();
    this.txtDescription = new TextBox();
    this.txtName = new TextBox();
    this.contextMenuStripInputs.SuspendLayout();
    this.SuspendLayout();
    this.contextMenuStripInputs.ImageScalingSize = new Size(20, 20);
    this.contextMenuStripInputs.Items.AddRange(new ToolStripItem[1]
    {
      (ToolStripItem) this.btnDeleteInput
    });
    this.contextMenuStripInputs.Name = "contextMenuStripInputs";
    this.contextMenuStripInputs.Size = new Size(161, 28);
    this.btnDeleteInput.Name = "btnDeleteInput";
    this.btnDeleteInput.Size = new Size(160 /*0xA0*/, 24);
    this.btnDeleteInput.Text = "Delete Input";
    this.btnDeleteInput.Click += new EventHandler(this.btnDeleteInput_Click);
    this.btnUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.btnUp.Font = new Font("Microsoft Sans Serif", 10.2f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.btnUp.Location = new Point(558, 31 /*0x1F*/);
    this.btnUp.Name = "btnUp";
    this.btnUp.Size = new Size(31 /*0x1F*/, 31 /*0x1F*/);
    this.btnUp.TabIndex = 3;
    this.btnUp.Text = "￪";
    this.btnUp.TextAlign = ContentAlignment.TopRight;
    this.btnUp.UseVisualStyleBackColor = true;
    this.btnUp.Click += new EventHandler(this.btnUp_Click);
    this.btnDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.btnDown.Font = new Font("Microsoft Sans Serif", 10.2f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.btnDown.Location = new Point(558, 68);
    this.btnDown.Name = "btnDown";
    this.btnDown.Size = new Size(31 /*0x1F*/, 31 /*0x1F*/);
    this.btnDown.TabIndex = 4;
    this.btnDown.Text = "￬";
    this.btnDown.TextAlign = ContentAlignment.TopRight;
    this.btnDown.UseVisualStyleBackColor = true;
    this.btnDown.Click += new EventHandler(this.btnDown_Click);
    this.inputTabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.inputTabs.ContextMenuStrip = this.contextMenuStripInputs;
    this.inputTabs.Location = new Point(217, 8);
    this.inputTabs.Name = "inputTabs";
    this.inputTabs.SelectedIndex = 0;
    this.inputTabs.Size = new Size(335, 178);
    this.inputTabs.TabIndex = 38;
    this.txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
    this.txtDescription.BackColor = SystemColors.Window;
    this.txtDescription.Location = new Point(8, 36);
    this.txtDescription.MaxLength = (int) byte.MaxValue;
    this.txtDescription.Multiline = true;
    this.txtDescription.Name = "txtDescription";
    this.txtDescription.Size = new Size(198, 150);
    this.txtDescription.TabIndex = 37;
    this.txtName.Location = new Point(8, 8);
    this.txtName.MaxLength = 40;
    this.txtName.Name = "txtName";
    this.txtName.Size = new Size(198, 23);
    this.txtName.TabIndex = 36;
    this.txtName.TextAlign = HorizontalAlignment.Center;
    this.txtName.TextChanged += new EventHandler(this.txtName_TextChanged);
    this.AutoScaleMode = AutoScaleMode.None;
    this.Controls.Add((Control) this.inputTabs);
    this.Controls.Add((Control) this.txtDescription);
    this.Controls.Add((Control) this.txtName);
    this.Controls.Add((Control) this.btnDown);
    this.Controls.Add((Control) this.btnUp);
    this.Font = new Font("Consolas", 7.8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.Margin = new Padding(10, 10, 3, 0);
    this.Name = nameof (CheatEditor);
    this.Padding = new Padding(5);
    this.Size = new Size(597, 194);
    this.DoubleClick += new EventHandler(this.CheatEditor_DoubleClick);
    this.contextMenuStripInputs.ResumeLayout(false);
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
