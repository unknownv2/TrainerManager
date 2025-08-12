
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace TrainerManager;

internal static class Program
{
  [STAThread]
  private static void Main()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".infinity");
    if (!File.Exists(path))
    {
      Application.Run((Form) new CreateConfig(path));
    }
    else
    {
      Config config;
      try
      {
        config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path, Encoding.UTF8));
      }
      catch
      {
        Application.Run((Form) new CreateConfig(path));
        return;
      }
      Application.Run((Form) new TrainerManager.Main(config));
    }
  }
}
