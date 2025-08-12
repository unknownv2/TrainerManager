
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace TrainerManager.CheatEngine;

public class CheatTableVersionSelector : Form
{
  private readonly Game _game;
  private IContainer components;
  private Label lblDescription;
  private ListBox listVersions;
  private TextBox txtNew;
  private Label lblNew;
  private Button btnImport;
  private Button btnReadTimestamp;

  public GameVersion SelectedVersion { get; private set; }

  public CheatTableVersionSelector(Game game)
  {
    this._game = game;
    this.InitializeComponent();
    this.PopulateVersions(this.ReadVersions());
  }

  private void PopulateVersions(IEnumerable<GameVersion> versions)
  {
    foreach (object version in versions)
      this.listVersions.Items.Add(version);
  }

  private IEnumerable<GameVersion> ReadVersions()
  {
    foreach (string enumerateFile in Directory.EnumerateFiles(this._game.Location, "*.cpp"))
    {
      string str = enumerateFile.Substring(enumerateFile.LastIndexOf('\\') + 1);
      string s = ((IEnumerable<string>) str.Remove(str.Length - 4).Split(new string[1]
      {
        "- "
      }, StringSplitOptions.None)).Last<string>();
      uint result;
      if (s.Length == 10 && uint.TryParse(s, out result))
        yield return new GameVersion()
        {
          Filename = str,
          Name = str.Substring(0, str.Length - 18).Trim(),
          Timestamp = result
        };
    }
    yield return GameVersion.Unknown;
  }

  private void btnImport_Click(object sender, EventArgs e)
  {
    if (MessageBox.Show("This will replace the trainer and all of its code with the data in the cheat table.\r\n\r\nAre you sure you want to do this?", "Overwrite trainer?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) != DialogResult.Yes)
      return;
    if (this.txtNew.TextLength != 0)
    {
      uint result;
      if (this.txtNew.TextLength != 10 || !uint.TryParse(this.txtNew.Text, out result))
      {
        int num = (int) MessageBox.Show("Game version must be a 10-digit timestamp.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return;
      }
      this.SelectedVersion = GameVersion.FromTimestamp(result);
    }
    else
    {
      if (this.listVersions.SelectedItem == null)
      {
        int num = (int) MessageBox.Show("Select or enter a game version / timestamp.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return;
      }
      this.SelectedVersion = (GameVersion) this.listVersions.SelectedItem;
    }
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void txtNew_TextChanged(object sender, EventArgs e)
  {
    this.listVersions.Enabled = this.txtNew.TextLength == 0;
  }

  private void btnReadTimestamp_Click(object sender, EventArgs e)
  {
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.Filter = "EXE|*.exe;*.dll";
    openFileDialog.Title = "Select the game's EXE";
    if (openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
      return;
    try
    {
      using (Stream input = openFileDialog.OpenFile())
      {
        BinaryReader binaryReader = new BinaryReader(input);
        input.Position = 60L;
        input.Position = (long) (binaryReader.ReadUInt32() + 8U);
        uint timestamp = binaryReader.ReadUInt32();
        GameVersion gameVersion = this.listVersions.Items.Cast<GameVersion>().FirstOrDefault<GameVersion>((Func<GameVersion, bool>) (v => (int) v.Timestamp == (int) timestamp));
        if (gameVersion != null)
          this.listVersions.SelectedItem = (object) gameVersion;
        else
          this.txtNew.Text = timestamp.ToString();
      }
    }
    catch
    {
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.lblDescription = new Label();
    this.listVersions = new ListBox();
    this.txtNew = new TextBox();
    this.lblNew = new Label();
    this.btnImport = new Button();
    this.btnReadTimestamp = new Button();
    this.SuspendLayout();
    this.lblDescription.Location = new Point(13, 9);
    this.lblDescription.Name = "lblDescription";
    this.lblDescription.Size = new Size(234, 40);
    this.lblDescription.TabIndex = 0;
    this.lblDescription.Text = "Select the version of the game the imported scripts are for.";
    this.listVersions.FormattingEnabled = true;
    this.listVersions.ItemHeight = 16 /*0x10*/;
    this.listVersions.Location = new Point(12, 52);
    this.listVersions.Name = "listVersions";
    this.listVersions.Size = new Size(235, 84);
    this.listVersions.TabIndex = 13;
    this.txtNew.Location = new Point((int) sbyte.MaxValue, 142);
    this.txtNew.Name = "txtNew";
    this.txtNew.Size = new Size(120, 22);
    this.txtNew.TabIndex = 12;
    this.txtNew.TextChanged += new EventHandler(this.txtNew_TextChanged);
    this.lblNew.AutoSize = true;
    this.lblNew.Location = new Point(82, 145);
    this.lblNew.Name = "lblNew";
    this.lblNew.Size = new Size(39, 17);
    this.lblNew.TabIndex = 14;
    this.lblNew.Text = "New:";
    this.btnImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.btnImport.Location = new Point(12, 206);
    this.btnImport.Name = "btnImport";
    this.btnImport.Size = new Size(235, 39);
    this.btnImport.TabIndex = 15;
    this.btnImport.Text = "Finish Import";
    this.btnImport.UseVisualStyleBackColor = true;
    this.btnImport.Click += new EventHandler(this.btnImport_Click);
    this.btnReadTimestamp.Location = new Point((int) sbyte.MaxValue, 170);
    this.btnReadTimestamp.Name = "btnReadTimestamp";
    this.btnReadTimestamp.Size = new Size(120, 30);
    this.btnReadTimestamp.TabIndex = 16 /*0x10*/;
    this.btnReadTimestamp.Text = "Open EXE...";
    this.btnReadTimestamp.UseVisualStyleBackColor = true;
    this.btnReadTimestamp.Click += new EventHandler(this.btnReadTimestamp_Click);
    this.AutoScaleDimensions = new SizeF(8f, 16f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(259, 257);
    this.Controls.Add((Control) this.btnReadTimestamp);
    this.Controls.Add((Control) this.btnImport);
    this.Controls.Add((Control) this.lblNew);
    this.Controls.Add((Control) this.listVersions);
    this.Controls.Add((Control) this.txtNew);
    this.Controls.Add((Control) this.lblDescription);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.Name = nameof (CheatTableVersionSelector);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Select the Game Version";
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
