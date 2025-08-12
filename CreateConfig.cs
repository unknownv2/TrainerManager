
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace TrainerManager;

public class CreateConfig : Form
{
  private readonly string _path;
  private IContainer components;
  private Label lblInstructions;
  private Button btnPick;

  public CreateConfig(string path)
  {
    this._path = path;
    this.InitializeComponent();
  }

  private void btnPick_Click(object sender, EventArgs e)
  {
    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
    {
      ShowNewFolderButton = false,
      SelectedPath = Path.Combine(folderPath, "Documents", "GitHub"),
      Description = "Select the folder that contains your local trainer repository cloned from GitHub."
    };
    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
      return;
    if (!Directory.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "Games")))
    {
      int num = (int) MessageBox.Show("The selected folder isn't a valid trainer repository.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }
    else
    {
      File.WriteAllText(this._path, JsonConvert.SerializeObject((object) new Config()
      {
        Repos = new List<string>()
        {
          folderBrowserDialog.SelectedPath
        }
      }, Formatting.Indented));
      Process.Start(Application.ExecutablePath);
      this.Close();
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (CreateConfig));
    this.lblInstructions = new Label();
    this.btnPick = new Button();
    this.SuspendLayout();
    this.lblInstructions.Location = new Point(12, 8);
    this.lblInstructions.Name = "lblInstructions";
    this.lblInstructions.Size = new Size(260, 56);
    this.lblInstructions.TabIndex = 0;
    this.lblInstructions.Text = "Select the folder that contains your local trainer repository.";
    this.lblInstructions.TextAlign = ContentAlignment.MiddleCenter;
    this.btnPick.Location = new Point(12, 71);
    this.btnPick.Name = "btnPick";
    this.btnPick.Size = new Size(260, 39);
    this.btnPick.TabIndex = 1;
    this.btnPick.Text = "Pick Folder...";
    this.btnPick.UseVisualStyleBackColor = true;
    this.btnPick.Click += new EventHandler(this.btnPick_Click);
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(284, 122);
    this.Controls.Add((Control) this.btnPick);
    this.Controls.Add((Control) this.lblInstructions);
    this.Font = new Font("Consolas", 7.8f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
    this.MaximizeBox = false;
    this.Name = nameof (CreateConfig);
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Trainer Manager";
    this.ResumeLayout(false);
  }
}
