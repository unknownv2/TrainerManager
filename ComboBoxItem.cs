

namespace TrainerManager;

public class ComboBoxItem
{
  public string Text { get; set; }

  public object Value { get; set; }

  public ComboBoxItem(string text, object value)
  {
    this.Text = text;
    this.Value = value;
  }

  public override string ToString() => this.Text;
}
