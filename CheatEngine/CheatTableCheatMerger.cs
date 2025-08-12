
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace TrainerManager.CheatEngine;

public class CheatTableCheatMerger : Form
{
  private bool _checkHandlerSuppressed = true;
  private IContainer components;
  private ListView listNew;
  private ListView listCurrent;
  private CheckBox ckCheckAllCurrent;
  private CheckBox ckCheckAllNew;
  private Label lblCurrent;
  private Label lblImported;
  private Button btnNext;
  private Label lblDescription;

  public List<Cheat> Cheats { get; private set; }

  public CheatTableCheatMerger(List<Cheat> currentCheats, List<Cheat> importedCheats)
  {
    this.InitializeComponent();
    List<string> stringList = new List<string>();
    foreach (Cheat currentCheat1 in currentCheats)
    {
      Cheat currentCheat = currentCheat1;
      Cheat cheat = importedCheats.FirstOrDefault<Cheat>((Func<Cheat, bool>) (c => c.Name == currentCheat.Name));
      if (cheat != null)
      {
        string str = currentCheat.Uuid;
        if (stringList.Contains(str))
          str = RandomString.Make();
        stringList.Add(str);
        cheat.Uuid = str;
      }
    }
    CheatTableCheatMerger.PopulateList(this.listCurrent, currentCheats);
    CheatTableCheatMerger.PopulateList(this.listNew, importedCheats);
    this.InitializeCheckBoxes();
  }

  private static ListViewItem GetMatchingCheatItem(ListView list, Cheat cheat)
  {
    return list.Items.Cast<ListViewItem>().FirstOrDefault<ListViewItem>((Func<ListViewItem, bool>) (i => ((Cheat) i.Tag).Name == cheat.Name));
  }

  private void btnNext_Click(object sender, EventArgs e)
  {
    List<Cheat> source = new List<Cheat>();
    foreach (ListViewItem listViewItem in this.listCurrent.Items.Cast<ListViewItem>())
    {
      if (listViewItem.Checked)
      {
        source.Add((Cheat) listViewItem.Tag);
      }
      else
      {
        ListViewItem matchingCheatItem = CheatTableCheatMerger.GetMatchingCheatItem(this.listNew, (Cheat) listViewItem.Tag);
        if (matchingCheatItem.Checked)
          source.Add((Cheat) matchingCheatItem.Tag);
      }
    }
    foreach (ListViewItem listViewItem in this.listNew.Items.Cast<ListViewItem>().Where<ListViewItem>((Func<ListViewItem, bool>) (i => i.Checked)))
    {
      if (CheatTableCheatMerger.GetMatchingCheatItem(this.listCurrent, (Cheat) listViewItem.Tag) == null)
        source.Add((Cheat) listViewItem.Tag);
    }
    foreach (Cheat cheat in source.Where<Cheat>((Func<Cheat, bool>) (c => c.Uuid == null)))
      cheat.Uuid = RandomString.Make();
    this.Cheats = source;
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void InitializeCheckBoxes()
  {
    foreach (ListViewItem listViewItem in this.listCurrent.Items.Cast<ListViewItem>())
    {
      if (CheatTableCheatMerger.GetMatchingCheatItem(this.listNew, (Cheat) listViewItem.Tag) == null)
        listViewItem.Checked = true;
    }
    foreach (ListViewItem listViewItem in this.listNew.Items.Cast<ListViewItem>())
      listViewItem.Checked = true;
    CheatTableCheatMerger.UpdateCheckAllState(this.listCurrent, this.ckCheckAllCurrent);
    CheatTableCheatMerger.UpdateCheckAllState(this.listNew, this.ckCheckAllNew);
  }

  private static void PopulateList(ListView list, List<Cheat> cheats)
  {
    list.BeginUpdate();
    list.Items.Clear();
    list.Items.AddRange(cheats.Select<Cheat, ListViewItem>(new Func<Cheat, ListViewItem>(CheatTableCheatMerger.MakeItem)).ToArray<ListViewItem>());
    list.EndUpdate();
  }

  private static ListViewItem MakeItem(Cheat cheat)
  {
    return new ListViewItem(cheat.Name)
    {
      Tag = (object) cheat
    };
  }

  private static void CheckAll(ListView list, bool check)
  {
    foreach (ListViewItem listViewItem in list.Items)
      listViewItem.Checked = check;
  }

  private void ckCheckAllCurrent_CheckedChanged(object sender, EventArgs e)
  {
    if (this._checkHandlerSuppressed)
      return;
    if (this.ckCheckAllCurrent.CheckState == CheckState.Checked)
    {
      CheatTableCheatMerger.CheckAll(this.listCurrent, true);
    }
    else
    {
      if (this.ckCheckAllCurrent.CheckState != CheckState.Unchecked)
        return;
      CheatTableCheatMerger.CheckAll(this.listCurrent, false);
    }
  }

  private static void UncheckMatchingCheat(ListView list, Cheat cheat)
  {
    foreach (ListViewItem listViewItem in list.Items)
    {
      if (((Cheat) listViewItem.Tag).Name == cheat.Name)
        listViewItem.Checked = false;
    }
  }

  private static void UpdateCheckAllState(ListView list, CheckBox checkBox)
  {
    if (list.Items.Cast<ListViewItem>().All<ListViewItem>((Func<ListViewItem, bool>) (i => i.Checked)))
      checkBox.CheckState = CheckState.Checked;
    else if (list.Items.Cast<ListViewItem>().All<ListViewItem>((Func<ListViewItem, bool>) (i => !i.Checked)))
      checkBox.CheckState = CheckState.Unchecked;
    else
      checkBox.CheckState = CheckState.Indeterminate;
  }

  private void ckCheckAllNew_CheckedChanged(object sender, EventArgs e)
  {
    if (this._checkHandlerSuppressed)
      return;
    if (this.ckCheckAllNew.CheckState == CheckState.Checked)
    {
      CheatTableCheatMerger.CheckAll(this.listNew, true);
    }
    else
    {
      if (this.ckCheckAllNew.CheckState != CheckState.Unchecked)
        return;
      CheatTableCheatMerger.CheckAll(this.listNew, false);
    }
  }

  private void OnCurrentCheatChecked(object sender, ItemCheckedEventArgs e)
  {
    if (this._checkHandlerSuppressed)
      return;
    this._checkHandlerSuppressed = true;
    CheatTableCheatMerger.UncheckMatchingCheat(this.listNew, (Cheat) e.Item.Tag);
    CheatTableCheatMerger.UpdateCheckAllState(this.listCurrent, this.ckCheckAllCurrent);
    CheatTableCheatMerger.UpdateCheckAllState(this.listNew, this.ckCheckAllNew);
    this._checkHandlerSuppressed = false;
  }

  private void OnImportedCheatChecked(object sender, ItemCheckedEventArgs e)
  {
    if (this._checkHandlerSuppressed)
      return;
    this._checkHandlerSuppressed = true;
    CheatTableCheatMerger.UncheckMatchingCheat(this.listCurrent, (Cheat) e.Item.Tag);
    CheatTableCheatMerger.UpdateCheckAllState(this.listCurrent, this.ckCheckAllCurrent);
    CheatTableCheatMerger.UpdateCheckAllState(this.listNew, this.ckCheckAllNew);
    this._checkHandlerSuppressed = false;
  }

  private void CheatTableImporter_Load(object sender, EventArgs e)
  {
    this._checkHandlerSuppressed = false;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.listNew = new ListView();
    this.listCurrent = new ListView();
    this.ckCheckAllCurrent = new CheckBox();
    this.ckCheckAllNew = new CheckBox();
    this.lblCurrent = new Label();
    this.lblImported = new Label();
    this.btnNext = new Button();
    this.lblDescription = new Label();
    this.SuspendLayout();
    this.listNew.CheckBoxes = true;
    this.listNew.Location = new Point(286, 70);
    this.listNew.Name = "listNew";
    this.listNew.Size = new Size(266, 315);
    this.listNew.TabIndex = 2;
    this.listNew.UseCompatibleStateImageBehavior = false;
    this.listNew.View = View.List;
    this.listNew.ItemChecked += new ItemCheckedEventHandler(this.OnImportedCheatChecked);
    this.listCurrent.CheckBoxes = true;
    this.listCurrent.Location = new Point(12, 70);
    this.listCurrent.Name = "listCurrent";
    this.listCurrent.Size = new Size(263, 315);
    this.listCurrent.TabIndex = 3;
    this.listCurrent.UseCompatibleStateImageBehavior = false;
    this.listCurrent.View = View.List;
    this.listCurrent.ItemChecked += new ItemCheckedEventHandler(this.OnCurrentCheatChecked);
    this.ckCheckAllCurrent.AutoSize = true;
    this.ckCheckAllCurrent.Location = new Point(187, 43);
    this.ckCheckAllCurrent.Name = "ckCheckAllCurrent";
    this.ckCheckAllCurrent.Size = new Size(88, 21);
    this.ckCheckAllCurrent.TabIndex = 4;
    this.ckCheckAllCurrent.Text = "Check All";
    this.ckCheckAllCurrent.ThreeState = true;
    this.ckCheckAllCurrent.UseVisualStyleBackColor = true;
    this.ckCheckAllCurrent.CheckedChanged += new EventHandler(this.ckCheckAllCurrent_CheckedChanged);
    this.ckCheckAllNew.AutoSize = true;
    this.ckCheckAllNew.Location = new Point(464, 43);
    this.ckCheckAllNew.Name = "ckCheckAllNew";
    this.ckCheckAllNew.Size = new Size(88, 21);
    this.ckCheckAllNew.TabIndex = 5;
    this.ckCheckAllNew.Text = "Check All";
    this.ckCheckAllNew.ThreeState = true;
    this.ckCheckAllNew.UseVisualStyleBackColor = true;
    this.ckCheckAllNew.CheckedChanged += new EventHandler(this.ckCheckAllNew_CheckedChanged);
    this.lblCurrent.AutoSize = true;
    this.lblCurrent.Location = new Point(12, 44);
    this.lblCurrent.Name = "lblCurrent";
    this.lblCurrent.Size = new Size(103, 17);
    this.lblCurrent.TabIndex = 6;
    this.lblCurrent.Text = "Current Cheats";
    this.lblImported.AutoSize = true;
    this.lblImported.Location = new Point(283, 44);
    this.lblImported.Name = "lblImported";
    this.lblImported.Size = new Size(111, 17);
    this.lblImported.TabIndex = 7;
    this.lblImported.Text = "Imported Cheats";
    this.btnNext.Location = new Point(403, 391);
    this.btnNext.Name = "btnNext";
    this.btnNext.Size = new Size(149, 36);
    this.btnNext.TabIndex = 9;
    this.btnNext.Text = "Next";
    this.btnNext.UseVisualStyleBackColor = true;
    this.btnNext.Click += new EventHandler(this.btnNext_Click);
    this.lblDescription.AutoSize = true;
    this.lblDescription.Location = new Point(12, 9);
    this.lblDescription.Name = "lblDescription";
    this.lblDescription.Size = new Size(260, 17);
    this.lblDescription.TabIndex = 10;
    this.lblDescription.Text = "Select the cheats that you want to keep.";
    this.AutoScaleDimensions = new SizeF(8f, 16f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.ClientSize = new Size(564, 439);
    this.Controls.Add((Control) this.lblDescription);
    this.Controls.Add((Control) this.btnNext);
    this.Controls.Add((Control) this.lblImported);
    this.Controls.Add((Control) this.lblCurrent);
    this.Controls.Add((Control) this.ckCheckAllNew);
    this.Controls.Add((Control) this.ckCheckAllCurrent);
    this.Controls.Add((Control) this.listCurrent);
    this.Controls.Add((Control) this.listNew);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.Name = nameof (CheatTableCheatMerger);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Merge Cheats";
    this.Load += new EventHandler(this.CheatTableImporter_Load);
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
